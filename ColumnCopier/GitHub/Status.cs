// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Status.cs
// Author           : Christian
// Created          : 06-06-2017
//
// Version          : 2.1.0
// Last Modified By : Christian
// Last Modified On : 06-07-2017
// ***********************************************************************
// <copyright file="Status.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Status class.
// </summary>
//
// Changelog:
//            - 2.1.0 (06-07-2017) - Removed unnecessary datacontract/datamember attributes.
//            - 2.0.0 (06-06-2017) - Initial version.
// ***********************************************************************

using System.Runtime.Serialization;

namespace ColumnCopier.GitHub
{
    /// <summary>
    /// Class Status.
    /// </summary>
    public class Status
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the last updated.
        /// </summary>
        /// <value>The last updated.</value>
        public string last_updated { get; set; }

        #endregion Public Properties
    }
}
