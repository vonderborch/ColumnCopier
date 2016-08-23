// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Program.cs
// Author           : Christian
// Created          : 08-15-2016
// 
// Version          : 1.0.0
// Last Modified By : Christian
// Last Modified On : 08-18-2016
// ***********************************************************************
// <copyright file="Program.cs" company="Christian Webber">
//		Copyright ©  2016
// </copyright>
// <summary>
// </summary>
//
// Changelog: 
//            - 1.0.0 (08-15-2016) - Initial version created.
// ***********************************************************************
using System;
using System.Windows.Forms;

namespace ColumnCopier
{
    /// <summary>
    /// Program class.
    /// </summary>
    internal static class Program
    {
        #region Private Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///  Changelog:
        ///             - 1.0.0 (08-15-2016) - Initial version.
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        #endregion Private Methods
    }
}