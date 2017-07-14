// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : InputSqlWizard.cs
// Author           : Christian
// Created          : 07-14-2017
// 
// Version          : 2.2.0
// Last Modified By : Christian
// Last Modified On : 07-14-2017
// ***********************************************************************
// <copyright file="InputSqlWizard.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the InputSqlWizard form.
// </summary>
//
// Changelog: 
//            - 2.2.0 (07-14-2017) - Initial version created.
// ***********************************************************************
using ColumnCopier.Enums;
using System;
using System.Windows.Forms;

namespace ColumnCopier
{
    /// <summary>
    /// Class InputSqlWizard.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class InputSqlWizard : Form
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InputSqlWizard"/> class.
        /// </summary>
        public InputSqlWizard()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the connection text.
        /// </summary>
        /// <value>The connection text.</value>
        public string ConnectionText
        {
            get { return conn_txt.Text; }
            set { conn_txt.Text = value; }
        }

        /// <summary>
        /// Gets or sets the query text.
        /// </summary>
        /// <value>The query text.</value>
        public string QueryText

        {
            get { return query_txt.Text; }
            set { query_txt.Text = value; }
        }

        /// <summary>
        /// Gets or sets the SQL provider.
        /// </summary>
        /// <value>The SQL provider.</value>
        public SqlConnectionProviders SqlProvider
        {
            get
            {
                return providerNone_rbn.Checked
                    ? SqlConnectionProviders.None
                    : providerMySql_rbn.Checked
                        ? SqlConnectionProviders.MySql
                        : providerPostgreSql_rbn.Checked
                            ? SqlConnectionProviders.PostgreSQL
                            : SqlConnectionProviders.SqlServer;
            }
            set
            {
                providerNone_rbn.Checked = value == SqlConnectionProviders.None;
                providerMySql_rbn.Checked = value == SqlConnectionProviders.MySql;
                providerPostgreSql_rbn.Checked = value == SqlConnectionProviders.PostgreSQL;
                providerSqlServer_rbn.Checked = value == SqlConnectionProviders.SqlServer;
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