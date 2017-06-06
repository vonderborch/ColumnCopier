// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Main.cs
// Author           : Christian
// Created          : 08-15-2016
// 
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="Main.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Main class.
// </summary>
//
// Changelog: 
//            - 2.0.0 (xx-xx-2017) - Rebuilt!
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
using System.Xml;
using System.Xml.Linq;

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
        private CCState ccState;
        private Guard checkGuard;
        private Guard saveGuard;
        private Guard pasteGuard;

        public Main()
        {
            InitializeComponent();

            ccState = new CCState();
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

        public void CheckForUpdates()
        {
            if (saveGuard.CheckSet)
            {
                ToggleProgressBar();

                var updateThread = new Thread(() => CheckForUpdatesHelper());

                updateThread.Start();
            }
            else
            {

            }
        }

        public void CheckForUpdatesHelper()
        {
            var latestRelease = GitHub.GitHub.GetLatestRelease();
            ToggleProgressBar();

            if (latestRelease != null)
            {
                var releaseVersion = ConvertReleaseTagVersionToInt(latestRelease.tag_name);

                if (releaseVersion > Constants.ProgramVersion)
                {
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
                    GetMessageBox(Constants.Instance.MessageTitleNoNewRelease, Constants.Instance.MessageBodyNoNewRelease,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                GetMessageBox(Constants.Instance.MessageTitleLatestReleaseUnavailable, Constants.Instance.MessageBodyLatestReleaseUnavailable, 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            saveGuard.Reset();
        }

        private static int ConvertReleaseTagVersionToInt(string tag)
        {
            var tagBits = tag.Split('.');
            var str = new StringBuilder();
            foreach (var bit in tagBits)
                str.Append(bit);

            return Converters.ConvertToInt(str.ToString(), -1);
        }

        private string ExecutableName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase); }
        }

        private string AssemblyExecutableName
        {
            get { return this.ProductName; }
        }

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
        
        public void PasteInput()
        {
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

                ccState.AddNewRequest(ClipBoard);

                UpdateRequestHistory();
                UpdateComboBoxIndex(requestHistory_cmb, 0);
                pasteGuard.Reset();
            }
        }

        public void CopyColumn(bool replace)
        {
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
        }

        public void CopyLine()
        {
            var index = Converters.ConvertToIntWithClamp(copyLineNumber_txt.Text, ccState.History[ccState.CurrentRequest].CopyNextLineIndex, 0, ccState.History[ccState.CurrentRequest].CurrentColumnRowCount - 1);

            if (index != ccState.History[ccState.CurrentRequest].CopyNextLineIndex)
                ccState.History[ccState.CurrentRequest].CopyNextLineIndex = index;

            var text = ccState.History[ccState.CurrentRequest].GetCurrentColumnNextLineText();
            UpdateTextBox(copyLineNumber_txt, ccState.History[ccState.CurrentRequest].CopyNextLineIndex.ToString());
            ClipBoard = text;
        }

        public void ExportRequest()
        {
            ClipBoard = ccState.ExportCurrentRequest();
        }

        public void DeleteRequest()
        {
            ccState.DeleteCurrentRequest();
            UpdateRequestHistory();
            if (requestHistory_cmb.Items.Count > 0)
                requestHistory_cmb.SelectedIndex = 0;
            StateSave();
        }

        public void UpdateLineSeperatorOptions(LineSeperatorOptions option)
        {
            if (checkGuard.CheckSet)
            {
                var sep = string.Empty;
                var pre = string.Empty;
                var post = string.Empty;
                var seperatorOption = 0;
                switch (option)
                {
                    case LineSeperatorOptions.Comma:
                        sep = ", ";
                        pre = "";
                        post = "";
                        seperatorOption = 0;
                        break;
                    case LineSeperatorOptions.DoubleQuoteComma:
                        sep = "\", \"";
                        pre = "";
                        post = "";
                        seperatorOption = 2;
                        break;
                    case LineSeperatorOptions.DoubleQuoteParenthesisComma:
                        sep = "\", \"";
                        pre = "(\"";
                        post = "\")";
                        seperatorOption = 5;
                        break;
                    case LineSeperatorOptions.Nothing:
                        sep = "";
                        pre = "";
                        post = "";
                        seperatorOption = 1;
                        break;
                    case LineSeperatorOptions.ParenthesisComma:
                        sep = ", ";
                        pre = "(";
                        post = ")";
                        seperatorOption = 3;
                        break;
                    case LineSeperatorOptions.SemiColon:
                        sep = ";";
                        pre = "";
                        post = "";
                        seperatorOption = 6;
                        break;
                    case LineSeperatorOptions.SingleQuoteParenthesisComma:
                        sep = "', '";
                        pre = "('";
                        post = "')";
                        seperatorOption = 4;
                        break;
                }

                ccState.LineSeparatorOptionIndex = seperatorOption;

                UpdateComboBoxIndex(seperatorOption_cmb, seperatorOption);
                UpdateTextBox(seperatorItem_txt, sep);
                UpdateTextBox(seperatorItemPre_txt, pre);
                UpdateTextBox(seperatorItemPost_txt, post);

                checkGuard.Reset();
            }
        }

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

        public void ClearHistory()
        {
            ccState.CleanHistory(ccState.HistoryLog.Count, false);
            UpdateRequestHistory();
            StateSave();
        }

        public void StateNew()
        {
            StateSaveAs();
            ClearHistory();
        }

        public void StateOpen(bool guardAlreadySet = false)
        {
            if (!guardAlreadySet) while (!saveGuard.CheckSet) ;

            var loadThread = new Thread(() => StateLoadHelper());

            loadThread.Start();
        }

        public void StateSave(bool guardAlreadySet = false)
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

        private void StateSaveHelper()
        {
            ToggleProgressBar();

            ccState.Save();

            ToggleProgressBar();
            saveGuard.Reset();
        }

        private void StateLoadHelper()
        {
            ToggleProgressBar();

            var result = ccState.Load();
            if (result)
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
            }

            ToggleProgressBar();
            saveGuard.Reset();
        }

        private delegate void UpdateTextBoxDelegate(TextBox textbox, string text);
        private delegate void UpdateLabelDelegate(Label label, string text);
        private delegate void UpdateCheckBoxDelegate(CheckBox checkbox, bool value);
        private delegate void UpdateRadioButtonDelegate(RadioButton radiobutton, bool value);
        private delegate void UpdateComboBoxItemsDelegate(ComboBox comboBox, List<string> values);
        private delegate void UpdateComboBoxIndexDelegate(ComboBox combobox, int index);
        private delegate void UpdateComboBoxTextDelegate(ComboBox combobox, string text);
        private delegate void UpdateMenuItemCheckedDelegate(ToolStripMenuItem menuitem, bool value);
        private delegate void UpdateProgressBar();

        private void UpdateMenuItemChecked(ToolStripMenuItem menuitem, bool value)
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

        private void UpdateLabelText(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                var d = new UpdateLabelDelegate(UpdateLabelText);
                this.Invoke(d, new object[] { label, text });
            }
            else
            {
                label.Text = text;
            }
        }

        private void UpdateComboBoxText(ComboBox combobox, string text)
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

        private void UpdateComboBoxIndex(ComboBox comboBox, int index)
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

        private void UpdateRadioButton(RadioButton radiobutton, bool value)
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

        private void UpdateComboBoxItems(ComboBox comboBox, List<string> values)
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

        private void UpdateTextBox(TextBox textbox, string text)
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

        private void UpdateCheckBox(CheckBox checkbox, bool value)
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

        private void ToggleProgressBar()
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

        public void StateSaveAs()
        {
            while (!saveGuard.CheckSet) ;

            SaveFileDialog fileSelector = new SaveFileDialog();
            fileSelector.DefaultExt = Constants.Instance.SaveExtension;
            fileSelector.Filter = string.Format("Column Copier Save File ({0})|*{0}|Compressed Column Copier Save File ({1})|*{1}",
                                                Constants.Instance.SaveExtension, Constants.Instance.SaveExtensionCompressed);
            fileSelector.InitialDirectory = ExecutableDirectory;

            fileSelector.ShowDialog();
            var file = fileSelector.FileName;
            ccState.SaveFile = file;
            StateSave(true);
        }

        public void OpenWebPage(string url)
        {
            Process.Start(url);
        }

        public void OpenAbout()
        {
            var about = new About();
            about.ShowDialog();
        }

        public void ChangeColumn(int columnIndex)
        {
            ccState.History[ccState.CurrentRequest].SetCurrentColumn(columnIndex);
            UpdateTextBox(currentColumnText_txt, ccState.History[ccState.CurrentRequest].GetCurrentColumnText());

            UpdateLabelText(statCurrentColumn_txt, string.Format(Constants.Instance.FormatStatCurrentColumn,
                ccState.History[ccState.CurrentRequest].CurrentColumnIndex + 1,
                ccState.History[ccState.CurrentRequest].CurrentColumnIndex));
            UpdateLabelText(statNumberColumns_txt, string.Format(Constants.Instance.FormatStatNumberColumns,
                ccState.History[ccState.CurrentRequest].NumberOfColumns));
            UpdateLabelText(statNumberRows_txt, string.Format(Constants.Instance.FormatStatNumberRows,
                ccState.History[ccState.CurrentRequest].GetColumnRawText().Count));

            StateSave();
        }

        public void ChangeRequest(int requestIndex)
        {
            ccState.CurrentRequest = ccState.HistoryLog[ccState.GetRequestHistoryPosition(requestHistory_cmb.Items[requestIndex].ToString())];

            UpdateComboBoxItems(currentColumn_cmb, ccState.CurrentRequestColumnNames());
            UpdateComboBoxIndex(currentColumn_cmb, ccState.History[ccState.CurrentRequest].CurrentColumnIndex);
        }

        private void paste_btn_Click(object sender, EventArgs e)
        {
            PasteInput();
        }

        private void pasteAndCopy_btn_Click(object sender, EventArgs e)
        {
            PasteInput();
            CopyColumn(false);
        }

        private void copyColumn_btn_Click(object sender, EventArgs e)
        {
            CopyColumn(false);
        }

        private void copyNextLine_btn_Click(object sender, EventArgs e)
        {
            CopyLine();
        }

        private void copyLineWithSeperators_btn_Click(object sender, EventArgs e)
        {
            CopyColumn(true);
        }

        private void exportRequest_btn_Click(object sender, EventArgs e)
        {
            ExportRequest();
        }

        private void deleteRequest_btn_Click(object sender, EventArgs e)
        {
            DeleteRequest();
        }

        private void seperatorOption_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions((LineSeperatorOptions)seperatorOption_cmb.SelectedIndex);
        }

        private void clearHistory_btn_Click(object sender, EventArgs e)
        {
            ClearHistory();
        }

        private void stateNew_btn_Click(object sender, EventArgs e)
        {
            StateNew();
        }

        public void StateLoad()
        {
            var fileSelector = new OpenFileDialog();
            fileSelector.DefaultExt = Constants.Instance.SaveExtension;
            fileSelector.Filter = string.Format("Column Copier Save File ({0})|*{0}|Compressed Column Copier Save File ({1})|*{1}",
                                                Constants.Instance.SaveExtension, Constants.Instance.SaveExtensionCompressed);
            fileSelector.InitialDirectory = ExecutableDirectory;

            fileSelector.ShowDialog();
            var file = fileSelector.FileName;
            ccState.SaveFile = file;

            StateOpen();
        }

        private void stateOpen_btn_Click(object sender, EventArgs e)
        {
            StateLoad();
        }

        private void stateSave_btn_Click(object sender, EventArgs e)
        {
            StateSave();
        }
        private string ExecutableDirectory
        {
            get
            {
                var dirName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

                return dirName.Remove(0, 6);
            }
        }

        private void stateSaveAs_btn_Click(object sender, EventArgs e)
        {
            StateSaveAs();
        }

        private void help_btn_Click(object sender, EventArgs e)
        {
            OpenWebPage(Constants.Instance.UrlHelp);
        }

        private void about_btn_Click(object sender, EventArgs e)
        {
            OpenAbout();
        }

        private void currentColumn_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeColumn(currentColumn_cmb.SelectedIndex);
        }

        private void requestHistory_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeRequest(requestHistory_cmb.SelectedIndex);
        }

        private void showOnTop_cxb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleShowOnTop(showOnTop_cxb.Checked);
        }

        private void fileNew_itm_Click(object sender, EventArgs e)
        {
            stateNew_btn_Click(sender, e);
        }

        private void fileOpen_itm_Click(object sender, EventArgs e)
        {
            stateOpen_btn_Click(sender, e);
        }

        private void fileSave_itm_Click(object sender, EventArgs e)
        {
            stateSave_btn_Click(sender, e);
        }

        private void fileSaveAs_itm_Click(object sender, EventArgs e)
        {
            stateSaveAs_btn_Click(sender, e);
        }

        private void fileSettingsShowOnTop_itm_Click(object sender, EventArgs e)
        {
            ToggleShowOnTop(!showOnTop_cxb.Checked);
        }

        private void fileExit_itm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void inputPaste_itm_Click(object sender, EventArgs e)
        {
            PasteInput();
        }

        private void inputPasteAndCopy_itm_Click(object sender, EventArgs e)
        {
            PasteInput();
            CopyColumn(false);
        }

        private void inputSettingsRemoveBlanks_itm_Click(object sender, EventArgs e)
        {
            ToggleCleanInputText(!removeBlankLines_cxb.Checked);
        }

        private void inputSettingsDataHasHeaders_itm_Click(object sender, EventArgs e)
        {
            ToggleDataHasHeaders(!dataHasHeaders_cxb.Checked);
        }

        private void inputSettingsCleanInputText_itm_Click(object sender, EventArgs e)
        {
            ToggleRemoveBlankLines(!cleanInputText_cxb.Checked);
        }

        private void outputCopyColumn_itm_Click(object sender, EventArgs e)
        {
            CopyColumn(false);
        }

        private void outputCopyNextLine_itm_Click(object sender, EventArgs e)
        {
            CopyLine();
        }

        private void outputCopyLineWithSeperator_itm_Click(object sender, EventArgs e)
        {
            CopyColumn(true);
        }

        private void outputExportRequest_itm_Click(object sender, EventArgs e)
        {
            ExportRequest();
        }

        private void outputSettingsCurrentCopyNextLineLine_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(copyLineNumber_txt, GetTextboxInputResults(Constants.Instance.InputQueryNextLineCopyLine, copyLineNumber_txt.Text));
        }

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

        private void outputSettingsLineReplacementSeperator_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(seperatorItem_txt, GetTextboxInputResults(Constants.Instance.InputQueryLineReplacementSeparator, seperatorItem_txt.Text));
        }

        private void outputSettingsLineReplacementPreString_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(seperatorItemPre_txt, GetTextboxInputResults(Constants.Instance.InputQueryLineReplacementPre, seperatorItemPre_txt.Text));
        }

        private void outputSettingsLineReplacementPostString_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(seperatorItemPost_txt, GetTextboxInputResults(Constants.Instance.InputQueryLineReplacementPost, seperatorItemPost_txt.Text));
        }

        private void outputSettingLineReplacementPresetBlank_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeperatorOptions.Nothing);
        }

        private void outputSettingLineReplacementPresetComma_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeperatorOptions.Comma);
        }

        private void outputSettingLineReplacementPresetDoubleQuoteComma_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeperatorOptions.DoubleQuoteComma);
        }

        private void outputSettingLineReplacementPresetSqlComma_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeperatorOptions.ParenthesisComma);
        }

        private void outputSettingLineReplacementPresetSqlText_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeperatorOptions.SingleQuoteParenthesisComma);
        }

        private void outputSettingLineReplacementPresetParenthesisQuotes_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeperatorOptions.DoubleQuoteParenthesisComma);
        }

        private void outputSettingLineReplacementPresetSemiColan_itm_Click(object sender, EventArgs e)
        {
            UpdateLineSeperatorOptions(LineSeperatorOptions.SemiColon);
        }

        private void historyDeleteRequest_itm_Click(object sender, EventArgs e)
        {
            DeleteRequest();
        }

        private void historySettingsMaxRequestHistory_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(maxHistory_txt, GetTextboxInputResults(Constants.Instance.InputQueryMaxHistory, maxHistory_txt.Text));
        }

        private void helpDocumentation_itm_Click(object sender, EventArgs e)
        {
            OpenWebPage(Constants.Instance.UrlHelp);
        }

        private void helpAbout_itm_Click(object sender, EventArgs e)
        {
            OpenAbout();
        }

        private void helpSupport_itm_Click(object sender, EventArgs e)
        {
            OpenWebPage(Constants.Instance.UrlSupport);
        }

        private void removeBlankLines_cxb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleRemoveBlankLines(removeBlankLines_cxb.Checked);
        }

        private void dataHasHeaders_cxb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleDataHasHeaders(dataHasHeaders_cxb.Checked);
        }

        private void cleanInputText_cxb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleRemoveBlankLines(cleanInputText_cxb.Checked);
        }

        public void ToggleRemoveBlankLines(bool? set = null)
        {
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
            }
        }

        public void ToggleDataHasHeaders(bool? set = null)
        {
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
            }
        }

        public void ToggleCleanInputText(bool? set = null)
        {
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
            }
        }

        public void ToggleShowOnTop(bool? set = null)
        {
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
            }
        }

        private void historySettingsPreserveCurrentRequest_itm_Click(object sender, EventArgs e)
        {
            PreserveCurrentRequest(!preserveCurrentRequest_cxb.Checked);
        }

        private void preserveCurrentRequest_cxb_CheckedChanged(object sender, EventArgs e)
        {
            PreserveCurrentRequest(preserveCurrentRequest_cxb.Checked);
        }

        public void PreserveCurrentRequest(bool? set = null)
        {
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
            }
        }

        private void historyClearHistory_itm_Click(object sender, EventArgs e)
        {
            ClearHistory();
        }

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

        private void defaultPriorityNumber_rbn_CheckedChanged(object sender, EventArgs e)
        {
            if (checkGuard.CheckSet)
            {
                SetDefaultPriorityToggles(DefaultColumnPriority.Number);
                StateSave();
                checkGuard.Reset();
            }
        }

        private void defaultPriorityName_rbn_CheckedChanged(object sender, EventArgs e)
        {
            if (checkGuard.CheckSet)
            {
                SetDefaultPriorityToggles(DefaultColumnPriority.Name);
                StateSave();
                checkGuard.Reset();
            }
        }

        private void inputSettingsDefaultColumnNumber_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(defaultColumnNumber_txt, GetTextboxInputResults(Constants.Instance.InputQueryDefaultColumnNumber, defaultColumnNumber_txt.Text));
        }

        private void inputSettingsDefaultColumnName_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(defaultColumnName_txt, GetTextboxInputResults(Constants.Instance.InputQueryDefaultColumnName, defaultColumnName_txt.Text));
        }

        private void inputSettingsDefaultPriorityNumber_itm_Click(object sender, EventArgs e)
        {
            if (checkGuard.CheckSet)
            {
                SetDefaultPriorityToggles(DefaultColumnPriority.Number);
                StateSave();
                checkGuard.Reset();
            }
        }

        private void inputSettingsDefaultPriorityName_itm_Click(object sender, EventArgs e)
        {
            if (checkGuard.CheckSet)
            {
                SetDefaultPriorityToggles(DefaultColumnPriority.Name);
                StateSave();
                checkGuard.Reset();
            }
        }

        private void inputSettingsDefaultPriorityNameSimilarity_itm_Click(object sender, EventArgs e)
        {
            UpdateTextBox(defaultPriorityNameSimilarity_txt, GetTextboxInputResults(Constants.Instance.InputQueryNameSimilarityValue, defaultPriorityNameSimilarity_txt.Text));
        }

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

        private DialogResult GetMessageBox(string title, string question, MessageBoxButtons buttonsType = MessageBoxButtons.YesNo, MessageBoxIcon iconType = MessageBoxIcon.Question)
        {
            return MessageBox.Show(question, title, buttonsType, iconType);
        }

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

        private void preSetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var inputItems = new List<string>();
            for (var i = 0; i < seperatorOption_cmb.Items.Count; i++)
                inputItems.Add(seperatorOption_cmb.Items[i].ToString());

            UpdateComboBoxIndex(seperatorOption_cmb, GetComboboxInputResults(Constants.Instance.InputQueryChangeHistoryRequest,
                                                        inputItems, seperatorOption_cmb.SelectedIndex));
        }

        private void seperatorItemPre_txt_TextChanged(object sender, EventArgs e)
        {
            ccState.LineSeparatorOptionPre = seperatorItemPre_txt.Text;
        }

        private void seperatorItem_txt_TextChanged(object sender, EventArgs e)
        {
            ccState.LineSeparatorOptionInter = seperatorItem_txt.Text;
        }

        private void seperatorItemPost_txt_TextChanged(object sender, EventArgs e)
        {
            ccState.LineSeparatorOptionPost = seperatorItemPost_txt.Text;
        }

        private void helpUpdateCheck_itm_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void fileSettingsCompressSave_itm_Click(object sender, EventArgs e)
        {
            ToggleSaveCompression();
        }

        public void ToggleSaveCompression()
        {
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
            }
        }

        public void ChangeOpacity()
        {
            ChangeFormOpacity(this, ccState.ProgramOpacity);
        }

        public void ChangeFormOpacity(Form form, int value)
        {
            if (form.InvokeRequired)
            {
                var d = new ChangeFormOpacityDelegate(ChangeFormOpacity);
                this.Invoke(d, new object[] { form, value });
            }
            else
            {
                form.Opacity = (value / 100d);
            }
        }
        private delegate void ChangeFormOpacityDelegate(Form form, int value);

        private void fileSettingsProgramOpacity_itm_Click(object sender, EventArgs e)
        {
            var opacity = GetTextboxInputResults(Constants.Instance.InputQueryProgramOpacity, ccState.ProgramOpacity.ToString());
            ccState.ProgramOpacity = Converters.ConvertToIntWithClamp(opacity, 100, 0, 100);
            ChangeOpacity();
            StateSave();
        }
    }
}