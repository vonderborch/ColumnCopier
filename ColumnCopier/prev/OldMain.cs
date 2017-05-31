// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Main.cs
// Author           : Christian
// Created          : 08-15-2016
// 
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 05-30-2017
// ***********************************************************************
// <copyright file="Main.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Main class.
// </summary>
//
// Changelog: 
//            - 2.0.0 (xx-xx-2017) - 
//            - 1.3.0 (05-30-2017) - Removed dependency on Octokit. Made line copy pre-set options a combobox rather than separate buttons. Adjustments to saving and loading to handle cleaning column name and row data. CurrentColumn save field is now actually used.
//            - 1.2.4 (01-23-2017) - Slight UI changes, added new Copy and Replace option.
//            - 1.2.3 (01-23-2017) - Loading a request now updates the current line for copying lines.
//            - 1.2.2 (12-27-2016) - Reset isSaving toggle to allow tool to continue to be used on failure of a save. Upon changing of a request preservation status, status text is displayed and the program state is saved. Added 'Copy and Replace' quick button for formatting column to SQL-style text list.
//            - 1.2.1 (10-04-2016) - Hopefully resolved issue causing program to never enter a state where it can actually be used.
//            - 1.2.0 (09-30-2016) - Saves now occur on a separate thread and saves can be compressed. Added option for compressed saves. Added preserve request toggle support. Added the missing process start code to open the latest release (when an update is detected) and fixed update notice typo. Added option to delete a request.
//            - 1.1.6 (09-29-2016) - Fixed bug when loading old saves (bumped save version), fixed bug when copying invalid characters, fixed bug with no data in the clipboard.
//            - 1.1.5 (09-21-2016) - Added tooltips and the export functionality.
//            - 1.1.4 (09-21-2016) - Added pre and post text for copying and replacing. Removed excessive saving. Added more information to status text. Bumped save version. Number is actually the default selected priority now.
//            - 1.1.3 (08-30-2016) - Replaced string.format with new approach (more consistent with elsewhere in the program), enhanced error message during saving/loading.
//            - 1.1.2 (08-29-2016) - Fixed issue with loading requests.
//            - 1.1.1 (08-29-2016) - Version bump.
//            - 1.1.0 (08-29-2016) - Added status text field, update checker, and error message popups.
//            - 1.0.0 (08-22-2016) - Initial version finished.
//            - 0.5.0 (08-18-2016) - Initial version created.
//            - 0.0.0 (08-15-2016) - Initial version created.
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

/// <summary>
/// The ColumnCopier namespace.
/// </summary>
namespace ColumnCopier.prev
{
    /// <summary>
    /// Main class.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class OldMain : Form
    {
        #region Private Fields

        /// <summary>
        /// Delegate UpdateProgressBar
        /// </summary>
        private delegate void UpdateProgressBar();

        /// <summary>
        /// The busy save text
        /// </summary>
        private const string BusySaveText = "Currently processing another request, please try again!";

        /// <summary>
        /// The compressed save file extension
        /// </summary>
        private const string CompressedSaveFileExtension = ".ccsx";

        /// <summary>
        /// The current column format
        /// </summary>
        private const string CurrentColumnFormat = "Current Column #: {0}";

        /// <summary>
        /// The git current release tag
        /// </summary>
        private const int GitCurrentReleaseTagVersion = 200;

        /// <summary>
        /// The git repository
        /// </summary>
        private const string GitRepository = "ColumnCopier";

        /// <summary>
        /// The git user
        /// </summary>
        private const string GitUser = "vonderborch";

        /// <summary>
        /// The number of columns format
        /// </summary>
        private const string NumberOfColumnsFormat = "# of Columns: {0}";

        /// <summary>
        /// The number of rows format
        /// </summary>
        private const string NumberOfRowsFormat = "# of Rows: {0}";

        /// <summary>
        /// The save file extension
        /// </summary>
        private const string SaveFileExtension = ".ccs";

        /// <summary>
        /// The save version
        /// </summary>
        private const string SaveVersion = "1.4";

        /// <summary>
        /// The copy line option index
        /// </summary>
        private int copyLineOptionIndex = 0;

        /// <summary>
        /// The current request
        /// </summary>
        private string currentRequest = "";

        /// <summary>
        /// The default column priority
        /// </summary>
        private string defaultColumnPriority = "Number";

        /// <summary>
        /// The history
        /// </summary>
        private Dictionary<string, OldRequest> history = new Dictionary<string, OldRequest>();

        /// <summary>
        /// The history log
        /// </summary>
        private List<string> historyLog = new List<string>();

        /// <summary>
        /// Whether the program is currently saving.
        /// </summary>
        private Guard isSaving = new Guard();

        /// <summary>
        /// The request identifier
        /// </summary>
        private int requestID = 0;

        /// <summary>
        /// The save file
        /// </summary>
        private string saveFile = "";

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public OldMain()
        {
            InitializeComponent();
            
            string defaultName = Title;

            string fileToLoad = "";
            if (AssemblyExecutableName != ExecutableName)
            {
                var regularFile = $"{ExecutableName}{SaveFileExtension}";
                var compressedFile = $"{ExecutableName}{CompressedSaveFileExtension}";

                if (File.Exists(compressedFile))
                    fileToLoad = compressedFile;
                else // default to using a non-compressed file
                    fileToLoad = regularFile;
            }
            else
                fileToLoad = $"ColumnCopier-{DateTime.Now.Ticks}{SaveFileExtension}";

            while (!isSaving.CheckSet) ;
            var destinationPath = Path.Combine(ExecutableDirectory, "TEMPORARY");
            if (Directory.Exists(destinationPath))
                Directory.Delete(destinationPath, true);
            LoadSettings($"{ExecutableDirectory}\\{fileToLoad}");

            isSaving.Reset();
            CheckForUpdates();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Handles the CheckedChanged event of the header_cxb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. 
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void header_cxb_CheckedChanged(object sender, EventArgs e)
        {
            while (!isSaving.CheckSet) ;
            SaveSettings(saveFile);
        }

        /// <summary>
        /// Handles the Click event of the help_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void help_btn_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        /// <summary>
        /// Handles the TextChanged event of the line_txt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void line_txt_TextChanged(object sender, EventArgs e)
        {
            history[currentRequest].CurrentRowId = ParseTextToInt(line_txt.Text);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the removeBlanks_cxb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. 
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void removeBlanks_cxb_CheckedChanged(object sender, EventArgs e)
        {
            while (!isSaving.CheckSet) ;
            SaveSettings(saveFile);
        }

        /// <summary>
        /// Handles the Click event of the replaceSemiColon_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. 
        ///             - 1.1.4 (09-21-2016) - Added pre and post texts. Added status text.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void replaceSemiColon_btn_Click(object sender, EventArgs e)
        {
            if (isSaving.CheckSet)
            {
                ReplaceText = ";";
                ReplaceTextPost = "";
                ReplaceTextPre = "";
                SaveSettings(saveFile);

                StatusText = "Changed replacement text to [;]";
            }
            else
            {
                StatusText = BusySaveText;
            }
        }

        /// <summary>
        /// Setups the title.
        /// </summary>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void SetupTitle()
        {
            Title = $"Column Copier - {ExecutableVersion} - {Path.GetFileNameWithoutExtension(saveFile)}";
        }

        #endregion Private Methods

        #region Private Properties

        /// <summary>
        /// Gets the name of the assembly executable.
        /// </summary>
        /// <value>The name of the assembly executable.</value>
        private string AssemblyExecutableName
        {
            get { return this.ProductName; }
        }

        /// <summary>
        /// Gets or sets the clip board.
        /// </summary>
        /// <value>The clip board.</value>
        private string ClipBoard
        {
            get
            {
                return Clipboard.ContainsText()
                    ? Clipboard.GetText()
                    : "";
            }
            set
            {
                Clipboard.Clear();
                if (!string.IsNullOrEmpty(value))
                    Clipboard.SetText(value);
            }
        }

        /// <summary>
        /// Gets or sets the column text.
        /// </summary>
        /// <value>The column text.</value>
        private string ColumnText
        {
            get { return column_txt.Text; }
            set { column_txt.Text = value; }
        }

        /// <summary>
        /// Gets the index of the copy line option.
        /// </summary>
        /// <value>The index of the copy line option.</value>
        public int CopyLineOptionIndex
        {
            get { return copyLineOptionIndex; }
        }

        /// <summary>
        /// Gets or sets the current column text.
        /// </summary>
        /// <value>The current column text.</value>
        private string CurrentColumnText
        {
            get { return currentColumn_txt.Text; }
            set { currentColumn_txt.Text = value; }
        }

        /// <summary>
        /// Gets or sets the current line.
        /// </summary>
        /// <value>The current line.</value>
        private int CurrentLine
        {
            get { return ParseTextToInt(line_txt.Text); }
            set { line_txt.Text = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the default column.
        /// </summary>
        /// <value>The default column.</value>
        private int DefaultColumn
        {
            get { return ParseTextToInt(defaultColumn_txt.Text); }
            set { defaultColumn_txt.Text = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the default name of the column.
        /// </summary>
        /// <value>The default name of the column.</value>
        private string DefaultColumnName
        {
            get { return defaultColumnText_txt.Text; }
            set { defaultColumnText_txt.Text = value; }
        }

        /// <summary>
        /// Gets the executable directory.
        /// </summary>
        /// <value>The executable directory.</value>
        private string ExecutableDirectory
        {
            get
            {
                var dirName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

                return dirName.Remove(0, 6);
            }
        }

        /// <summary>
        /// Gets the name of the executable.
        /// </summary>
        /// <value>The name of the executable.</value>
        private string ExecutableName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase); }
        }

        /// <summary>
        /// Gets the executable version.
        /// </summary>
        /// <value>The executable version.</value>
        private string ExecutableVersion
        {
            get { return this.ProductVersion; }
        }

        /// <summary>
        /// Gets or sets the maximum history.
        /// </summary>
        /// <value>The maximum history.</value>
        private int MaxHistory
        {
            get { return ParseTextToInt(history_txt.Text); }
            set { history_txt.Text = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the number of columns text.
        /// </summary>
        /// <value>The number of columns text.</value>
        private string NumberOfColumnsText
        {
            get { return columnNum_txt.Text; }
            set { columnNum_txt.Text = value; }
        }

        /// <summary>
        /// Gets or sets the number of rows text.
        /// </summary>
        /// <value>The number of rows text.</value>
        private string NumberOfRowsText
        {
            get { return rowNum_txt.Text; }
            set { rowNum_txt.Text = value; }
        }

        /// <summary>
        /// Gets or sets the replace text.
        /// </summary>
        /// <value>The replace text.</value>
        private string ReplaceText
        {
            get { return replaceText_txt.Text; }
            set { replaceText_txt.Text = value; }
        }

        /// <summary>
        /// Gets or sets the post replace text.
        /// </summary>
        /// <value>The post replace text.</value>
        private string ReplaceTextPost
        {
            get { return replaceTextPost_txt.Text; }
            set { replaceTextPost_txt.Text = value; }
        }

        /// <summary>
        /// Gets or sets the pre replace text.
        /// </summary>
        /// <value>The pre replace text.</value>
        private string ReplaceTextPre
        {
            get { return replaceTextPre_txt.Text; }
            set { replaceTextPre_txt.Text = value; }
        }

        /// <summary>
        /// Gets or sets the status text.
        /// </summary>
        /// <value>The status text.</value>
        private string StatusText
        {
            get { return status_txt.Text; }
            set { status_txt.Text = value; }
        }

        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        /// <value>The threshold.</value>
        private int Threshold
        {
            get { return ParseTextToInt(threshold_txt.Text, 5); }
            set { threshold_txt.Text = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        private string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Checks for updates.
        /// </summary>
        /// <param name="client">The client.</param>
        ///  Changelog:
        ///             - 1.3.0 (05-30-2017) - Removed dependency on Octokit.
        ///             - 1.2.0 (09-30-2016) - Removed update text typo. Added actual functionality to go to the update.
        ///             - 1.0.0 (08-29-2016) - Initial version.
        public static void CheckForUpdates()
        {
            var latestRelease = GitHub.GitHub.GetLatestRelease();

            if (latestRelease != null)
            {
                var releaseVersion = ConvertReleaseTagVersionToInt(latestRelease.tag_name);

                if (releaseVersion > GitCurrentReleaseTagVersion)
                {
                    var result = MessageBox.Show($"A newly released version is available, version {latestRelease.tag_name}. Would you like to download the update?",
                                    "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            Process.Start(latestRelease.html_url);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Computes the similarity between two strings.
        /// </summary>
        /// <param name="s">The first string.</param>
        /// <param name="t">The second string.</param>
        /// <returns>The similarity of the strings.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
                return m;
            if (m == 0)
                return n;

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    // Step 3a
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 3b
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            // Step 4
            return d[n, m];
        }

        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <param name="text">The text.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Moved history cleaning to seperate method.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public void CreateRequest(string text)
        {
            var newRequest = new OldRequest(text, requestID++, header_cxb.Checked, removeBlanks_cxb.Checked);

            history.Add(newRequest.Name, newRequest);
            historyLog.Add(newRequest.Name);

            CleanHistory();

            LoadRequest(newRequest.Name);
        }

        /// <summary>
        /// Determines the index of the selected.
        /// </summary>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public int DetermineSelectedIndex()
        {
            switch (defaultColumnPriority)
            {
                case "Number":
                    if (DefaultColumn >= 0)
                        return DefaultColumn;
                    break;

                case "Name":
                    int currentClosestDistance = int.MaxValue;
                    int currentClosestID = int.MaxValue;

                    for (int i = 0; i < history[currentRequest].ColumnKeys.Count; i++)
                    {
                        int computedDistance = Compute(DefaultColumnName, history[currentRequest].ColumnKeys[i]);

                        if (computedDistance < currentClosestDistance)
                        {
                            currentClosestDistance = computedDistance;
                            currentClosestID = i;
                        }
                    }

                    if (currentClosestDistance <= Threshold)
                        return currentClosestID;

                    break;
            }

            if (DefaultColumn >= 0)
                return DefaultColumn;

            return 0;
        }

        /// <summary>
        /// Loads the column.
        /// </summary>
        /// <param name="columnNumber">The column number.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public void LoadColumn(int columnNumber)
        {
            ColumnText = history[currentRequest].GetColumnText(columnNumber);
            NumberOfColumnsText = string.Format(NumberOfColumnsFormat, history[currentRequest].NumberOfColumns);
            NumberOfRowsText = string.Format(NumberOfRowsFormat, history[currentRequest].CurrentColumnNumberOfRows);
            CurrentColumnText = string.Format(CurrentColumnFormat, history[currentRequest].CurrentColumnInteger);
        }

        /// <summary>
        /// Loads the request.
        /// </summary>
        /// <param name="name">The name.</param>
        ///  Changelog:
        ///             - 1.3.0 (05-30-2017) - Cleans column name text.
        ///             - 1.2.3 (01-23-2017) - Loading a request now updates the current line for copying lines.
        ///             - 1.2.0 (09-30-2016) - Added preserve request toggle support.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public void LoadRequest(string name)
        {
            try
            {
                currentRequest = name;
                ColumnText = "";

                column_cmb.Items.Clear();
                foreach (var newColumn in history[currentRequest].ColumnKeys.Values)
                {
                    var text = newColumn;
                    foreach (var pair in Constants.Instance.StringReplacements)
                        text = text.Replace(pair.Key, pair.Value);
                    column_cmb.Items.Add(text);
                }

                column_cmb.SelectedIndex = DetermineSelectedIndex();
                preserve_cxb.Checked = history[currentRequest].PreserveRequest;
                CurrentLine = history[currentRequest].CurrentRowId;
            }
            catch
            {

            }
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 1.3.0 (05-30-2017) - Loads the current column field properly now.
        ///             - 1.1.3 (08-30-2016) - Enhanced error message.
        ///             - 1.1.0 (08-29-2016) - Added error message.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public bool LoadSettings(string file)
        {
            ResetFields();
            saveFile = file;
            SetupTitle();

            try
            {
                if (!File.Exists(file))
                {
                    isSaving.Reset();
                    return false;
                }

                bool isCompressed = Path.GetExtension(file) == CompressedSaveFileExtension;
                if (isCompressed)
                {
                    var destinationPath = Path.Combine(Path.GetDirectoryName(file), "TEMPORARY");
                    System.IO.Compression.ZipFile.ExtractToDirectory(file, destinationPath);
                    file = Path.Combine(destinationPath, Path.GetFileName(file));
                }

                var doc = XDocument.Load(file);

                var currentColumn = "";
                foreach (var node in doc.Root.Elements())
                {
                    var nodeName = node.Name.ToString();
                    if (nodeName == "SaveVersion")
                    {
                        if (node.Value.ToString() != SaveVersion)
                            return false;
                    }
                    else if (nodeName == "Settings")
                    {
                        foreach (var settingNode in node.Elements())
                        {
                            switch (settingNode.Name.ToString())
                            {
                                case "ShowOnTop":
                                    isTop_cbx.Checked = ParseTextToBool(settingNode.Value.ToString(), false);
                                    break;

                                case "DataHasHeaders":
                                    header_cxb.Checked = ParseTextToBool(settingNode.Value.ToString(), false);
                                    break;

                                case "NextLine":
                                    line_txt.Text = settingNode.Value.ToString();
                                    break;

                                case "ReplaceTextOption":
                                    copyLineOptionIndex = ParseTextToInt(settingNode.Value.ToString(), 1);
                                    copyLineOptions_cmb.SelectedIndex = copyLineOptionIndex;
                                    break;

                                case "ReplaceText":
                                    ReplaceText = settingNode.Value.ToString();
                                    break;

                                case "ReplaceTextPost":
                                    ReplaceTextPost = settingNode.Value.ToString();
                                    break;

                                case "ReplaceTextPre":
                                    ReplaceTextPre = settingNode.Value.ToString();
                                    break;

                                case "DefaultColumn":
                                    defaultColumn_txt.Text = settingNode.Value.ToString();
                                    break;

                                case "DefaultColumnName":
                                    defaultColumnText_txt.Text = settingNode.Value.ToString();
                                    break;

                                case "Threshold":
                                    threshold_txt.Text = settingNode.Value.ToString();
                                    break;

                                case "Priority":
                                    switch (settingNode.Value.ToString())
                                    {
                                        case "Number":
                                            number_rbtn.Checked = true;
                                            text_rbtn.Checked = false;
                                            break;

                                        case "Name":
                                            number_rbtn.Checked = false;
                                            text_rbtn.Checked = true;
                                            break;
                                    }
                                    break;

                                case "MaxHistory":
                                    history_txt.Text = settingNode.Value.ToString();
                                    break;
                            }
                        }
                    }
                    else if (nodeName == "History")
                    {
                        foreach (var historyNode in node.Elements())
                        {
                            var historyName = historyNode.Name.ToString();

                            if (historyName == "CurrentRequest")
                            {
                                currentRequest = historyNode.Value.ToString();
                            }
                            else if (historyName == "CurrentColumn")
                            {
                                var text = historyNode.Value.ToString();
                                foreach (var pair in Constants.Instance.StringReplacements)
                                    text = text.Replace(pair.Key, pair.Value);
                                currentColumn = text;
                            }
                            else if (historyName == "Requests")
                            {
                                history.Clear();
                                foreach (var requestNode in historyNode.Elements())
                                {
                                    var request = new OldRequest(requestNode);
                                    history.Add(request.Name, request);
                                    historyLog.Add(request.Name);
                                }
                            }
                        }
                    }
                }

                ResetFields();
                saveFile = file;
                SetupTitle();

                var maxId = 0;
                foreach (var request in history)
                {
                    if (request.Value.ID > maxId)
                        maxId = request.Value.ID;
                }

                history_cmb.Items.Clear();
                foreach (var historyRequest in history.Keys)
                    history_cmb.Items.Add(historyRequest);
                history_cmb.SelectedIndex = history_cmb.Items.Count - 1;

                LoadRequest(currentRequest);

                var index = 0;
                try
                {
                    for (var i = 0; i < column_cmb.Items.Count; i++)
                    {
                        if (column_cmb.Items[i].ToString() == currentColumn)
                        {
                            index = i;
                            break;
                        }
                    }
                }
                catch { index = 0; }

                column_cmb.SelectedIndex = index;
                requestID = maxId + 1;

                if (isCompressed)
                    Directory.Delete(Path.GetDirectoryName(file), true);

                isSaving.Reset();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace.ToString(), $"Error: {ex.ToString()}", MessageBoxButtons.OK, MessageBoxIcon.Error);

                bool isCompressed = Path.GetExtension(file) == CompressedSaveFileExtension;
                if (isCompressed)
                {
                    var destinationPath = Path.Combine(Path.GetDirectoryName(file), "TEMPORARY");
                    Directory.Delete(destinationPath, true);
                }
            }

            isSaving.Reset();
            return false;
        }

        /// <summary>
        /// Parses the text to bool.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public static bool ParseTextToBool(string text, bool defaultValue = false)
        {
            bool output;
            if (bool.TryParse(text, out output))
                return output;
            return defaultValue;
        }

        /// <summary>
        /// Parses the text to int.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public static int ParseTextToInt(string text, int defaultValue = 0)
        {
            int output;
            if (int.TryParse(text, out output))
                return output;
            return defaultValue;
        }

        /// <summary>
        /// Replaces the new line in text.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 1.1.4 (09-21-2016) - Added pre and post replace text.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public string ReplaceNewLineInText()
        {
            var lines = history[currentRequest].GetColumnTextLines(column_cmb.SelectedIndex);

            if (lines.Count > 0)
            {
                StringBuilder str = new StringBuilder();
                str.Append(ReplaceTextPre);

                var finalLine = lines.Count - 1;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (i == finalLine)
                        str.AppendFormat("{0}", lines[i]);
                    else
                        str.AppendFormat("{0}{1}", lines[i], ReplaceText);
                }

                str.Append(ReplaceTextPost);
                return str.ToString();
            }

            return "";
        }

        /// <summary>
        /// Resets the fields.
        /// </summary>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public void ResetFields()
        {
            ColumnText = "";
            column_cmb.Items.Clear();

            NumberOfColumnsText = string.Format(NumberOfColumnsFormat, 0);
            NumberOfRowsText = string.Format(NumberOfRowsFormat, 0);
            CurrentColumnText = string.Format(CurrentColumnFormat, 0);
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. Added support for compressed saves.
        ///             - 1.1.3 (08-30-2016) - Removed string.format to new format approach, enhanced error message.
        ///             - 1.1.0 (08-29-2016) - Added error message.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public void SaveSettings(string file)
        {
            var saveThread = new Thread(() => SaveSettingsThead(file));

            saveThread.Start();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Cleans the history.
        /// </summary>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Initial version.
        private void CleanHistory()
        {

            var numberOfHistoryToDelete = historyLog.Count - MaxHistory;
            for (int i = 0; i < historyLog.Count; i++)
            {
                if (history[historyLog[i]].PreserveRequest)
                    numberOfHistoryToDelete--;
            }
            if (numberOfHistoryToDelete > 0)
            {
                var i = 0;
                for (var j = 0; j < numberOfHistoryToDelete; j++)
                {
                    var request = historyLog[i];
                    if (!history[request].PreserveRequest)
                    {
                        history.Remove(request);
                        historyLog.RemoveAt(i);

                        i--;
                    }
                    else
                    {
                        j--;
                    }
                    i++;
                }
            }

            history_cmb.Items.Clear();
            for (var i = 0; i < historyLog.Count; i++)
                history_cmb.Items.Add(historyLog[i]);
            history_cmb.SelectedIndex = history_cmb.Items.Count - 1;

            if (history_cmb.Items.Count == 0)
            {
                column_cmb.Items.Add("");
                column_cmb.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the Click event of the clearHistory_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. 
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void clearHistory_btn_Click(object sender, EventArgs e)
        {
            if (!isSaving.CheckSet)
            {
                history.Clear();
                history_cmb.Items.Clear();

                ResetFields();

                SaveSettings(saveFile);

                StatusText = "Cleared History!";
            }
            else
                StatusText = BusySaveText;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the column_cmb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. 
        ///             - 1.1.4 (09-21-2016) - Updated status text to display more information.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void column_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadColumn(column_cmb.SelectedIndex);
            if (isSaving.CheckSet)
            {
                SaveSettings(saveFile);
            }

            StatusText = $"Changed selected column to [{column_cmb.Items[column_cmb.SelectedIndex]}]!";
        }

        /// <summary>
        /// Converts the release tag version to int.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-29-2016) - Initial version.
        private static int ConvertReleaseTagVersionToInt(string tag)
        {
            var tagBits = tag.Split('.');
            var str = new StringBuilder();
            foreach (var bit in tagBits)
                str.Append(bit);

            return ParseTextToInt(str.ToString(), -1);
        }

        /// <summary>
        /// Handles the Click event of the copy_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.1.5 (09-21-2016) - Updated status text to display more information.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void copy_btn_Click(object sender, EventArgs e)
        {
            ClipBoard = ColumnText;
            column_txt.Focus();
            column_txt.SelectAll();

            StatusText = $"Copied selected column [{column_cmb.Items[column_cmb.SelectedIndex]}]!";
        }

        /// <summary>
        /// Handles the Click event of the copyLine_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.1.4 (09-21-2016) - Updated status text to display more information.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void copyLine_btn_Click(object sender, EventArgs e)
        {
            var line = history[currentRequest].GetNextLine(CurrentLine);
            ClipBoard = line;
            CurrentLine = history[currentRequest].CurrentRowId;

            StatusText = $"Copied line! Text: [{line}]";
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the copyLineOptions_cmb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.3.0 (05-30-2017) - Initial version.
        private void copyLineOptions_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            copyLineOptionIndex = copyLineOptions_cmb.SelectedIndex;
            var selection = copyLineOptions_cmb.Items[copyLineOptionIndex].ToString();

            var statusTextFormat = "Changed replacement text to ";
            var statusTextPostfix = "";
            switch (selection)
            {
                case "":
                    ReplaceText = "";
                    ReplaceTextPost = "";
                    ReplaceTextPre = "";
                    statusTextPostfix = "no seperator.";
                    break;
                case ",":
                    ReplaceText = ", ";
                    ReplaceTextPost = "";
                    ReplaceTextPre = "";
                    statusTextPostfix = "[,]";
                    break;
                case "\",\"":
                    ReplaceText = "\", \"";
                    ReplaceTextPost = "\"";
                    ReplaceTextPre = "\"";
                    statusTextPostfix = "a [\", \"], with a [\"] at the beginning and end of the result string.";
                    break;
                case "( , )":
                    ReplaceText = ", ";
                    ReplaceTextPost = ")";
                    ReplaceTextPre = "(";
                    statusTextPostfix = "a [, ] character, with a ( at the beginning and a ) at the end of the resulting string.";
                    break;
                case "(' ', ' ')":
                    ReplaceText = "', '";
                    ReplaceTextPost = "')";
                    ReplaceTextPre = "('";
                    statusTextPostfix = "a [', '] character, with a (' at the beginning and a ') at the end of the resulting string.";
                    break;
                case "(\" \", \" \")":
                    ReplaceText = "\", \"";
                    ReplaceTextPost = "\")";
                    ReplaceTextPre = "(\"";
                    statusTextPostfix = "a [\", \"] character, with a (\" at the beginning and a \") at the end of the resulting string.";
                    break;
                case ";":
                    ReplaceText = ";";
                    ReplaceTextPost = "";
                    ReplaceTextPre = "";
                    statusTextPostfix = "[;]";
                    break;
            }

            SaveSettings(saveFile);
            StatusText = $"{statusTextFormat}{statusTextPostfix}";
        }

        /// <summary>
        /// Handles the Click event of the copyReplace_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.1.4 (09-21-2016) - Updated status text to display more information.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void copyReplace_btn_Click(object sender, EventArgs e)
        {
            ClipBoard = ReplaceNewLineInText();
            column_txt.Focus();
            column_txt.SelectAll();

            StatusText = "Copied selected column and replaced line breaks!";
        }

        /// <summary>
        /// Handles the Click event of the deleteRequest_txt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Initial version.
        private void deleteRequest_txt_Click(object sender, EventArgs e)
        {
            if (history_cmb.Items.Count > 0)
            {
                if (isSaving.CheckSet)
                {
                    try
                    {
                        var request = history_cmb.Text;
                        historyLog.Remove(request);
                        history.Remove(request);
                        ColumnText = "";
                        column_cmb.Items.Clear();

                        history_cmb.SelectedIndex = history_cmb.Items.Count - 2;
                        CleanHistory();
                        SaveSettings(saveFile);

                        StatusText = $"Deleted the current request [{request}]!";
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        isSaving.Reset();
                    }
                }
                else
                    StatusText = BusySaveText;
            }
            else
            {
                StatusText = "No more requests to delete!";
            }
        }

        /// <summary>
        /// Handles the Click event of the export_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.1.5 (09-21-2016) - Initial version.
        private void export_btn_Click(object sender, EventArgs e)
        {
            ClipBoard = history[currentRequest].Export();

            StatusText = $"Exported the current request [{history_cmb.Text}]!";
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the history_cmb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. 
        ///             - 1.1.4 (09-21-2016) - Updated status text to display more information.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void history_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequest(history_cmb.Text);
            if (isSaving.CheckSet)
            {
                SaveSettings(saveFile);
            }

            StatusText = $"Changed displayed request to [{history_cmb.Text}]!";
        }

        /// <summary>
        /// Handles the CheckedChanged event of the isTop_cbx control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. 
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void isTop_cbx_CheckedChanged(object sender, EventArgs e)
        {
            if (isTop_cbx.Checked)
                this.TopMost = true;
            else
                this.TopMost = false;
            while (!isSaving.CheckSet) ;

            SaveSettings(saveFile);
        }

        /// <summary>
        /// Handles the Click event of the loadSettings_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Added compressed save and threading support.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void loadSettings_btn_Click(object sender, EventArgs e)
        {
            if (isSaving.CheckSet)
            {
                OpenFileDialog fileSelector = new OpenFileDialog();
                fileSelector.DefaultExt = SaveFileExtension;
                fileSelector.Filter = $"Column Copier Save File ({SaveFileExtension})|*{SaveFileExtension}|Compressed Column Copier Save File ({CompressedSaveFileExtension})|*{CompressedSaveFileExtension}";
                fileSelector.InitialDirectory = ExecutableDirectory;

                fileSelector.ShowDialog();
                var file = fileSelector.FileName;

                if (!string.IsNullOrWhiteSpace(file))
                {
                    LoadSettings(file);

                    StatusText = "Loaded settings file!";
                }
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the number_rbtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. 
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void number_rbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (number_rbtn.Checked)
                defaultColumnPriority = "Number";

            while (!isSaving.CheckSet) ;
            SaveSettings(saveFile);
        }

        /// <summary>
        /// Handles the Click event of the paste_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. Added preserve request toggle support.
        ///             - 1.1.6 (09-29-2016) - Added check for an empty clipboard.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void paste_btn_Click(object sender, EventArgs e)
        {
            if (isSaving.CheckSet)
            {
                var text = VerifyText(ClipBoard);
                if (text != null)
                {
                    CreateRequest(text);
                    SaveSettings(saveFile);
                    preserve_cxb.Checked = false;

                    StatusText = "Pasted data!";
                }
                else
                {
                    StatusText = "No data in the clipboard!";
                }
            }
            else
            {
                StatusText = BusySaveText;
            }
        }

        /// <summary>
        /// Handles the Click event of the pasteCopy_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. Added preserve request toggle support.
        ///             - 1.1.6 (09-29-2016) - Added check for an empty clipboard.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void pasteCopy_btn_Click(object sender, EventArgs e)
        {
            if (isSaving.CheckSet)
            {
                var text = VerifyText(ClipBoard);
                if (text != null)
                {
                    CreateRequest(text);
                    column_txt.Focus();
                    column_txt.SelectAll();
                    ClipBoard = ColumnText;
                    SaveSettings(saveFile);
                    preserve_cxb.Checked = false;

                    StatusText = "Pasted data and copied the default selected column!";
                }
                else
                {
                    StatusText = "No data in the clipboard!";
                    }
                }
            else
            {
                StatusText = "Currently processing another request, please try again!";
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the preserve_cxb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.1 (12-27-2016) - Saves the settings upon the toggle changing and displays information that the request preservation status has changed.
        ///             - 1.2.0 (09-30-2016) - Initial version.
        private void preserve_cxb_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(currentRequest))
            {
                history[currentRequest].PreserveRequest = preserve_cxb.Checked;
                SaveSettings(saveFile);

                StatusText = $"Preservation status on request {currentRequest} has changed to: {preserve_cxb.Checked}";
            }
        }

        /// <summary>
        /// Handles the Click event of the saveAsNew_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. Added support for compressed saves.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void saveAsNew_btn_Click(object sender, EventArgs e)
        {
            if (isSaving.CheckSet)
            {
                SaveFileDialog fileSelector = new SaveFileDialog();
                fileSelector.DefaultExt = SaveFileExtension;
                fileSelector.Filter = $"Column Copier Save File ({SaveFileExtension})|*{SaveFileExtension}|Compressed Column Copier Save File ({CompressedSaveFileExtension})|*{CompressedSaveFileExtension}";
                fileSelector.InitialDirectory = ExecutableDirectory;

                fileSelector.ShowDialog();
                var file = fileSelector.FileName;

                if (!string.IsNullOrWhiteSpace(file))
                {
                    SaveSettings(file);
                    LoadSettings(file);

                    StatusText = "Saved as new file!";
                }
            }
            else
            {
                StatusText = BusySaveText;
            }
        }

        /// <summary>
        /// Saves the settings thead.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 1.3.0 (05-30-2017) - Now remembers pre and post text.
        ///             - 1.2.2 (12-27-2016) - Reset isSaving toggle to allow tool to continue to be used on failure of a save.
        ///             - 1.2.0 (09-30-2016) - Initial version.
        private bool SaveSettingsThead(string file)
        {
            try
            {
                ToggleProgressBar();
                if (File.Exists(file))
                    File.Delete(file);

                var str = new StringBuilder();

                str.AppendLine("<ColumnCopier>");

                str.AppendLine($"<SaveVersion>{SaveVersion}</SaveVersion>");
                str.AppendLine("<Settings>");
                str.AppendLine($"<ShowOnTop>{isTop_cbx.Checked}</ShowOnTop>");
                str.AppendLine($"<DataHasHeaders>{header_cxb.Checked}</DataHasHeaders>");
                str.AppendLine($"<NextLine>{line_txt.Text}</NextLine>");
                str.AppendLine($"<ReplaceTextOption>{CopyLineOptionIndex}</ReplaceTextOption>");
                str.AppendLine($"<ReplaceText>{ReplaceText}</ReplaceText>");
                str.AppendLine($"<ReplaceTextPre>{ReplaceTextPre}</ReplaceTextPre>");
                str.AppendLine($"<ReplaceTextPost>{ReplaceTextPost}</ReplaceTextPost>");
                str.AppendLine($"<DefaultColumn>{DefaultColumn}</DefaultColumn>");
                str.AppendLine($"<DefaultColumnName>{DefaultColumnName}</DefaultColumnName>");
                str.AppendLine($"<Threshold>{Threshold}</Threshold>");
                str.AppendLine($"<Priority>{defaultColumnPriority}</Priority>");
                str.AppendLine($"<MaxHistory>{MaxHistory}</MaxHistory>");
                str.AppendLine("</Settings>");

                str.AppendLine("<History>");
                if (!string.IsNullOrEmpty(currentRequest))
                {
                    str.AppendLine($"<CurrentRequest>{currentRequest}</CurrentRequest>");
                    str.AppendLine($"<CurrentColumn>{OldRequest.CleanText(history[currentRequest].CurrentColumn)}</CurrentColumn>");
                    str.AppendLine("<Requests>");
                    foreach (var request in history)
                        str.AppendLine(request.Value.ToXmlText());
                    str.AppendLine("</Requests>");
                }
                str.AppendLine("</History>");

                str.AppendLine("</ColumnCopier>");

                var result = str.ToString();
                var doc = XDocument.Parse(result);
                doc.Save(file);
                doc = null;

                bool isCompressed = Path.GetExtension(file) == CompressedSaveFileExtension;
                if (isCompressed)
                {
                    var destinationPath = Path.Combine(Path.GetDirectoryName(file), "TEMPORARY");
                    Directory.CreateDirectory(destinationPath);
                    File.Move(file, Path.Combine(destinationPath, Path.GetFileName(file)));
                    System.IO.Compression.ZipFile.CreateFromDirectory(destinationPath, file);
                    Directory.Delete(destinationPath, true);
                }

                isSaving.Reset();
                ToggleProgressBar();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace.ToString(), $"Error: {ex.ToString()}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isSaving.Reset();
                ToggleProgressBar();
            }

            return false;
        }

        /// <summary>
        /// Handles the Click event of the saveSettings_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. Added support for compressed saves.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void saveSettings_btn_Click(object sender, EventArgs e)
        {
            if (isSaving.CheckSet)
            {
                SaveSettings(saveFile);

                StatusText = "Saved file!";
            }
            else
            {
                StatusText = BusySaveText;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the text_rbtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Saves now occur on a separate thread. 
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void text_rbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (text_rbtn.Checked)
                defaultColumnPriority = "Name";

            while (!isSaving.CheckSet) ;
            SaveSettings(saveFile);
        }

        /// <summary>
        /// Toggles the progress bar.
        /// </summary>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Initial version.
        private void ToggleProgressBar()
        {
            if (progress_bar.InvokeRequired)
            {
                UpdateProgressBar d = new UpdateProgressBar(ToggleProgressBar);
                this.Invoke(d);
            }
            else
            {
                progress_bar.Visible = !progress_bar.Visible;
            }
        }

        /// <summary>
        /// Verifies that text will be valid in the system.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns><c>true</c> if the text is valid, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 1.1.6 (09-19-2016) - Initial version.
        private string VerifyText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            StringBuilder output = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                var tmp = text[i];
                if (XmlConvert.IsXmlChar(tmp))
                    output.Append(tmp);
            }
            return output.ToString();
        }

        #endregion Private Methods
     }
}