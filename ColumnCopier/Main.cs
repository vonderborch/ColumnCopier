﻿// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Main.cs
// Author           : Christian
// Created          : 08-15-2016
//
// Version          : 2.2.1
// Last Modified By : Christian
// Last Modified On : 08-07-2017
// ***********************************************************************
// <copyright file="Main.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Main class.
// </summary>
//
// Changelog:
//            - 2.2.1 (08-07-2017) - Loads line separator options into the GUI, updates program title with filename.
//            - 2.2.0 (07-14-2017) - SQL Input Wizard and multiple column copying.
//            - 2.2.0 (07-13-2017) - SQL input support.
//            - 2.1.0 (06-07-2017) - New save system. Fixed auto-scaling. Fixed state loading not changing to correct request. Fixed copy next line bug. Improved error handling.
//            - 2.0.0 (06-06-2017) - Rebuilt!
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
using ColumnCopier.Classes;
using ColumnCopier.Enums;
using ColumnCopier.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

/// <summary>
/// The ColumnCopier namespace.
/// </summary>
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
        /// The cc state
        /// </summary>
        private ColumnCopierState ccState;

        /// <summary>
        /// The check guard
        /// </summary>
        private Guard checkGuard;

        /// <summary>
        /// The paste guard
        /// </summary>
        private Guard pasteGuard;

        /// <summary>
        /// The save guard
        /// </summary>
        private Guard saveGuard;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            InitializeComponent();

            UpdateStatusText($"Welcome!{Environment.NewLine}");

            ccState = new ColumnCopierState();
            checkGuard = new Guard();
            saveGuard = new Guard();
            pasteGuard = new Guard();

            string defaultName = this.Text;

            string fileToLoad = "";
            if (AssemblyExecutableName != ExecutableName)
            {
                var regularFile = $"{ExecutableName}{Constants.Instance.SaveExtension}";
                var compressedFile = $"{ExecutableName}{Constants.Instance.SaveExtensionCompressed}";

                if (File.Exists(compressedFile))
                    fileToLoad = compressedFile;
                else // default to using a non-compressed file
                    fileToLoad = regularFile;
            }
            else
                fileToLoad = $"ColumnCopier-{DateTime.Now.Ticks}{Constants.Instance.SaveExtension}";

            //while (!isSaving.CheckSet) ;
            var destinationPath = Path.Combine(ExecutableDirectory, "TEMPORARY");
            if (Directory.Exists(destinationPath))
                Directory.Delete(destinationPath, true);
            ccState.SaveFile = $"{ExecutableDirectory}\\{fileToLoad}";
            StateOpen();
            //LoadSettings($"{ExecutableDirectory}\\{fileToLoad}");
        }

        #endregion Public Constructors

        #region Private Delegates

        /// <summary>
        /// Delegate ChangeFormOpacityDelegate
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="value">The value.</param>
        private delegate void ChangeFormOpacityDelegate(Form form, int value);

        /// <summary>
        /// Delegate UpdateCheckBoxDelegate
        /// </summary>
        /// <param name="checkbox">The checkbox.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private delegate void UpdateCheckBoxDelegate(CheckBox checkbox, bool value);

        /// <summary>
        /// Delegate UpdateComboBoxIndexDelegate
        /// </summary>
        /// <param name="combobox">The combobox.</param>
        /// <param name="index">The index.</param>
        private delegate void UpdateComboBoxIndexDelegate(ComboBox combobox, int index);

        /// <summary>
        /// Delegate UpdateComboBoxItemsDelegate
        /// </summary>
        /// <param name="comboBox">The combo box.</param>
        /// <param name="values">The values.</param>
        private delegate void UpdateComboBoxItemsDelegate(ComboBox comboBox, List<string> values);

        /// <summary>
        /// Delegate UpdateComboBoxTextDelegate
        /// </summary>
        /// <param name="combobox">The combobox.</param>
        /// <param name="text">The text.</param>
        private delegate void UpdateComboBoxTextDelegate(ComboBox combobox, string text);

        /// <summary>
        /// Delegate UpdateLabelDelegate
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="text">The text.</param>
        private delegate void UpdateLabelDelegate(Label label, string text);

        /// <summary>
        /// Delegate UpdateMenuItemCheckedDelegate
        /// </summary>
        /// <param name="menuitem">The menuitem.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private delegate void UpdateMenuItemCheckedDelegate(ToolStripMenuItem menuitem, bool value);

        /// <summary>
        /// Delegate UpdateProgramTitleDelegate
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="text">The text.</param>
        private delegate void UpdateProgramTitleDelegate(Form form, string text);

        /// <summary>
        /// Delegate UpdateProgressBar
        /// </summary>
        private delegate void UpdateProgressBar();

        /// <summary>
        /// Delegate UpdateRadioButtonDelegate
        /// </summary>
        /// <param name="radiobutton">The radiobutton.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private delegate void UpdateRadioButtonDelegate(RadioButton radiobutton, bool value);

        /// <summary>
        /// Delegate UpdateTextBoxDelegate
        /// </summary>
        /// <param name="textbox">The textbox.</param>
        /// <param name="text">The text.</param>
        private delegate void UpdateTextBoxDelegate(TextBox textbox, string text);

        #endregion Private Delegates

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

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Adds the input.
        /// </summary>
        /// <param name="newInput">The new input.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-13-2017) - Initial version.
        public void AddInput(string newInput)
        {
            try
            {
                UpdateStatusText("Adding input...");
                if (pasteGuard.CheckSet)
                {
                    var lastPreservationState = preserveCurrentRequest_cxb.Checked;

                    UpdateCheckBox(preserveCurrentRequest_cxb, false);
                    UpdateMenuItemChecked(historySettingsPreserveCurrentRequest_itm, false);
                    ccState.SetCurrentRequestPreservationToggle(lastPreservationState);

                    ccState.MaxHistory = Converters.ConvertToIntWithClamp(maxHistory_txt.Text, 0, 0);
                    ccState.DefaultColumnIndex = Converters.ConvertToInt(defaultColumnNumber_txt.Text, 0);
                    ccState.DefaultColumnName = defaultColumnName_txt.Text;
                    ccState.DefaultColumnNameMatch = Converters.ConvertToIntWithClamp(defaultPriorityNameSimilarity_txt.Text, 0, 0);
                    if (defaultPriorityName_rbn.Checked)
                        ccState.DefaultColumnPriority = DefaultColumnPriority.Name;
                    else
                        ccState.DefaultColumnPriority = DefaultColumnPriority.Number;

                    ccState.AddNewRequest(newInput);

                    UpdateRequestHistory();
                    UpdateComboBoxIndex(requestHistory_cmb, 0);
                    pasteGuard.Reset();
                    UpdateStatusText("Input added!");
                }
                else
                {
                    UpdateStatusText("Busy, please try again...");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Changes the column.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void ChangeColumn(int columnIndex)
        {
            try
            {
                UpdateStatusText("Changing column...");
                ccState.History[ccState.CurrentRequest].SetCurrentColumn(columnIndex);
                UpdateTextBox(currentColumnText_txt, ccState.History[ccState.CurrentRequest].GetCurrentColumnText());

                UpdateLabelText(statCurrentColumn_txt, string.Format(Constants.Instance.FormatStatCurrentColumn,
                    ccState.History[ccState.CurrentRequest].CurrentColumnIndex + 1,
                    ccState.History[ccState.CurrentRequest].CurrentColumnIndex));
                UpdateLabelText(statNumberColumns_txt, string.Format(Constants.Instance.FormatStatNumberColumns,
                    ccState.History[ccState.CurrentRequest].NumberOfColumns));
                UpdateLabelText(statNumberRows_txt, string.Format(Constants.Instance.FormatStatNumberRows,
                    ccState.History[ccState.CurrentRequest].GetColumnRawText().Count));

                UpdateTextBox(copyLineNumber_txt, ccState.History[ccState.CurrentRequest].CopyNextLineIndex.ToString());
                UpdateStatusText("Column changed!");
                StateSave();
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Changes the opacity.
        /// </summary>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void ChangeOpacity()
        {
            ChangeFormOpacity(this, ccState.ProgramOpacity);
        }

        /// <summary>
        /// Changes the request.
        /// </summary>
        /// <param name="requestIndex">Index of the request.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void ChangeRequest(int requestIndex)
        {
            try
            {
                UpdateStatusText("Changing request...");
                ccState.CurrentRequest = ccState.HistoryLog[ccState.GetRequestHistoryPosition(requestHistory_cmb.Items[requestIndex].ToString())];

                UpdateComboBoxItems(currentColumn_cmb, ccState.CurrentRequestColumnNames());
                UpdateComboBoxIndex(currentColumn_cmb, ccState.History[ccState.CurrentRequest].CurrentColumnIndex);
                UpdateStatusText("Request changed!");
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Changes the SQL connection type and sets the connection string.
        /// </summary>
        /// <param name="set">if set to <c>true</c> [set].</param>
        ///  Changelog:
        ///             - 2.2.0 (07-13-2017) - Initial version.
        public void ChangeSqlConnectionStringType(SqlConnectionProviders set)
        {
            try
            {
                UpdateStatusText("Toggling connection string type...");
                if (checkGuard.CheckSet)
                {
                    UpdateMenuItemChecked(inputSqlConnectionStringNone_itm, set == SqlConnectionProviders.None);
                    UpdateMenuItemChecked(inputSqlConnectionStringPostgreSql_itm, set == SqlConnectionProviders.PostgreSQL);
                    UpdateMenuItemChecked(inputSqlConnectionStringSqlServer_itm, set == SqlConnectionProviders.SqlServer);
                    UpdateMenuItemChecked(inputSqlConnectionMySql_itm, set == SqlConnectionProviders.MySql);

                    ccState.SqlConnectionProvider = set;
                    switch (set)
                    {
                        case SqlConnectionProviders.MySql:
                            ccState.SqlProvider = new Classes.SqlSupport.MySqlProvider();
                            break;
                        case SqlConnectionProviders.PostgreSQL:
                            ccState.SqlProvider = new Classes.SqlSupport.PostgreSqlProvider();
                            break;
                        case SqlConnectionProviders.SqlServer:
                            ccState.SqlProvider = new Classes.SqlSupport.SqlServerProvider();
                            break;
                        case SqlConnectionProviders.None:
                            ccState.SqlProvider = null;
                            break;
                    }
                    if (ccState.SqlProvider != null)
                    {
                        ccState.SqlProvider.SqlConnectionString = ccState.SqlConnectionString;
                        ccState.SqlProvider.SqlQuery = ccState.SqlSelectQuery;
                    }

                    StateSave();
                    checkGuard.Reset();
                    UpdateStatusText("SQL provider changed!");

                    if (!CanSupportSqlConnectionProvider(set))
                    {
                        UpdateStatusText("No support for the Sql Connection provider!");

                        var files = ccState.SqlConnectionProvider == SqlConnectionProviders.None
                                        ? Constants.Instance.NeededFilesNoneConnection
                                        : ccState.SqlConnectionProvider == SqlConnectionProviders.MySql
                                            ? Constants.Instance.NeededFilesMySqlConnection
                                            : ccState.SqlConnectionProvider == SqlConnectionProviders.PostgreSQL
                                                ? Constants.Instance.NeededFilesPostgreSqlConnection
                                                : Constants.Instance.NeededFilesSqlServerConnection;

                        var str = new StringBuilder();
                        for (var i = 0; i < files.Count; i++)
                        {
                            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, files[i])))
                                str.Append($"{files[i]}, ");
                        }
                        str.Remove(str.Length - 2, 2);

                        var res = GetMessageBox(Constants.Instance.MessageTitleNoSqlProvider,
                            string.Format(Constants.Instance.MessageBodyNoSqlProvider, 
                                ccState.SqlConnectionProvider.ToString(), str.ToString(),
                                Environment.NewLine),
                            MessageBoxButtons.OK);
                    }
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Checks for updates.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Made multi-threaded.
        ///             - 1.3.0 (05-30-2017) - Initial version.
        public void CheckForUpdates()
        {
            try
            {
                UpdateStatusText("Checking for updates...");
                if (saveGuard.CheckSet)
                {
                    ToggleProgressBar();

                    var updateThread = new Thread(() => CheckForUpdatesHelper());

                    updateThread.Start();
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Checks for updates helper.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void CheckForUpdatesHelper()
        {
            try
            {
                var latestRelease = GitHub.GitHub.GetLatestRelease();
                ToggleProgressBar();
            
                switch (latestRelease.Status)
                {
                    case "GitHubStatusDown":
                        GetMessageBox(Constants.Instance.MessageTitleGitHubDown, Constants.Instance.MessageBodyGitHubDown,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateStatusText(Constants.Instance.MessageTitleGitHubDown);
                        saveGuard.Reset();
                        return;
                    case "GitHubStatusReleaseUnavailable":
                        GetMessageBox(Constants.Instance.MessageTitleLatestReleaseUnavailable, Constants.Instance.MessageBodyLatestReleaseUnavailable,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateStatusText(Constants.Instance.MessageTitleLatestReleaseUnavailable);
                        saveGuard.Reset();
                        return;
                }
            
                var releaseVersion = ConvertReleaseTagVersionToInt(latestRelease.tag_name);

                if (releaseVersion > Constants.ProgramVersion)
                {
                    UpdateStatusText(Constants.Instance.MessageTitleNewReleaseAvailable);
                    var result = GetMessageBox(Constants.Instance.MessageTitleNewReleaseAvailable, string.Format(Constants.Instance.MessageBodyNewReleaseAvailable, latestRelease.tag_name));

                    switch (result)
                    {
                        case DialogResult.Yes:
                            Process.Start(latestRelease.html_url);
                            break;
                    }
                }
                else
                {
                    UpdateStatusText(Constants.Instance.MessageTitleNoNewRelease);
                    GetMessageBox(Constants.Instance.MessageTitleNoNewRelease, Constants.Instance.MessageBodyNoNewRelease,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                saveGuard.Reset();
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Clears the history.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void ClearHistory()
        {
            try
            {
                UpdateStatusText("Clearing history...");
                ccState.CleanHistory(ccState.HistoryLog.Count, false);
                UpdateRequestHistory();
                StateSave();
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Copies the column.
        /// </summary>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void CopyColumn(bool replace)
        {
            try
            {
                UpdateStatusText("Copying column...");
                var text = string.Empty;

                if (replace)
                {
                    var lines = ccState.History[ccState.CurrentRequest].GetColumnRawText();

                    var str = new StringBuilder();
                    str.Append(ccState.LineSeparatorOptionPre);

                    var last = lines.Count - 1;
                    for (var i = 0; i < lines.Count; i++)
                        str.AppendFormat("{0}{1}", lines[i], i == last ? "" : ccState.LineSeparatorOptionInter);

                    str.Append(ccState.LineSeparatorOptionPost);
                    text = str.ToString();
                }
                else
                    text = currentColumnText_txt.Text;

                ClipBoard = text;
                currentColumnText_txt.Focus();
                currentColumnText_txt.SelectAll();
                UpdateStatusText("Column copied!");
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Copies the line.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Fixed line copying incorrect line and displaying invalid lines being copied. Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void CopyLine()
        {
            try
            {
                UpdateStatusText("Copying next line...");
                var index = Converters.ConvertToIntWithClamp(copyLineNumber_txt.Text, ccState.History[ccState.CurrentRequest].CopyNextLineIndex, 0, ccState.History[ccState.CurrentRequest].CurrentColumnRowCount - 1);

                if (index != ccState.History[ccState.CurrentRequest].CopyNextLineIndex)
                    ccState.History[ccState.CurrentRequest].CopyNextLineIndex = index;

                var line = ccState.History[ccState.CurrentRequest].CopyNextLineIndex;
                var text = ccState.History[ccState.CurrentRequest].GetCurrentColumnNextLineText();
                UpdateTextBox(copyLineNumber_txt, ccState.History[ccState.CurrentRequest].CopyNextLineIndex.ToString());
                ClipBoard = text;
                UpdateStatusText($"Line {line} copied [{text}]!");
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Copies multiple column.
        /// </summary>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public void CopyMultipleColumns(bool replace = false)
        {
            try
            {
                UpdateStatusText("Opening multiple column copy wizard...");

                var wiz = new OutputMultiColumnCopyWizard()
                {
                    AvailableColumns = ccState.CurrentRequestColumnNames(),
                    RequestData = ccState.GetCurrentRequest(),
                };

                var result = wiz.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var text = wiz.ResultText;

                    ClipBoard = text;
                    UpdateStatusText("Results copied!");
                }
                else
                {
                    UpdateStatusText("Wizard cancelled!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Deletes the request.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void DeleteRequest()
        {
            try
            {
                UpdateStatusText("Deleting request...");
                ccState.DeleteCurrentRequest();
                UpdateRequestHistory();
                if (requestHistory_cmb.Items.Count > 0)
                    requestHistory_cmb.SelectedIndex = 0;
                UpdateStatusText("Request deleted!");
                StateSave();
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Executes the SQL query.
        /// </summary>
        /// <exception cref="Exception">
        /// DLL(s) not found for the SQL connection provider!
        /// or
        /// No connection string provided!
        /// or
        /// No select query provided!
        /// or
        /// SQL select query provided is invalid!
        /// or
        /// Could not connect to the database!
        /// </exception>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public void ExecuteSqlQuery()
        {
            try
            {
                if (ccState.SqlConnectionProvider == SqlConnectionProviders.None) return;

                if (!CanSupportSqlConnectionProvider(ccState.SqlConnectionProvider))
                    throw new Exception("DLL(s) not found for the SQL connection provider!");

                if (string.IsNullOrWhiteSpace(ccState.SqlConnectionString))
                    throw new Exception("No connection string provided!");

                if (string.IsNullOrWhiteSpace(ccState.SqlSelectQuery))
                    throw new Exception("No select query provided!");

                if (!ccState.SqlProvider.SqlSelectQueryIsValid())
                    throw new Exception("SQL select query provided is invalid!");

                if (!ccState.SqlProvider.CanConnectToServer())
                    throw new Exception("Could not connect to the database!");

                var result = ccState.SqlProvider.ExecuteSqlQuery();
                if (string.IsNullOrWhiteSpace(result))
                    UpdateStatusText("SQL query returned no results!");
                else
                    AddInput(result);
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Exports the request.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void ExportRequest()
        {
            try
            {
                UpdateStatusText("Exporting request...");
                ClipBoard = ccState.ExportCurrentRequest();
                UpdateStatusText("Request exported!");
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Inputs the SQL wizard.
        /// </summary>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public void InputSqlWizard()
        {
            try
            {
                UpdateStatusText("SQL input wizard...");
                if (checkGuard.CheckSet)
                {
                    var wiz = new InputSqlWizard()
                    {
                        SqlProvider = ccState.SqlConnectionProvider,
                        QueryText = ccState.SqlSelectQuery,
                        ConnectionText = ccState.SqlConnectionString,
                    };
                    var result = wiz.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        UpdateMenuItemChecked(inputSqlConnectionStringNone_itm, wiz.SqlProvider == SqlConnectionProviders.None);
                        UpdateMenuItemChecked(inputSqlConnectionStringPostgreSql_itm, wiz.SqlProvider == SqlConnectionProviders.PostgreSQL);
                        UpdateMenuItemChecked(inputSqlConnectionStringSqlServer_itm, wiz.SqlProvider == SqlConnectionProviders.SqlServer);
                        UpdateMenuItemChecked(inputSqlConnectionMySql_itm, wiz.SqlProvider == SqlConnectionProviders.MySql);

                        ccState.SqlConnectionProvider = wiz.SqlProvider;
                        ccState.SqlConnectionString = wiz.ConnectionText;
                        ccState.SqlSelectQuery = wiz.QueryText;

                        switch (ccState.SqlConnectionProvider)
                        {
                            case SqlConnectionProviders.MySql:
                                ccState.SqlProvider = new Classes.SqlSupport.MySqlProvider();
                                break;
                            case SqlConnectionProviders.PostgreSQL:
                                ccState.SqlProvider = new Classes.SqlSupport.PostgreSqlProvider();
                                break;
                            case SqlConnectionProviders.SqlServer:
                                ccState.SqlProvider = new Classes.SqlSupport.SqlServerProvider();
                                break;
                            case SqlConnectionProviders.None:
                                ccState.SqlProvider = null;
                                break;
                        }

                        if (ccState.SqlProvider != null)
                        {
                            ccState.SqlProvider.SqlConnectionString = ccState.SqlConnectionString;
                            ccState.SqlProvider.SqlQuery = ccState.SqlSelectQuery;
                        }

                        StateSave();
                        checkGuard.Reset();
                        UpdateStatusText("SQL wizard completed, executing query...");

                        if (!CanSupportSqlConnectionProvider(ccState.SqlConnectionProvider))
                        {
                            UpdateStatusText("No support for the Sql Connection provider!");
                            return;
                        }

                        ExecuteSqlQuery();
                    }
                    else
                    {
                        UpdateStatusText("Wizard cancelled!");
                    }
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Opens the about.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void OpenAbout()
        {
            try
            {
                UpdateStatusText("Opening about...");
                var about = new About();
                about.ShowDialog();
                UpdateStatusText("About opened!");
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Opens the web page.
        /// </summary>
        /// <param name="url">The URL.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void OpenWebPage(string url)
        {
            try
            {
                UpdateStatusText("Opening browser...");
                Process.Start(url);
                UpdateStatusText($"Browser opened for {url}");
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Pastes the input.
        /// </summary>
        ///  Changelog:
        ///             - 2.2.0 (07-13-2017) - Moved most code to a new common input adding method.
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void PasteInput()
        {
            try
            {
                UpdateStatusText("Pasting input...");
                AddInput(ClipBoard);
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Preserves the current request.
        /// </summary>
        /// <param name="set">if set to <c>true</c> [set].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void PreserveCurrentRequest(bool? set = null)
        {
            try
            {
                UpdateStatusText("Preserving current request...");
                if (checkGuard.CheckSet)
                {
                    if (set == null)
                    {
                        UpdateCheckBox(preserveCurrentRequest_cxb, !preserveCurrentRequest_cxb.Checked);
                        UpdateMenuItemChecked(historySettingsPreserveCurrentRequest_itm, !historySettingsPreserveCurrentRequest_itm.Checked);
                    }
                    else
                    {
                        UpdateCheckBox(preserveCurrentRequest_cxb, (bool)set);
                        UpdateMenuItemChecked(historySettingsPreserveCurrentRequest_itm, (bool)set);
                    }

                    ccState.SetCurrentRequestPreservationToggle(preserveCurrentRequest_cxb.Checked);
                    if (!pasteGuard.Check)
                        StateSave();

                    checkGuard.Reset();
                    UpdateStatusText("Request preserved!");
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Sets the SQL connection string.
        /// </summary>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public void SetSqlConnectionString()
        {
            try
            {
                UpdateStatusText("Setting new connection string...");
                if (checkGuard.CheckSet)
                {
                    ccState.SqlConnectionString = GetTextboxInputResults(Constants.Instance.InputQuerySqlConnectionString, ccState.SqlConnectionString);
                    if (ccState.SqlProvider != null) ccState.SqlProvider.SqlConnectionString = ccState.SqlConnectionString;

                    StateSave();
                    checkGuard.Reset();
                    UpdateStatusText("New connection string set!");
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Sets the SQL select query.
        /// </summary>
        /// <param name="set">if set to <c>true</c> [set].</param>
        ///  Changelog:
        ///             - 2.2.0 (07-13-2017) - Initial version.
        public void SqlSelectionQuery()
        {
            try
            {
                UpdateStatusText("Asking for the SQL select query...");
                if (checkGuard.CheckSet)
                {
                    var query = GetMultilineTextboxInputResults(Constants.Instance.InputQuerySqlSelectQuery, ccState.SqlSelectQuery);


                    ccState.SqlSelectQuery = query;
                    if (ccState.SqlProvider != null) ccState.SqlProvider.SqlQuery = ccState.SqlSelectQuery;

                    StateSave();
                    checkGuard.Reset();
                    UpdateStatusText("New select query set!");
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// States the load.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void StateLoad()
        {
            try
            {
                UpdateStatusText("Choose file to load...");
                var fileSelector = new OpenFileDialog();
                fileSelector.DefaultExt = Constants.Instance.SaveExtension;
                fileSelector.Filter = string.Format("Column Copier Save File ({0})|*{0}|Compressed Column Copier Save File ({1})|*{1}",
                                                    Constants.Instance.SaveExtension, Constants.Instance.SaveExtensionCompressed);
                fileSelector.InitialDirectory = ExecutableDirectory;

                fileSelector.ShowDialog();
                var file = fileSelector.FileName;
                ccState.SaveFile = file;

                UpdateStatusText("File choosen!");
                StateOpen();
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// States the new.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void StateNew()
        {
            try
            {
                StateSaveAs();
                ClearHistory();
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// States the open.
        /// </summary>
        /// <param name="guardAlreadySet">if set to <c>true</c> [guard already set].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void StateOpen(bool guardAlreadySet = false)
        {
            try
            {
                UpdateStatusText("Attempting to load file...");
                if (!guardAlreadySet) while (!saveGuard.CheckSet) ;

                var loadThread = new Thread(() => StateLoadHelper());

                loadThread.Start();
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// States the save.
        /// </summary>
        /// <param name="guardAlreadySet">if set to <c>true</c> [guard already set].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void StateSave(bool guardAlreadySet = false)
        {
            try
            {
                if (!guardAlreadySet)
                {
                    if (saveGuard.CheckSet)
                    {
                        var saveThread = new Thread(() => StateSaveHelper());

                        saveThread.Start();
                    }
                }
                else
                {
                    var saveThread = new Thread(() => StateSaveHelper());

                    saveThread.Start();
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// States the save as.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void StateSaveAs()
        {
            try
            {
                UpdateStatusText("Attempting to save file...");
                while (!saveGuard.CheckSet) ;

                UpdateStatusText("Choosing new file name...");
                SaveFileDialog fileSelector = new SaveFileDialog();
                fileSelector.DefaultExt = Constants.Instance.SaveExtension;
                fileSelector.Filter = string.Format("Column Copier Save File ({0})|*{0}|Compressed Column Copier Save File ({1})|*{1}",
                                                    Constants.Instance.SaveExtension, Constants.Instance.SaveExtensionCompressed);
                fileSelector.InitialDirectory = ExecutableDirectory;

                fileSelector.ShowDialog();
                var file = fileSelector.FileName;
                ccState.SaveFile = file;
                UpdateStatusText("Filename choosen!");
                StateSave(true);
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Toggles the clean input text.
        /// </summary>
        /// <param name="set">if set to <c>true</c> [set].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void ToggleCleanInputText(bool? set = null)
        {
            try
            {
                UpdateStatusText("Toggling input cleaning...");
                if (checkGuard.CheckSet)
                {
                    if (set == null)
                    {
                        UpdateCheckBox(cleanInputText_cxb, !cleanInputText_cxb.Checked);
                        UpdateMenuItemChecked(inputSettingsCleanInputText_itm, !inputSettingsCleanInputText_itm.Checked);
                    }
                    else
                    {
                        UpdateCheckBox(cleanInputText_cxb, (bool)set);
                        UpdateMenuItemChecked(inputSettingsCleanInputText_itm, (bool)set);
                    }

                    ccState.RemoveEmptyLines = cleanInputText_cxb.Checked;
                    StateSave();
                    checkGuard.Reset();
                    UpdateStatusText("Input cleaning toggled!");
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Toggles the data has headers.
        /// </summary>
        /// <param name="set">if set to <c>true</c> [set].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void ToggleDataHasHeaders(bool? set = null)
        {
            try
            {
                UpdateStatusText("Toggling data headers...");
                if (checkGuard.CheckSet)
                {
                    if (set == null)
                    {
                        UpdateCheckBox(dataHasHeaders_cxb, !dataHasHeaders_cxb.Checked);
                        UpdateMenuItemChecked(inputSettingsDataHasHeaders_itm, !inputSettingsDataHasHeaders_itm.Checked);
                    }
                    else
                    {
                        UpdateCheckBox(dataHasHeaders_cxb, (bool)set);
                        UpdateMenuItemChecked(inputSettingsDataHasHeaders_itm, (bool)set);
                    }

                    ccState.DataHasHeaders = dataHasHeaders_cxb.Checked;
                    StateSave();
                    checkGuard.Reset();
                    UpdateStatusText("Data headers toggled!");
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Toggles the remove blank lines.
        /// </summary>
        /// <param name="set">if set to <c>true</c> [set].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void ToggleRemoveBlankLines(bool? set = null)
        {
            try
            {
                UpdateStatusText("Toggling blank line removal...");
                if (checkGuard.CheckSet)
                {
                    if (set == null)
                    {
                        UpdateCheckBox(removeBlankLines_cxb, !removeBlankLines_cxb.Checked);
                        UpdateMenuItemChecked(inputSettingsRemoveBlanks_itm, !inputSettingsRemoveBlanks_itm.Checked);
                    }
                    else
                    {
                        UpdateCheckBox(removeBlankLines_cxb, (bool)set);
                        UpdateMenuItemChecked(inputSettingsRemoveBlanks_itm, (bool)set);
                    }

                    ccState.RemoveEmptyLines = removeBlankLines_cxb.Checked;
                    StateSave();
                    checkGuard.Reset();
                    UpdateStatusText("Blank line removal toggled!");
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Toggles the save compression.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void ToggleSaveCompression()
        {
            try
            {
                UpdateStatusText("Toggling save compression...");
                if (saveGuard.CheckSet)
                {
                    var file = ccState.SaveFile;

                    var extension = Path.GetExtension(file);
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var directory = Path.GetDirectoryName(file);

                    if (extension == Constants.Instance.SaveExtensionCompressed)
                        fileSettingsCompressSave_itm.Checked = false;
                    else if (extension == Constants.Instance.SaveExtension)
                        fileSettingsCompressSave_itm.Checked = true;

                    var newFile = Path.Combine(directory,
                                        string.Format("{0}{1}", fileName, fileSettingsCompressSave_itm.Checked
                                            ? Constants.Instance.SaveExtensionCompressed : Constants.Instance.SaveExtension));

                    ccState.SaveFile = newFile;
                    StateSave(true);
                    // delete the old save file...
                    if (File.Exists(file))
                        File.Delete(file);
                    UpdateStatusText("Save compression toggled!");
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Toggles the show on top.
        /// </summary>
        /// <param name="set">if set to <c>true</c> [set].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void ToggleShowOnTop(bool? set = null)
        {
            try
            {
                UpdateStatusText("Toggling program shown on top...");
                if (checkGuard.CheckSet)
                {
                    if (set == null)
                    {
                        UpdateCheckBox(showOnTop_cxb, !showOnTop_cxb.Checked);
                        UpdateMenuItemChecked(fileSettingsShowOnTop_itm, !fileSettingsShowOnTop_itm.Checked);
                    }
                    else
                    {
                        UpdateCheckBox(showOnTop_cxb, (bool)set);
                        UpdateMenuItemChecked(fileSettingsShowOnTop_itm, (bool)set);
                    }

                    ccState.ShowOnTop = showOnTop_cxb.Checked;
                    this.TopMost = ccState.ShowOnTop;

                    StateSave();
                    checkGuard.Reset();
                    UpdateStatusText("Program shown on top toggled!");
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the line seperator options.
        /// </summary>
        /// <param name="option">The option.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Moved line seperator options to the Constants class.
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void UpdateLineSeperatorOptions(LineSeparatorOptions option)
        {
            try
            {
                UpdateStatusText("Updating line separator option...");
                if (checkGuard.CheckSet)
                {
                    var choice = Constants.Instance.LineSeperators[option];

                    ccState.LineSeparatorOptionIndex = choice.Item4;

                    UpdateComboBoxIndex(seperatorOption_cmb, choice.Item4);
                    UpdateTextBox(seperatorItem_txt, choice.Item1);
                    UpdateTextBox(seperatorItemPre_txt, choice.Item2);
                    UpdateTextBox(seperatorItemPost_txt, choice.Item3);

                    checkGuard.Reset();
                    UpdateStatusText("Line separator option updated!");
                }
                else
                {
                    UpdateStatusText("Busy, please try again!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Determines whether this instance [can support SQL connection provider] the specified connection.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns><c>true</c> if this instance [can support SQL connection provider] the specified connection; otherwise, <c>false</c>.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private bool CanSupportSqlConnectionProvider(SqlConnectionProviders conn)
        {
            var files = conn == SqlConnectionProviders.None
                            ? Constants.Instance.NeededFilesNoneConnection
                            : conn == SqlConnectionProviders.MySql
                                ? Constants.Instance.NeededFilesMySqlConnection
                                : conn == SqlConnectionProviders.PostgreSQL
                                    ? Constants.Instance.NeededFilesPostgreSqlConnection
                                    : Constants.Instance.NeededFilesSqlServerConnection;

            for (var i = 0; i < files.Count; i++)
            {
                if (!File.Exists(Path.Combine(Environment.CurrentDirectory, files[i])))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Changes the form opacity.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="value">The value.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void ChangeFormOpacity(Form form, int value)
        {
            try
            {
                UpdateStatusText("Changing program opacity...");
                if (form.InvokeRequired)
                {
                    var d = new ChangeFormOpacityDelegate(ChangeFormOpacity);
                    this.Invoke(d, new object[] { form, value });
                }
                else
                {
                    form.Opacity = (value / 100d);
                    UpdateStatusText("Opacity changed!");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Converts the release tag version to int.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 1.1.0 (08-29-2016) - Initial version.
        private static int ConvertReleaseTagVersionToInt(string tag)
        {
            var tagBits = tag.Split('.');
            var str = new StringBuilder();
            foreach (var bit in tagBits)
                str.Append(bit);

            return Converters.ConvertToInt(str.ToString(), -1);
        }

        /// <summary>
        /// Handles the Click event of the about_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void about_btn_Click(object sender, EventArgs e)
        {
            OpenAbout();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the cleanInputText_cxb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void cleanInputText_cxb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleRemoveBlankLines(cleanInputText_cxb.Checked);
        }

        /// <summary>
        /// Handles the Click event of the clearHistory_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void clearHistory_btn_Click(object sender, EventArgs e)
        {
            ClearHistory();
        }

        /// <summary>
        /// Handles the Click event of the copyColumn_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void copyColumn_btn_Click(object sender, EventArgs e)
        {
            CopyColumn(false);
        }

        /// <summary>
        /// Handles the Click event of the copyLineWithSeperators_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void copyLineWithSeperators_btn_Click(object sender, EventArgs e)
        {
            CopyColumn(true);
        }

        /// <summary>
        /// Handles the Click event of the copyNextLine_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void copyNextLine_btn_Click(object sender, EventArgs e)
        {
            CopyLine();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the currentColumn_cmb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void currentColumn_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeColumn(currentColumn_cmb.SelectedIndex);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the dataHasHeaders_cxb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void dataHasHeaders_cxb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleDataHasHeaders(dataHasHeaders_cxb.Checked);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the defaultPriorityName_rbn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void defaultPriorityName_rbn_CheckedChanged(object sender, EventArgs e)
        {
            if (checkGuard.CheckSet)
            {
                SetDefaultPriorityToggles(DefaultColumnPriority.Name);
                StateSave();
                checkGuard.Reset();
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the defaultPriorityNumber_rbn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void defaultPriorityNumber_rbn_CheckedChanged(object sender, EventArgs e)
        {
            if (checkGuard.CheckSet)
            {
                SetDefaultPriorityToggles(DefaultColumnPriority.Number);
                StateSave();
                checkGuard.Reset();
            }
        }

        /// <summary>
        /// Handles the Click event of the deleteRequest_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void deleteRequest_btn_Click(object sender, EventArgs e)
        {
            DeleteRequest();
        }

        /// <summary>
        /// Handles the Click event of the exportRequest_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void exportRequest_btn_Click(object sender, EventArgs e)
        {
            ExportRequest();
        }

        /// <summary>
        /// Handles the Click event of the fileExit_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void fileExit_itm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the fileNew_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void fileNew_itm_Click(object sender, EventArgs e)
        {
            stateNew_btn_Click(sender, e);
        }

        /// <summary>
        /// Handles the Click event of the fileOpen_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void fileOpen_itm_Click(object sender, EventArgs e)
        {
            stateOpen_btn_Click(sender, e);
        }

        /// <summary>
        /// Handles the Click event of the fileSave_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void fileSave_itm_Click(object sender, EventArgs e)
        {
            stateSave_btn_Click(sender, e);
        }

        /// <summary>
        /// Handles the Click event of the fileSaveAs_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void fileSaveAs_itm_Click(object sender, EventArgs e)
        {
            stateSaveAs_btn_Click(sender, e);
        }

        /// <summary>
        /// Handles the Click event of the fileSettingsCompressSave_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void fileSettingsCompressSave_itm_Click(object sender, EventArgs e)
        {
            ToggleSaveCompression();
        }

        /// <summary>
        /// Handles the Click event of the fileSettingsProgramOpacity_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void fileSettingsProgramOpacity_itm_Click(object sender, EventArgs e)
        {
            var opacity = GetTextboxInputResults(Constants.Instance.InputQueryProgramOpacity, ccState.ProgramOpacity.ToString());
            ccState.ProgramOpacity = Converters.ConvertToIntWithClamp(opacity, 100, 0, 100);
            ChangeOpacity();
            StateSave();
        }

        /// <summary>
        /// Handles the Click event of the fileSettingsShowOnTop_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void fileSettingsShowOnTop_itm_Click(object sender, EventArgs e)
        {
            ToggleShowOnTop(!showOnTop_cxb.Checked);
        }

        /// <summary>
        /// Forms the exception issue URL.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.1.0 (06-06-2017) - Initial version.
        private string FormExceptionIssueUrl(Exception ex)
        {
            return string.Format("{0}/new?title={1}&body={2)", Constants.Instance.UrlSupport, ex.Message.ToString(), ex.InnerException.ToString());
        }

        /// <summary>
        /// Gets the combobox input results.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="items">The items.</param>
        /// <param name="defaultReturn">The default return.</param>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private int GetComboboxInputResults(string question, List<string> items, int defaultReturn)
        {
            var inputBox = new InputComboDialogBox()
            {
                QuestionText = question,
            };
            inputBox.SetInputListItems(items);
            inputBox.InputSelectedItem = seperatorOption_cmb.SelectedIndex;

            var result = inputBox.ShowDialog();

            return result == DialogResult.OK
                ? inputBox.InputSelectedItem
                : defaultReturn;
        }

        /// <summary>
        /// Gets the message box.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="question">The question.</param>
        /// <param name="buttonsType">Type of the buttons.</param>
        /// <param name="iconType">Type of the icon.</param>
        /// <returns>DialogResult.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private DialogResult GetMessageBox(string title, string question, MessageBoxButtons buttonsType = MessageBoxButtons.YesNo, MessageBoxIcon iconType = MessageBoxIcon.Question)
        {
            return MessageBox.Show(question, title, buttonsType, iconType);
        }

        /// <summary>
        /// Gets the multiline textbox input results.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="defaultText">The default text.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private string GetMultilineTextboxInputResults(string question, string defaultText)
        {
            var inputBox = new InputMultilineTextDialogBox()
            {
                QuestionText = question,
                InputText = defaultText,
            };

            var result = inputBox.ShowDialog();

            return result == DialogResult.OK
                ? inputBox.InputText
                : defaultText;
        }

        /// <summary>
        /// Gets the textbox input results.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="defaultText">The default text.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private string GetTextboxInputResults(string question, string defaultText)
        {
            var inputBox = new InputTextDialogBox()
            {
                QuestionText = question,
                InputText = defaultText,
            };

            var result = inputBox.ShowDialog();

            return result == DialogResult.OK
                ? inputBox.InputText
                : defaultText;
        }

        /// <summary>
        /// Handles the Click event of the help_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void help_btn_Click(object sender, EventArgs e)
        {
            OpenWebPage(Constants.Instance.UrlHelp);
        }

        /// <summary>
        /// Handles the Click event of the helpAbout_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void helpAbout_itm_Click(object sender, EventArgs e)
        {
            OpenAbout();
        }

        /// <summary>
        /// Handles the Click event of the helpDocumentation_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void helpDocumentation_itm_Click(object sender, EventArgs e)
        {
            OpenWebPage(Constants.Instance.UrlHelp);
        }

        /// <summary>
        /// Handles the Click event of the helpSupport_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void helpSupport_itm_Click(object sender, EventArgs e)
        {
            OpenWebPage(Constants.Instance.UrlSupport);
        }

        /// <summary>
        /// Handles the Click event of the helpUpdateCheck_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void helpUpdateCheck_itm_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        /// <summary>
        /// Handles the Click event of the historyChangeRequest_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void historyChangeRequest_itm_Click(object sender, EventArgs e)
        {
            if (requestHistory_cmb.Items.Count > 0)
            {
                var inputItems = new List<string>();
                for (var i = 0; i < requestHistory_cmb.Items.Count; i++)
                    inputItems.Add(requestHistory_cmb.Items[i].ToString());

                UpdateComboBoxIndex(requestHistory_cmb, GetComboboxInputResults(Constants.Instance.InputQueryChangeHistoryRequest,
                                                            inputItems, requestHistory_cmb.SelectedIndex));
            }
            else
            {
                GetMessageBox(Constants.Instance.MessageTitleNoRequestHistory, Constants.Instance.MessageBodyNoRequestHistory,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Handles the Click event of the historyClearHistory_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void historyClearHistory_itm_Click(object sender, EventArgs e)
        {
            ClearHistory();
        }

        /// <summary>
        /// Handles the Click event of the historyDeleteRequest_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void historyDeleteRequest_itm_Click(object sender, EventArgs e)
        {
            DeleteRequest();
        }

        /// <summary>
        /// Handles the Click event of the historySettingsMaxRequestHistory_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void historySettingsMaxRequestHistory_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(maxHistory_txt, GetTextboxInputResults(Constants.Instance.InputQueryMaxHistory, maxHistory_txt.Text));
        }

        /// <summary>
        /// Handles the Click event of the historySettingsPreserveCurrentRequest_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void historySettingsPreserveCurrentRequest_itm_Click(object sender, EventArgs e)
        {
            PreserveCurrentRequest(!preserveCurrentRequest_cxb.Checked);
        }

        /// <summary>
        /// Handles the Click event of the inputExecuteSqlQuery_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void inputExecuteSqlQuery_itm_Click(object sender, EventArgs e)
        {
            ExecuteSqlQuery();
        }

        /// <summary>
        /// Handles the Click event of the inputPaste_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void inputPaste_itm_Click(object sender, EventArgs e)
        {
            PasteInput();
        }

        /// <summary>
        /// Handles the Click event of the inputPasteAndCopy_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void inputPasteAndCopy_itm_Click(object sender, EventArgs e)
        {
            PasteInput();
            CopyColumn(false);
        }

        /// <summary>
        /// Handles the Click event of the inputSettingsCleanInputText_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void inputSettingsCleanInputText_itm_Click(object sender, EventArgs e)
        {
            ToggleRemoveBlankLines(!cleanInputText_cxb.Checked);
        }

        /// <summary>
        /// Handles the Click event of the inputSettingsDataHasHeaders_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void inputSettingsDataHasHeaders_itm_Click(object sender, EventArgs e)
        {
            ToggleDataHasHeaders(!dataHasHeaders_cxb.Checked);
        }

        /// <summary>
        /// Handles the Click event of the inputSettingsDefaultColumnName_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void inputSettingsDefaultColumnName_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(defaultColumnName_txt, GetTextboxInputResults(Constants.Instance.InputQueryDefaultColumnName, defaultColumnName_txt.Text));
        }

        /// <summary>
        /// Handles the Click event of the inputSettingsDefaultColumnNumber_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void inputSettingsDefaultColumnNumber_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(defaultColumnNumber_txt, GetTextboxInputResults(Constants.Instance.InputQueryDefaultColumnNumber, defaultColumnNumber_txt.Text));
        }

        /// <summary>
        /// Handles the Click event of the inputSettingsDefaultPriorityName_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void inputSettingsDefaultPriorityName_itm_Click(object sender, EventArgs e)
        {
            if (checkGuard.CheckSet)
            {
                SetDefaultPriorityToggles(DefaultColumnPriority.Name);
                StateSave();
                checkGuard.Reset();
            }
        }

        /// <summary>
        /// Handles the Click event of the inputSettingsDefaultPriorityNameSimilarity_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void inputSettingsDefaultPriorityNameSimilarity_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(defaultPriorityNameSimilarity_txt, GetTextboxInputResults(Constants.Instance.InputQueryNameSimilarityValue, defaultPriorityNameSimilarity_txt.Text));
        }

        /// <summary>
        /// Handles the Click event of the inputSettingsDefaultPriorityNumber_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void inputSettingsDefaultPriorityNumber_itm_Click(object sender, EventArgs e)
        {
            if (checkGuard.CheckSet)
            {
                SetDefaultPriorityToggles(DefaultColumnPriority.Number);
                StateSave();
                checkGuard.Reset();
            }
        }

        /// <summary>
        /// Handles the Click event of the inputSettingsRemoveBlanks_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void inputSettingsRemoveBlanks_itm_Click(object sender, EventArgs e)
        {
            ToggleCleanInputText(!removeBlankLines_cxb.Checked);
        }

        /// <summary>
        /// Handles the Click event of the inputSqlConnection_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void inputSqlConnection_itm_Click(object sender, EventArgs e)
        {
            SetSqlConnectionString();
        }

        /// <summary>
        /// Handles the Click event of the inputSqlConnectionMySql_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void inputSqlConnectionMySql_itm_Click(object sender, EventArgs e)
        {
            ChangeSqlConnectionStringType(SqlConnectionProviders.MySql);
        }

        /// <summary>
        /// Handles the Click event of the inputSqlConnectionQuery_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void inputSqlConnectionQuery_itm_Click(object sender, EventArgs e)
        {
            SqlSelectionQuery();
        }

        /// <summary>
        /// Handles the Click event of the inputSqlConnectionStringNone_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void inputSqlConnectionStringNone_itm_Click(object sender, EventArgs e)
        {
            ChangeSqlConnectionStringType(SqlConnectionProviders.None);
        }

        /// <summary>
        /// Handles the Click event of the inputSqlConnectionStringPostgreSql_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void inputSqlConnectionStringPostgreSql_itm_Click(object sender, EventArgs e)
        {
            ChangeSqlConnectionStringType(SqlConnectionProviders.PostgreSQL);
        }

        /// <summary>
        /// Handles the Click event of the inputSqlConnectionStringSqlServer_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void inputSqlConnectionStringSqlServer_itm_Click(object sender, EventArgs e)
        {
            ChangeSqlConnectionStringType(SqlConnectionProviders.SqlServer);
        }

        /// <summary>
        /// Handles the Click event of the inputSqlInputWizard_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void inputSqlInputWizard_itm_Click(object sender, EventArgs e)
        {
            InputSqlWizard();
        }

        /// <summary>
        /// Handles the Click event of the outputCopyColumn_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputCopyColumn_itm_Click(object sender, EventArgs e)
        {
            CopyColumn(false);
        }

        /// <summary>
        /// Handles the Click event of the outputCopyLineWithSeperator_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputCopyLineWithSeperator_itm_Click(object sender, EventArgs e)
        {
            CopyColumn(true);
        }

        /// <summary>
        /// Handles the Click event of the outputCopyNextLine_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputCopyNextLine_itm_Click(object sender, EventArgs e)
        {
            CopyLine();
        }

        /// <summary>
        /// Handles the Click event of the outputExportRequest_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputExportRequest_itm_Click(object sender, EventArgs e)
        {
            ExportRequest();
        }

        /// <summary>
        /// Handles the Click event of the outputMultiColumnCopyWizard_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void outputMultiColumnCopyWizard_itm_Click(object sender, EventArgs e)
        {
            CopyMultipleColumns();
        }

        /// <summary>
        /// Handles the Click event of the outputSettingLineReplacementPresetBlank_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingLineReplacementPresetBlank_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeparatorOptions.Nothing);
        }

        /// <summary>
        /// Handles the Click event of the outputSettingLineReplacementPresetComma_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingLineReplacementPresetComma_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeparatorOptions.Comma);
        }

        /// <summary>
        /// Handles the Click event of the outputSettingLineReplacementPresetDoubleQuoteComma_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingLineReplacementPresetDoubleQuoteComma_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeparatorOptions.DoubleQuoteComma);
        }

        /// <summary>
        /// Handles the Click event of the outputSettingLineReplacementPresetParenthesisQuotes_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingLineReplacementPresetParenthesisQuotes_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeparatorOptions.DoubleQuoteParenthesisComma);
        }

        /// <summary>
        /// Handles the Click event of the outputSettingLineReplacementPresetSemiColan_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingLineReplacementPresetSemiColan_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeparatorOptions.SemiColon);
        }

        /// <summary>
        /// Handles the Click event of the outputSettingLineReplacementPresetSqlComma_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingLineReplacementPresetSqlComma_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeparatorOptions.ParenthesisComma);
        }

        /// <summary>
        /// Handles the Click event of the outputSettingLineReplacementPresetSqlText_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingLineReplacementPresetSqlText_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeparatorOptions.SingleQuoteParenthesisComma);
        }

        /// <summary>
        /// Handles the Click event of the outputSettingsCurrentCopyNextLineLine_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingsCurrentCopyNextLineLine_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(copyLineNumber_txt, GetTextboxInputResults(Constants.Instance.InputQueryNextLineCopyLine, copyLineNumber_txt.Text));
        }

        /// <summary>
        /// Handles the Click event of the outputSettingsLineReplacementPostString_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingsLineReplacementPostString_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(seperatorItemPost_txt, GetTextboxInputResults(Constants.Instance.InputQueryLineReplacementPost, seperatorItemPost_txt.Text));
        }

        /// <summary>
        /// Handles the Click event of the outputSettingsLineReplacementPreString_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingsLineReplacementPreString_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(seperatorItemPre_txt, GetTextboxInputResults(Constants.Instance.InputQueryLineReplacementPre, seperatorItemPre_txt.Text));
        }

        /// <summary>
        /// Handles the Click event of the outputSettingsLineReplacementSeperator_itm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void outputSettingsLineReplacementSeperator_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(seperatorItem_txt, GetTextboxInputResults(Constants.Instance.InputQueryLineReplacementSeparator, seperatorItem_txt.Text));
        }

        /// <summary>
        /// Handles the Click event of the paste_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void paste_btn_Click(object sender, EventArgs e)
        {
            PasteInput();
        }

        /// <summary>
        /// Handles the Click event of the pasteAndCopy_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void pasteAndCopy_btn_Click(object sender, EventArgs e)
        {
            PasteInput();
            CopyColumn(false);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the preserveCurrentRequest_cxb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void preserveCurrentRequest_cxb_CheckedChanged(object sender, EventArgs e)
        {
            PreserveCurrentRequest(preserveCurrentRequest_cxb.Checked);
        }

        /// <summary>
        /// Handles the Click event of the preSetsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void preSetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var inputItems = new List<string>();
            for (var i = 0; i < seperatorOption_cmb.Items.Count; i++)
                inputItems.Add(seperatorOption_cmb.Items[i].ToString());

            UpdateComboBoxIndex(seperatorOption_cmb, GetComboboxInputResults(Constants.Instance.InputQueryChangeHistoryRequest,
                                                        inputItems, seperatorOption_cmb.SelectedIndex));
        }

        /// <summary>
        /// Handles the CheckedChanged event of the removeBlankLines_cxb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void removeBlankLines_cxb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleRemoveBlankLines(removeBlankLines_cxb.Checked);
        }

        private void requestHistory_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeRequest(requestHistory_cmb.SelectedIndex);
        }

        /// <summary>
        /// Handles the TextChanged event of the seperatorItem_txt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void seperatorItem_txt_TextChanged(object sender, EventArgs e)
        {
            ccState.LineSeparatorOptionInter = seperatorItem_txt.Text;
        }

        /// <summary>
        /// Handles the TextChanged event of the seperatorItemPost_txt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void seperatorItemPost_txt_TextChanged(object sender, EventArgs e)
        {
            ccState.LineSeparatorOptionPost = seperatorItemPost_txt.Text;
        }

        /// <summary>
        /// Handles the TextChanged event of the seperatorItemPre_txt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void seperatorItemPre_txt_TextChanged(object sender, EventArgs e)
        {
            ccState.LineSeparatorOptionPre = seperatorItemPre_txt.Text;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the seperatorOption_cmb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void seperatorOption_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions((LineSeparatorOptions)seperatorOption_cmb.SelectedIndex);
        }

        /// <summary>
        /// Sets the default priority toggles.
        /// </summary>
        /// <param name="priority">The priority.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void SetDefaultPriorityToggles(DefaultColumnPriority priority)
        {
            var nameChecked = false;
            var numberChecked = false;
            switch (priority)
            {
                case DefaultColumnPriority.Name:
                    nameChecked = true;
                    numberChecked = false;
                    break;

                case DefaultColumnPriority.Number:
                    nameChecked = false;
                    numberChecked = true;
                    break;
            }

            UpdateRadioButton(defaultPriorityName_rbn, nameChecked);
            UpdateRadioButton(defaultPriorityNumber_rbn, numberChecked);
            UpdateMenuItemChecked(inputSettingsDefaultPriorityName_itm, nameChecked);
            UpdateMenuItemChecked(inputSettingsDefaultPriorityNumber_itm, numberChecked);
        }

        /// <summary>
        /// Handles the CheckedChanged event of the showOnTop_cxb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void showOnTop_cxb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleShowOnTop(showOnTop_cxb.Checked);
        }

        /// <summary>
        /// States the load helper.
        /// </summary>
        ///  Changelog:
        ///             - 2.2.1 (08-07-2017) - Properly updates the seperator options on loads. Updates program title upon saving.
        ///             - 2.1.0 (06-07-2017) - Support for the new save system, fixed default request selection being incorrect.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void StateLoadHelper()
        {
            UpdateStatusText("Loading program state...");
            ToggleProgressBar();

            var result = ccState.Load();
            if (result == Ternary.True)
            {
                // update gui here
                ChangeOpacity();
                UpdateCheckBox(removeBlankLines_cxb, ccState.RemoveEmptyLines);
                UpdateCheckBox(dataHasHeaders_cxb, ccState.DataHasHeaders);
                UpdateCheckBox(cleanInputText_cxb, ccState.CleanInputData);
                UpdateCheckBox(showOnTop_cxb, ccState.ShowOnTop);

                UpdateTextBox(defaultColumnName_txt, ccState.DefaultColumnName);
                UpdateTextBox(defaultColumnNumber_txt, ccState.DefaultColumnIndex.ToString());
                UpdateTextBox(defaultPriorityNameSimilarity_txt, ccState.DefaultColumnNameMatch.ToString());

                UpdateTextBox(seperatorItemPre_txt, ccState.LineSeparatorOptionPre);
                UpdateTextBox(seperatorItemPost_txt, ccState.LineSeparatorOptionPost);
                UpdateTextBox(seperatorItem_txt, ccState.LineSeparatorOptionInter);
                UpdateComboBoxIndex(seperatorOption_cmb, ccState.LineSeparatorOptionIndex);

                switch (ccState.DefaultColumnPriority)
                {
                    case DefaultColumnPriority.Name:
                        UpdateRadioButton(defaultPriorityName_rbn, true);
                        UpdateRadioButton(defaultPriorityNumber_rbn, false);
                        break;

                    case DefaultColumnPriority.Number:
                        UpdateRadioButton(defaultPriorityName_rbn, false);
                        UpdateRadioButton(defaultPriorityNumber_rbn, true);
                        break;
                }
                UpdateRequestHistory();

                UpdateComboBoxIndex(requestHistory_cmb, ccState.CurrentRequestIndex);

                UpdateProgramTitle(this, $"Column Copier - {Path.GetFileNameWithoutExtension(ccState.SaveFile)}");
                UpdateStatusText("State loaded!");
            }
            else if (result == Ternary.Neutral)
            {
                UpdateStatusText("State not loaded, old save version!");
            }
            else
            {
                UpdateStatusText("State could not be loaded!");
            }

            ToggleProgressBar();
            saveGuard.Reset();
        }

        /// <summary>
        /// Handles the Click event of the stateNew_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void stateNew_btn_Click(object sender, EventArgs e)
        {
            StateNew();
        }

        /// <summary>
        /// Handles the Click event of the stateOpen_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void stateOpen_btn_Click(object sender, EventArgs e)
        {
            StateLoad();
        }

        /// <summary>
        /// Handles the Click event of the stateSave_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void stateSave_btn_Click(object sender, EventArgs e)
        {
            StateSave();
        }

        /// <summary>
        /// Handles the Click event of the stateSaveAs_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void stateSaveAs_btn_Click(object sender, EventArgs e)
        {
            StateSaveAs();
        }

        /// <summary>
        /// States the save helper.
        /// </summary>
        ///  Changelog:
        ///             - 2.2.1 (08-07-2017) - Updates program title upon saving...
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void StateSaveHelper()
        {
            UpdateStatusText("Saving program state...");
            ToggleProgressBar();

            ccState.Save();

            UpdateProgramTitle(this, $"Column Copier - {Path.GetFileNameWithoutExtension(ccState.SaveFile)}");
            UpdateStatusText("Program state saved!");
            ToggleProgressBar();
            saveGuard.Reset();
        }

        /// <summary>
        /// Toggles the progress bar.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 1.2.0 (09-30-2017) - Initial version.
        private void ToggleProgressBar()
        {
            try
            {
                if (progress_bar.InvokeRequired)
                {
                    var d = new UpdateProgressBar(ToggleProgressBar);
                    this.Invoke(d);
                }
                else
                {
                    progress_bar.Visible = !progress_bar.Visible;
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the CheckBox.
        /// </summary>
        /// <param name="checkbox">The checkbox.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void UpdateCheckBox(CheckBox checkbox, bool value)
        {
            try
            {
                if (checkbox.InvokeRequired)
                {
                    var d = new UpdateCheckBoxDelegate(UpdateCheckBox);
                    this.Invoke(d, new object[] { checkbox, value });
                }
                else
                {
                    checkbox.Checked = value;
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the index of the ComboBox.
        /// </summary>
        /// <param name="comboBox">The combo box.</param>
        /// <param name="index">The index.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void UpdateComboBoxIndex(ComboBox comboBox, int index)
        {
            try
            {
                if (comboBox.InvokeRequired)
                {
                    var d = new UpdateComboBoxIndexDelegate(UpdateComboBoxIndex);
                    this.Invoke(d, new object[] { comboBox, index });
                }
                else
                {
                    comboBox.SelectedIndex = index;
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the ComboBox items.
        /// </summary>
        /// <param name="comboBox">The combo box.</param>
        /// <param name="values">The values.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void UpdateComboBoxItems(ComboBox comboBox, List<string> values)
        {
            try
            {
                if (comboBox.InvokeRequired)
                {
                    var d = new UpdateComboBoxItemsDelegate(UpdateComboBoxItems);
                    this.Invoke(d, new object[] { comboBox, values });
                }
                else
                {
                    comboBox.Items.Clear();
                    for (var i = 0; i < values.Count; i++)
                        comboBox.Items.Add(values[i]);
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the ComboBox text.
        /// </summary>
        /// <param name="combobox">The combobox.</param>
        /// <param name="text">The text.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void UpdateComboBoxText(ComboBox combobox, string text)
        {
            try
            {
                if (combobox.InvokeRequired)
                {
                    var d = new UpdateComboBoxTextDelegate(UpdateComboBoxText);
                    this.Invoke(d, new object[] { combobox, text });
                }
                else
                {
                    combobox.Text = text;
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the label text.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="text">The text.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void UpdateLabelText(Label label, string text)
        {
            try
            {
                if (label.InvokeRequired)
                {
                    var d = new UpdateLabelDelegate(UpdateLabelText);
                    this.Invoke(d, new object[] { label, text });
                }
                else
                {
                    try
                    {
                        label.Text = text;
                    }
                    catch
                    {
                        var d = new UpdateLabelDelegate(UpdateLabelText);
                        this.Invoke(d, new object[] { label, text });
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the menu item checked.
        /// </summary>
        /// <param name="menuitem">The menuitem.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void UpdateMenuItemChecked(ToolStripMenuItem menuitem, bool value)
        {
            try
            {
                menuitem.Checked = value;

                /*
                if (menuitem.InvokeRequired)
                {
                    var d = new UpdateMenuItemCheckedDelegate(UpdateMenuItemChecked);
                    this.Invoke(d, new object[] { menuitem, value });
                }
                else
                {
                    menuitem.Checked = value;
                }*/
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the program's title text.
        /// </summary>
        /// <param name="text">The text.</param>
        ///  Changelog:
        ///             - 2.2.1 (08-07-2017) - Initial version.
        private void UpdateProgramTitle(Form form, string text)
        {
            try
            {
                if (form.InvokeRequired)
                {
                    var d = new UpdateProgramTitleDelegate(UpdateProgramTitle);
                    this.Invoke(d, new object[] { form, text });
                }
                else
                {
                    try
                    {
                        form.Text = text;
                    }
                    catch
                    {
                        var d = new UpdateProgramTitleDelegate(UpdateProgramTitle);
                        this.Invoke(d, new object[] { form, text });
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the RadioButton.
        /// </summary>
        /// <param name="radiobutton">The radiobutton.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void UpdateRadioButton(RadioButton radiobutton, bool value)
        {
            try
            {
                if (radiobutton.InvokeRequired)
                {
                    var d = new UpdateRadioButtonDelegate(UpdateRadioButton);
                    this.Invoke(d, new object[] { radiobutton, value });
                }
                else
                {
                    radiobutton.Checked = value;
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the request history.
        /// </summary>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void UpdateRequestHistory()
        {
            UpdateComboBoxItems(requestHistory_cmb, ccState.GetRequestHistory());

            UpdateTextBox(currentColumnText_txt, string.Empty);
            UpdateComboBoxItems(currentColumn_cmb, new List<string>());

            UpdateComboBoxText(currentColumn_cmb, string.Empty);
            UpdateComboBoxText(requestHistory_cmb, string.Empty);

            UpdateLabelText(statCurrentColumn_txt, string.Format(Constants.Instance.FormatStatCurrentColumn, string.Empty, string.Empty));
            UpdateLabelText(statNumberColumns_txt, string.Format(Constants.Instance.FormatStatNumberColumns, string.Empty));
            UpdateLabelText(statNumberRows_txt, string.Format(Constants.Instance.FormatStatNumberRows, string.Empty));
        }

        /// <summary>
        /// Updates the status text.
        /// </summary>
        /// <param name="text">The text.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void UpdateStatusText(string text)
        {
            UpdateLabelText(status_txt, text);
        }

        /// <summary>
        /// Updates the text box.
        /// </summary>
        /// <param name="textbox">The textbox.</param>
        /// <param name="text">The text.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Improved error handling.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void UpdateTextBox(TextBox textbox, string text)
        {
            try
            {
                if (textbox.InvokeRequired)
                {
                    var d = new UpdateTextBoxDelegate(UpdateTextBox);
                    this.Invoke(d, new object[] { textbox, text });
                }
                else
                {
                    textbox.Text = text;
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Exception occurred!");

                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        #endregion Private Methods
    }
}