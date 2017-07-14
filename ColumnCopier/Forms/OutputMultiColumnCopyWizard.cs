// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : OutputMultiColumnCopyWizard.cs
// Author           : Christian
// Created          : 07-14-2017
//
// Version          : 2.2.0
// Last Modified By : Christian
// Last Modified On : 07-14-2017
// ***********************************************************************
// <copyright file="OutputMultiColumnCopyWizard.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the OutputMultiColumnCopyWizard form.
// </summary>
//
// Changelog:
//            - 2.2.0 (07-14-2017) - Initial version created.
// ***********************************************************************
using ColumnCopier.Classes;
using ColumnCopier.Enums;
using ColumnCopier.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace ColumnCopier
{
    /// <summary>
    /// Class OutputMultiColumnCopyWizard.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class OutputMultiColumnCopyWizard : Form
    {
        #region Private Fields

        /// <summary>
        /// The check guard
        /// </summary>
        private Guard checkGuard;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputMultiColumnCopyWizard"/> class.
        /// </summary>
        public OutputMultiColumnCopyWizard()
        {
            InitializeComponent();
            checkGuard = new Guard();
        }

        #endregion Public Constructors

        #region Private Delegates

        /// <summary>
        /// Delegate UpdateComboBoxIndexDelegate
        /// </summary>
        /// <param name="combobox">The combobox.</param>
        /// <param name="index">The index.</param>
        private delegate void UpdateComboBoxIndexDelegate(ComboBox combobox, int index);

        /// <summary>
        /// Delegate UpdateTextBoxDelegate
        /// </summary>
        /// <param name="textbox">The textbox.</param>
        /// <param name="text">The text.</param>
        private delegate void UpdateTextBoxDelegate(TextBox textbox, string text);

        #endregion Private Delegates

        #region Public Properties

        /// <summary>
        /// Gets or sets the available columns.
        /// </summary>
        /// <value>The available columns.</value>
        public List<string> AvailableColumns
        {
            get
            {
                var tmp = new List<string>();
                for (var i = 0; i < availableColumns_lst.Items.Count; i++)
                    tmp.Add(availableColumns_lst.Items[i].ToString());
                return tmp;
            }
            set
            {
                availableColumns_lst.Items.Clear();
                for (var i = 0; i < value.Count; i++)
                    availableColumns_lst.Items.Add(value[i]);
            }
        }

        /// <summary>
        /// Gets or sets the request data.
        /// </summary>
        /// <value>The request data.</value>
        public Request RequestData { get; set; }

        /// <summary>
        /// Gets or sets the result text.
        /// </summary>
        /// <value>The result text.</value>
        public string ResultText { get; set; } = string.Empty;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Opens the web page.
        /// </summary>
        /// <param name="url">The URL.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public void OpenWebPage(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Sets the result text.
        /// </summary>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public void SetResultText()
        {
            if (RequestData != null)
            {
                if (selectedColumns_lst.Items.Count > 0)
                {
                    var str = new StringBuilder();
                    var columnSep = seperatorItem_txt.Text;
                    var columnPre = seperatorItemPre_txt.Text;
                    var columnPost = seperatorItemPost_txt.Text;
                    var lineSep = lineSeperatorItem_txt.Text;

                    if (lineSep == "\\n")
                        lineSep = Environment.NewLine;

                    if (includeColumnHeaders_cxb.Checked)
                    {
                        str.Append($"{columnPre}");
                        for (var i = 0; i < selectedColumns_lst.Items.Count; i++)
                            str.Append($"{selectedColumns_lst.Items[i].ToString()}{columnSep}");

                        str.Append($"{columnPost}{lineSep}");
                    }

                    var rowCount = RequestData.CurrentColumnRowCount;
                    for (var i = 0; i < rowCount; i++)
                    {
                        str.Append($"{columnPre}");

                        var rowStr = new StringBuilder();
                        for (var j = 0; j < selectedColumns_lst.Items.Count; j++)
                            rowStr.Append($"{XmlTextHelpers.ConvertFromXml(RequestData.GetColumnRowText(XmlTextHelpers.ConvertForXml(selectedColumns_lst.Items[j].ToString()), i))}{columnSep}");

                        rowStr.Remove(rowStr.Length - columnSep.Length, columnSep.Length);

                        str.Append($"{rowStr.ToString()}{columnPost}{lineSep}");
                    }

                    ResultText = str.ToString();
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Handles the SelectedIndexChanged event of the availableColumns_lst control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void availableColumns_lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (availableColumns_lst.SelectedIndex != -1)
            {
                var index = XmlTextHelpers.ConvertForXml(availableColumns_lst.Items[availableColumns_lst.SelectedIndex].ToString());
                currentColumnText_txt.Text = RequestData.GetColumnText(index);
            }
            else
            {
                currentColumnText_txt.Text = string.Empty;
            }
        }

        /// <summary>
        /// Handles the Click event of the cancel_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void cancel_btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Handles the Click event of the columnDown_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void columnDown_btn_Click(object sender, EventArgs e)
        {
            if (selectedColumns_lst.SelectedIndex > -1)
            {
                var index = selectedColumns_lst.SelectedIndex;

                if (index < selectedColumns_lst.Items.Count - 1)
                {
                    var item = selectedColumns_lst.Items[index];

                    selectedColumns_lst.Items.RemoveAt(index);
                    selectedColumns_lst.Items.Insert(index + 1, item);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the columnUp_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void columnUp_btn_Click(object sender, EventArgs e)
        {
            if (selectedColumns_lst.SelectedIndex > -1)
            {
                var index = selectedColumns_lst.SelectedIndex;

                if (index > 0)
                {
                    var item = selectedColumns_lst.Items[index];

                    selectedColumns_lst.Items.RemoveAt(index);
                    selectedColumns_lst.Items.Insert(index - 1, item);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the deselect_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void deselect_btn_Click(object sender, EventArgs e)
        {
            if (selectedColumns_lst.SelectedIndex > -1)
            {
                var item = selectedColumns_lst.Items[selectedColumns_lst.SelectedIndex];

                availableColumns_lst.Items.Add(item);
                selectedColumns_lst.Items.RemoveAt(selectedColumns_lst.SelectedIndex);
            }
        }

        /// <summary>
        /// Forms the exception issue URL.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private string FormExceptionIssueUrl(Exception ex)
        {
            return string.Format("{0}/new?title={1}&body={2)", Constants.Instance.UrlSupport, ex.Message.ToString(), ex.InnerException.ToString());
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
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private DialogResult GetMessageBox(string title, string question, MessageBoxButtons buttonsType = MessageBoxButtons.YesNo, MessageBoxIcon iconType = MessageBoxIcon.Question)
        {
            return MessageBox.Show(question, title, buttonsType, iconType);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the lineSeperatorOption_cmb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void lineSeperatorOption_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkGuard.CheckSet)
            {
                var choice = Constants.Instance.ColumnLineSeperators[(ColumnLineSeparatorOptions)lineSeperatorOption_cmb.SelectedIndex];

                UpdateComboBoxIndex(lineSeperatorOption_cmb, choice.Item2);
                UpdateTextBox(lineSeperatorItem_txt, choice.Item1);

                checkGuard.Reset();
            }
        }

        /// <summary>
        /// Handles the Click event of the ok_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void ok_btn_Click(object sender, EventArgs e)
        {
            SetResultText();
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Handles the Click event of the select_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void select_btn_Click(object sender, EventArgs e)
        {
            if (availableColumns_lst.SelectedIndex > -1)
            {
                var item = availableColumns_lst.Items[availableColumns_lst.SelectedIndex];

                selectedColumns_lst.Items.Add(item);
                availableColumns_lst.Items.RemoveAt(availableColumns_lst.SelectedIndex);
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the selectedColumns_lst control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void selectedColumns_lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedColumns_lst.SelectedIndex != -1)
            {
                var index = XmlTextHelpers.ConvertForXml(selectedColumns_lst.Items[selectedColumns_lst.SelectedIndex].ToString());
                currentSelectedColumn_txt.Text = RequestData.GetColumnText(index);
            }
            else
            {
                currentSelectedColumn_txt.Text = string.Empty;
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the seperatorOption_cmb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void seperatorOption_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkGuard.CheckSet)
            {
                var choice = Constants.Instance.LineSeperators[(LineSeparatorOptions)seperatorOption_cmb.SelectedIndex];

                UpdateComboBoxIndex(seperatorOption_cmb, choice.Item4);
                UpdateTextBox(seperatorItem_txt, choice.Item1);
                UpdateTextBox(seperatorItemPre_txt, choice.Item2);
                UpdateTextBox(seperatorItemPost_txt, choice.Item3);

                checkGuard.Reset();
            }
        }

        /// <summary>
        /// Updates the index of the ComboBox.
        /// </summary>
        /// <param name="comboBox">The combo box.</param>
        /// <param name="index">The index.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
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
                var result = GetMessageBox(Constants.Instance.MessageTitleException,
                    string.Format(Constants.Instance.MessageBodyException, ex.ToString()),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    OpenWebPage(FormExceptionIssueUrl(ex));
            }
        }

        /// <summary>
        /// Updates the text box.
        /// </summary>
        /// <param name="textbox">The textbox.</param>
        /// <param name="text">The text.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
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