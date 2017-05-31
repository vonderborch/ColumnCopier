// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Release.cs
// Author           : Christian
// Created          : 05-30-2017
//
// Version          : 1.3.0
// Last Modified By : Christian
// Last Modified On : 05-30-2017
// ***********************************************************************
// <copyright file="Release.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Release class.
// </summary>
//
// Changelog:
//            - 1.3.0 (05-30-2017) - Initial code for detecting the latest release of the app.
// ***********************************************************************

/// <summary>
/// The GitHub namespace.
/// </summary>
namespace ColumnCopier.GitHub
{
    /// <summary>
    /// Class Release.
    /// </summary>
    public class Release
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the HTML URL.
        /// </summary>
        /// <value>The HTML URL.</value>
        public string html_url { get; set; }
        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        /// <value>The name of the tag.</value>
        public string tag_name { get; set; }

        #endregion Public Properties
    }
}