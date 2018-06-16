// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : RequestData.cs
// Author           : Christian
// Created          : 06-07-2017
//
// Version          : 2.1.0
// Last Modified By : Christian
// Last Modified On : 06-07-2017
// ***********************************************************************
// <copyright file="RequestData.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The RequestData class.
// </summary>
//
// Changelog:
//            - 2.1.0 (06-07-2017) - Initial version created.
// ***********************************************************************
using System.Collections.Generic;

/// <summary>
/// The Classes namespace.
/// </summary>
namespace ColumnCopier.Classes
{
    /// <summary>
    /// Class RequestData.
    /// </summary>
    public class RequestData
    {
        #region Public Fields

        /// <summary>
        /// The column data
        /// </summary>
        public Dictionary<string, ColumnData> ColumnData;

        /// <summary>
        /// The column keys
        /// </summary>
        public Dictionary<int, string> ColumnKeys;

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the index of the current column.
        /// </summary>
        /// <value>The index of the current column.</value>
        public int CurrentColumnIndex { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is preserved.
        /// </summary>
        /// <value><c>true</c> if this instance is preserved; otherwise, <c>false</c>.</value>
        public bool IsPreserved { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion Public Properties
    }
}