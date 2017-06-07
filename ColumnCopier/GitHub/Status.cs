// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Status.cs
// Author           : Christian
// Created          : 06-06-2017
//
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="Status.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Status class.
// </summary>
//
// Changelog:
//            - 2.0.0 (06-06-2017) - Initial version.
// ***********************************************************************

using System.Runtime.Serialization;

namespace ColumnCopier.GitHub
{
    /// <summary>
    /// Class Status.
    /// </summary>
    [DataContract]
    public class Status
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [DataMember]
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the last updated.
        /// </summary>
        /// <value>The last updated.</value>
        [DataMember]
        public string last_updated { get; set; }

        #endregion Public Properties
    }
}
