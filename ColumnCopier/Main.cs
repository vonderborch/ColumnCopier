// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Main.cs
// Author           : Christian
// Created          : 08-15-2016
// 
// Version          : 1.0.0
// Last Modified By : Christian
// Last Modified On : 08-29-2016
// ***********************************************************************
// <copyright file="Main.cs" company="Christian Webber">
//		Copyright ©  2016
// </copyright>
// <summary>
//      The Main class.
// </summary>
//
// Changelog: 
//            - 1.1.0 (08-29-2016) - Added status text field, update checker, and error message popups.
//            - 1.0.0 (08-22-2016) - Initial version finished.
//            - 0.5.0 (08-18-2016) - Initial version created.
//            - 0.0.0 (08-15-2016) - Initial version created.
// ***********************************************************************
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ColumnCopier
{
    /// <summary>
    /// Main class.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class Main : Form
    {
        #region Private Fields

        /// <summary>
        /// The current column format
        /// </summary>
        private const string CurrentColumnFormat = "Current Column #: {0}";

        /// <summary>
        /// The git client
        /// </summary>
        private GitHubClient gitClient;

        /// <summary>
        /// The git current release tag
        /// </summary>
        private const int GitCurrentReleaseTagVersion = 111;

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
        private const string SaveVersion = "1.0";

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
        private Dictionary<string, Request> history = new Dictionary<string, Request>();

        /// <summary>
        /// The history log
        /// </summary>
        private Queue<string> historyLog = new Queue<string>();

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
        public Main()
        {
            InitializeComponent();


            string defaultName = Title;

            string fileToLoad = "";
            if (AssemblyExecutableName != ExecutableName)
                fileToLoad = $"{ExecutableName}{SaveFileExtension}";
            else
                fileToLoad = $"ColumnCopier-{DateTime.Now.Ticks}{SaveFileExtension}";

            LoadSettings($"{ExecutableDirectory}\\{fileToLoad}");

            gitClient = new GitHubClient(new ProductHeaderValue($"{AssemblyExecutableName}_Application"));
            CheckForUpdates(gitClient);
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Handles the CheckedChanged event of the header_cxb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void header_cxb_CheckedChanged(object sender, EventArgs e)
        {
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
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void removeBlanks_cxb_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings(saveFile);
        }

        /// <summary>
        /// Handles the Click event of the replaceSemiColon_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void replaceSemiColon_btn_Click(object sender, EventArgs e)
        {
            ReplaceText = ";";
            SaveSettings(saveFile);
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
        ///             - 1.0.0 (08-29-2016) - Initial version.
        public static async void CheckForUpdates(GitHubClient client)
        {
            var latestRelease = await client.Repository.Release.GetLatest(GitUser, GitRepository);

            var releaseVersion = ConvertReleaseTagVersionToInt(latestRelease.TagName);

            if (releaseVersion > GitCurrentReleaseTagVersion)
                MessageBox.Show($"A newly released version is available, version ${latestRelease.TagName}. Would you like to download the update?",
                                "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Computes the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="t">The t.</param>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <param name="text">The text.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public void CreateRequest(string text)
        {
            var newRequest = new Request(text, requestID++, header_cxb.Checked, removeBlanks_cxb.Checked);

            history.Add(newRequest.Name, newRequest);
            historyLog.Enqueue(newRequest.Name);

            var numberOfHistoryToDelete = historyLog.Count - MaxHistory;
            if (numberOfHistoryToDelete > 0)
            {
                for (int i = 0; i < numberOfHistoryToDelete; i++)
                    history.Remove(historyLog.Dequeue());
            }

            history_cmb.Items.Clear();
            foreach (var historyRequest in history.Keys)
                history_cmb.Items.Add(historyRequest);
            history_cmb.SelectedIndex = history_cmb.Items.Count - 1;

            LoadRequest(newRequest.Name);
            SaveSettings(saveFile);
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
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public void LoadRequest(string name)
        {
            currentRequest = name;
            ColumnText = "";

            column_cmb.Items.Clear();
            foreach (var newColumn in history[currentRequest].ColumnKeys.Values)
                column_cmb.Items.Add(newColumn);

            column_cmb.SelectedIndex = DetermineSelectedIndex();
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public bool LoadSettings(string file)
        {
            ResetFields();
            saveFile = file;
            SetupTitle();

            try
            {
                if (!File.Exists(file))
                    return false;

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

                                case "ReplaceText":
                                    ReplaceText = settingNode.Value.ToString();
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
                                currentColumn = historyNode.Value.ToString();
                            }
                            else if (historyName == "Requests")
                            {
                                history.Clear();
                                foreach (var requestNode in historyNode.Elements())
                                {
                                    Request request = new Request(requestNode);
                                    history.Add(request.Name, request);
                                    historyLog.Enqueue(request.Name);
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
                requestID = maxId++;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), $"Error: {ex.ToString()}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /// <summary>
        /// Replaces the new line in text.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public string ReplaceNewLineInText()
        {
            var lines = history[currentRequest].GetColumnTextLines(column_cmb.SelectedIndex);

            if (lines.Count > 0)
            {
                StringBuilder str = new StringBuilder();

                var finalLine = lines.Count - 1;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (i == finalLine)
                        str.AppendFormat("{0}", lines[i]);
                    else
                        str.AppendFormat("{0}{1}", lines[i], ReplaceText);
                }

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
        ///             - 1.0.0 (08-15-2016) - Initial version.
        public bool SaveSettings(string file)
        {
            try
            {
                if (File.Exists(file))
                    File.Delete(file);

                var str = new StringBuilder();

                str.AppendLine("<ColumnCopier>");

                str.AppendLine(string.Format("<SaveVersion>{0}</SaveVersion>", SaveVersion));
                str.AppendLine("<Settings>");
                str.AppendLine(string.Format("<ShowOnTop>{0}</ShowOnTop>", isTop_cbx.Checked));
                str.AppendLine(string.Format("<DataHasHeaders>{0}</DataHasHeaders>", header_cxb.Checked));
                str.AppendLine(string.Format("<NextLine>{0}</NextLine>", line_txt.Text));
                str.AppendLine(string.Format("<ReplaceText>{0}</ReplaceText>", ReplaceText));
                str.AppendLine(string.Format("<DefaultColumn>{0}</DefaultColumn>", DefaultColumn));
                str.AppendLine(string.Format("<DefaultColumnName>{0}</DefaultColumnName>", DefaultColumnName));
                str.AppendLine(string.Format("<Threshold>{0}</Threshold>", Threshold));
                str.AppendLine(string.Format("<Priority>{0}</Priority>", defaultColumnPriority));
                str.AppendLine(string.Format("<MaxHistory>{0}</MaxHistory>", MaxHistory));
                str.AppendLine("</Settings>");

                str.AppendLine("<History>");
                if (!string.IsNullOrEmpty(currentRequest))
                {
                    str.AppendLine(string.Format("<CurrentRequest>{0}</CurrentRequest>", currentRequest));
                    str.AppendLine(string.Format("<CurrentColumn>{0}</CurrentColumn>", history[currentRequest].CurrentColumn));
                    str.AppendLine("<Requests>");
                    foreach (var request in history)
                        str.AppendLine(request.Value.ToXmlText());
                    str.AppendLine("</Requests>");
                }
                str.AppendLine("</History>");

                str.AppendLine("</ColumnCopier>");

                var doc = XDocument.Parse(str.ToString());
                doc.Save(file);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), $"Error: {ex.ToString()}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        #endregion Public Methods

        #region Private Methods

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
        /// Handles the Click event of the clearHistory_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void clearHistory_btn_Click(object sender, EventArgs e)
        {
            history.Clear();
            history_cmb.Items.Clear();

            ResetFields();

            SaveSettings(saveFile);

            StatusText = "Cleared History!";
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the column_cmb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void column_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadColumn(column_cmb.SelectedIndex);
            SaveSettings(saveFile);

            StatusText = "Changed selected column!";
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
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void copy_btn_Click(object sender, EventArgs e)
        {
            ClipBoard = ColumnText;
            column_txt.Focus();
            column_txt.SelectAll();

            StatusText = "Copied selected column!";
        }

        /// <summary>
        /// Handles the Click event of the copyLine_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void copyLine_btn_Click(object sender, EventArgs e)
        {
            ClipBoard = history[currentRequest].GetNextLine(CurrentLine);
            CurrentLine = history[currentRequest].CurrentRowId;

            StatusText = "Copied line!";
        }

        /// <summary>
        /// Handles the Click event of the copyReplace_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void copyReplace_btn_Click(object sender, EventArgs e)
        {
            ClipBoard = ReplaceNewLineInText();
            column_txt.Focus();
            column_txt.SelectAll();

            StatusText = "Copied selected column and replaced lines!";
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the history_cmb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void history_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequest(history_cmb.Text);
            SaveSettings(saveFile);

            StatusText = "Loaded new request!";
        }

        /// <summary>
        /// Handles the CheckedChanged event of the isTop_cbx control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void isTop_cbx_CheckedChanged(object sender, EventArgs e)
        {
            if (isTop_cbx.Checked)
                this.TopMost = true;
            else
                this.TopMost = false;

            SaveSettings(saveFile);
        }

        /// <summary>
        /// Handles the Click event of the loadSettings_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void loadSettings_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileSelector = new OpenFileDialog();
            fileSelector.DefaultExt = SaveFileExtension;
            fileSelector.Filter = $"Column Copier Save File ({SaveFileExtension})|*{SaveFileExtension}";
            fileSelector.InitialDirectory = ExecutableDirectory;

            fileSelector.ShowDialog();
            var file = fileSelector.FileName;

            LoadSettings(file);

            StatusText = "Loaded settings file!";
        }

        /// <summary>
        /// Handles the CheckedChanged event of the number_rbtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void number_rbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (number_rbtn.Checked)
                defaultColumnPriority = "Number";

            SaveSettings(saveFile);
        }

        /// <summary>
        /// Handles the Click event of the paste_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void paste_btn_Click(object sender, EventArgs e)
        {
            CreateRequest(ClipBoard);
            SaveSettings(saveFile);

            StatusText = "Pasted data!";
        }

        /// <summary>
        /// Handles the Click event of the pasteCopy_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void pasteCopy_btn_Click(object sender, EventArgs e)
        {
            CreateRequest(ClipBoard);
            column_txt.Focus();
            column_txt.SelectAll();
            ClipBoard = ColumnText;
            SaveSettings(saveFile);

            StatusText = "Pasted data and copied the selected column!";
        }

        /// <summary>
        /// Handles the Click event of the replaceComma_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void replaceComma_btn_Click(object sender, EventArgs e)
        {
            ReplaceText = ", ";
            SaveSettings(saveFile);
        }

        /// <summary>
        /// Handles the Click event of the saveAsNew_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void saveAsNew_btn_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileSelector = new SaveFileDialog();
            fileSelector.DefaultExt = SaveFileExtension;
            fileSelector.Filter = $"Column Copier Save File ({SaveFileExtension})|*{SaveFileExtension}";
            fileSelector.InitialDirectory = ExecutableDirectory;

            fileSelector.ShowDialog();
            var file = fileSelector.FileName;

            SaveSettings(file);
            LoadSettings(file);

            StatusText = "Saved as new file!";
        }

        /// <summary>
        /// Handles the Click event of the saveSettings_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void saveSettings_btn_Click(object sender, EventArgs e)
        {
            SaveSettings(saveFile);

            StatusText = "Saved file!";
        }

        /// <summary>
        /// Handles the CheckedChanged event of the text_rbtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        private void text_rbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (text_rbtn.Checked)
                defaultColumnPriority = "Name";

            SaveSettings(saveFile);
        }

        #endregion Private Methods
    }
}