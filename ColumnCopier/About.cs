// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : About.cs
// Author           : Christian
// Created          : 08-22-2016
// 
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-01-2017
// ***********************************************************************
// <copyright file="About.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The About/Help form.
// </summary>
//
// Changelog: 
///           - 2.0.0 (06-01-2017) - Help URL is now defined in the Constants.
//            - 1.0.0 (08-22-2016) - Initial version created.
// ***********************************************************************
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ColumnCopier
{
    /// <summary>
    /// About form class.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    partial class About : Form
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>
        public About()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
        }

        #endregion Public Constructors

        #region Assembly Attribute Accessors

        /// <summary>
        /// Gets the assembly company.
        /// </summary>
        /// <value>The assembly company.</value>
        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        /// <summary>
        /// Gets the assembly copyright.
        /// </summary>
        /// <value>The assembly copyright.</value>
        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        /// <summary>
        /// Gets the assembly description.
        /// </summary>
        /// <value>The assembly description.</value>
        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        /// <summary>
        /// Gets the assembly product.
        /// </summary>
        /// <value>The assembly product.</value>
        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        /// <summary>
        /// Gets the assembly title.
        /// </summary>
        /// <value>The assembly title.</value>
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        /// <value>The assembly version.</value>
        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        #endregion Assembly Attribute Accessors

        #region Private Methods

        /// <summary>
        /// Handles the Click event of the help_btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-01-2017) - Help URL is now defined in the Constants.
        ///             - 1.0.0 (08-22-2016) - Initial version.
        private void help_btn_Click(object sender, EventArgs e)
        {
            // Open the github site in the user's default browser.
            System.Diagnostics.Process.Start(Constants.Instance.UrlHelp);
        }

        #endregion Private Methods
    }
}