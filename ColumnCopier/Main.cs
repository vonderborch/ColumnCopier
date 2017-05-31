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

        public Main()
        {
            InitializeComponent();

            ccState = new CCState();
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

        public void AUpdateInterface()
        {

        }
        
        public void PasteInput()
        {
            ccState.AddNewRequest(ClipBoard);
        }

        public void CopyColumn(bool replace)
        {
        }

        public void CopyLine()
        {
        }

        public void ExportRequest()
        {
        }

        public void DeleteRequest()
        {
        }

        public void UpdateLineSeperatorOptions(LineSeperatorOptions option)
        {
        }

        public void ClearHistory()
        {
        }

        public void PreserveCurrentRequest()
        {

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

        public void OpenHelp()
        {

        }

        public void OpenAbout()
        {

        }

        public void ChangeColumn(int columnIndex)
        {

        }

        public void ChangeRequest(int requestIndex)
        {

        }

        public void ToggleShowOnTop()
        {

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

        private void preserveCurrentRequest_cxb_CheckedChanged(object sender, EventArgs e)
        {
            PreserveCurrentRequest();
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
            OpenHelp();
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
            ToggleShowOnTop();
        }
    }
}