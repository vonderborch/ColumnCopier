// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Program.cs
// Author           : Christian
// Created          : 08-15-2016
// 
// Version          : 2.2.0
// Last Modified By : Christian
// Last Modified On : 07-14-2016
// ***********************************************************************
// <copyright file="Program.cs" company="Christian Webber">
//		Copyright ©  2016
// </copyright>
// <summary>
//      Main entrypoint.
// </summary>
//
// Changelog: 
//            - 2.2.0 (07-14-2017) - Added try/catch to the entrypoint.
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
        ///             - 2.2.0 (07-14-2017) - Added try/catch to surround the application, just in case.
        ///             - 1.0.0 (08-15-2016) - Initial version.
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            catch (Exception ex)
            {

            }
        }

        #endregion Private Methods
    }
}