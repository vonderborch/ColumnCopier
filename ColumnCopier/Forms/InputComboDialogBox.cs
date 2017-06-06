// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : InputComboDialogBox.cs
// Author           : Christian
// Created          : 06-06-2017
// 
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="InputComboDialogBox.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the InputComboDialogBox class.
// </summary>
//
// Changelog: 
//            - 2.0.0 (06-06-2017) - Initial version created.
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ColumnCopier
{
    /// <summary>
    /// Class InputComboDialogBox.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class InputComboDialogBox : Form
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InputComboDialogBox"/> class.
        /// </summary>
        public InputComboDialogBox()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the input selected item.
        /// </summary>
        /// <value>The input selected item.</value>
        public int InputSelectedItem
        {
            get { return input_cmb.SelectedIndex; }
            set { input_cmb.SelectedIndex = value; }
        }

        /// <summary>
        /// Gets the input text.
        /// </summary>
        /// <value>The input text.</value>
        public string InputText
        {
            get { return input_cmb.Items[input_cmb.SelectedIndex].ToString(); }
        }

        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        /// <value>The question text.</value>
        public string QuestionText
        {
            get { return this.Text; }
            set
            {
                this.Text = value;
                question_txt.Text = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sets the input list items.
        /// </summary>
        /// <param name="items">The items.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void SetInputListItems(List<string> items)
        {
            input_cmb.Items.Clear();
            for (var i = 0; i < items.Count; i++)
                input_cmb.Items.Add(items[i]);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Handles the Click event of the cancel_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void cancel_btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Handles the Click event of the ok_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        private void ok_btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        #endregion Private Methods
    }
}