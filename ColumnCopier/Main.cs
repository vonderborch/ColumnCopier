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
                preserveCurrentRequest_cxb.Checked = false;
                historySettingsPreserveCurrentRequest_itm.Checked = false;
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
                requestHistory_cmb.SelectedIndex = 0;
                pasteGuard.Reset();
            }
        }

        public void CopyColumn(bool replace)
        {
            var text = string.Empty;

            if (replace)
            {
                var pre = seperatorItemPre_txt.Text;
                var post = seperatorItemPost_txt.Text;
                var intr = seperatorItem_txt.Text;
                var lines = ccState.History[ccState.CurrentRequest].GetColumnRawText();

                var str = new StringBuilder();
                str.Append(pre);

                var last = lines.Count - 1;
                for (var i = 0; i < lines.Count; i++)
                    str.AppendFormat("{0}{1}", lines[i], i == last ? "" : intr);

                str.Append(post);
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
            copyLineNumber_txt.Text = ccState.History[ccState.CurrentRequest].CopyNextLineIndex.ToString();
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
        }

        private void UpdateRequestHistory()
        {
            requestHistory_cmb.Items.Clear();
            foreach (var request in ccState.GetRequestHistory())
                requestHistory_cmb.Items.Add(request);

            currentColumnText_txt.Text = string.Empty;
            currentColumn_cmb.Items.Clear();

            currentColumn_cmb.Text = string.Empty;
            requestHistory_cmb.Text = string.Empty;
            statCurrentColumn_txt.Text = string.Format(Constants.Instance.FormatStatCurrentColumn, string.Empty, string.Empty);
            statNumberColumns_txt.Text = string.Format(Constants.Instance.FormatStatNumberColumns, string.Empty);
            statNumberRows_txt.Text = string.Format(Constants.Instance.FormatStatNumberRows, string.Empty);
        }

        public void ClearHistory()
        {
            ccState.CleanHistory(ccState.HistoryLog.Count, false);
            UpdateRequestHistory();
            StateSave();
        }

        public void StateNew()
        {

        }

        public void StateOpen()
        {

        }

        public void StateSave()
        {

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
            currentColumnText_txt.Text = ccState.History[ccState.CurrentRequest].GetCurrentColumnText();
            
            statCurrentColumn_txt.Text = string.Format(Constants.Instance.FormatStatCurrentColumn,
                ccState.History[ccState.CurrentRequest].CurrentColumnIndex + 1,
                ccState.History[ccState.CurrentRequest].CurrentColumnIndex);
            statNumberColumns_txt.Text = string.Format(Constants.Instance.FormatStatNumberColumns,
                ccState.History[ccState.CurrentRequest].NumberOfColumns);
            statNumberRows_txt.Text = string.Format(Constants.Instance.FormatStatNumberRows,
                ccState.History[ccState.CurrentRequest].GetColumnRawText().Count);

            StateSave();
        }

        public void ChangeRequest(int requestIndex)
        {
            ccState.CurrentRequest = ccState.HistoryLog[ccState.GetRequestHistoryPosition(requestHistory_cmb.Items[requestIndex].ToString())];

            currentColumn_cmb.Items.Clear();
            foreach (var column in ccState.CurrentRequestColumnNames())
                currentColumn_cmb.Items.Add(column);
            
            currentColumn_cmb.SelectedIndex = ccState.History[ccState.CurrentRequest].CurrentColumnIndex;
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

        private void stateOpen_btn_Click(object sender, EventArgs e)
        {
            StateOpen();
        }

        private void stateSave_btn_Click(object sender, EventArgs e)
        {
            StateSave();
        }

        private void stateSaveAs_btn_Click(object sender, EventArgs e)
        {
            // Insert code to change the save file/location HERE

            StateSave();
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

        }

        private void fileOpen_itm_Click(object sender, EventArgs e)
        {

        }

        private void fileClear_itm_Click(object sender, EventArgs e)
        {

        }

        private void fileSave_itm_Click(object sender, EventArgs e)
        {

        }

        private void fileSaveAs_itm_Click(object sender, EventArgs e)
        {

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

        private void inputSettingsDefaultColumnNumber_itm_Click(object sender, EventArgs e)
        {

        }

        private void inputSettingsDefaultColumnName_itm_Click(object sender, EventArgs e)
        {

        }

        private void inputSettingsDefaultPriorityNumber_itm_Click(object sender, EventArgs e)
        {

        }

        private void inputSettingsDefaultPriorityName_itm_Click(object sender, EventArgs e)
        {

        }

        private void inputSettingsDefaultPriorityNameSimilarity_itm_Click(object sender, EventArgs e)
        {

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

        }

        private void outputSettingsLineReplacementSeperator_itm_Click(object sender, EventArgs e)
        {

        }

        private void outputSettingsLineReplacementPreString_itm_Click(object sender, EventArgs e)
        {

        }

        private void outputSettingsLineReplacementPostString_itm_Click(object sender, EventArgs e)
        {

        }

        private void outputSettingLineReplacementPresetBlank_itm_Click(object sender, EventArgs e)
        {

        }

        private void outputSettingLineReplacementPresetComma_itm_Click(object sender, EventArgs e)
        {

        }

        private void outputSettingLineReplacementPresetDoubleQuoteComma_itm_Click(object sender, EventArgs e)
        {

        }

        private void outputSettingLineReplacementPresetSqlComma_itm_Click(object sender, EventArgs e)
        {

        }

        private void outputSettingLineReplacementPresetSqlText_itm_Click(object sender, EventArgs e)
        {

        }

        private void outputSettingLineReplacementPresetParenthesisQuotes_itm_Click(object sender, EventArgs e)
        {

        }

        private void outputSettingLineReplacementPresetSemiColan_itm_Click(object sender, EventArgs e)
        {

        }

        private void historyDeleteRequest_itm_Click(object sender, EventArgs e)
        {
            DeleteRequest();
        }

        private void historyChangeRequest_itm_Click(object sender, EventArgs e)
        {

        }

        private void historySettingsMaxRequestHistory_itm_Click(object sender, EventArgs e)
        {

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
                    removeBlankLines_cxb.Checked = !removeBlankLines_cxb.Checked;
                    inputSettingsRemoveBlanks_itm.Checked = !inputSettingsRemoveBlanks_itm.Checked;
                }
                else
                {
                    removeBlankLines_cxb.Checked = (bool)set;
                    inputSettingsRemoveBlanks_itm.Checked = (bool)set;
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
                    dataHasHeaders_cxb.Checked = !dataHasHeaders_cxb.Checked;
                    inputSettingsDataHasHeaders_itm.Checked = !inputSettingsDataHasHeaders_itm.Checked;
                }
                else
                {
                    dataHasHeaders_cxb.Checked = (bool)set;
                    inputSettingsDataHasHeaders_itm.Checked = (bool)set;
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
                    cleanInputText_cxb.Checked = !cleanInputText_cxb.Checked;
                    inputSettingsCleanInputText_itm.Checked = !inputSettingsCleanInputText_itm.Checked;
                }
                else
                {
                    cleanInputText_cxb.Checked = (bool)set;
                    inputSettingsCleanInputText_itm.Checked = (bool)set;
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
                    showOnTop_cxb.Checked = !showOnTop_cxb.Checked;
                    fileSettingsShowOnTop_itm.Checked = !fileSettingsShowOnTop_itm.Checked;
                }
                else
                {
                    showOnTop_cxb.Checked = (bool)set;
                    fileSettingsShowOnTop_itm.Checked = (bool)set;
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
                    preserveCurrentRequest_cxb.Checked = !preserveCurrentRequest_cxb.Checked;
                    historySettingsPreserveCurrentRequest_itm.Checked = !historySettingsPreserveCurrentRequest_itm.Checked;
                }
                else
                {
                    preserveCurrentRequest_cxb.Checked = (bool)set;
                    historySettingsPreserveCurrentRequest_itm.Checked = (bool)set;
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
    }
}