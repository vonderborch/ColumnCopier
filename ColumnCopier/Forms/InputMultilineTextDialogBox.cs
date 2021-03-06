﻿// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : InputMultilineTextDialogBox.cs
// Author           : Christian
// Created          : 07-14-2017
// 
// Version          : 2.2.0
// Last Modified By : Christian
// Last Modified On : 07-14-2017
// ***********************************************************************
// <copyright file="InputMultilineTextDialogBox.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the InputMultilineTextDialogBox form.
// </summary>
//
// Changelog: 
//            - 2.2.0 (07-14-2017) - Initial version created.
// ***********************************************************************
using System;
using System.Windows.Forms;

namespace ColumnCopier
{
    /// <summary>
    /// Class InputMultilineTextDialogBox.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class InputMultilineTextDialogBox : Form
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InputMultilineTextDialogBox"/> class.
        /// </summary>
        public InputMultilineTextDialogBox()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the input text.
        /// </summary>
        /// <value>The input text.</value>
        public string InputText
        {
            get { return input_txt.Text; }
            set { input_txt.Text = value; }
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

        #region Private Methods

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
        /// Handles the Click event of the ok_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void ok_btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        #endregion Private Methods
    }
}