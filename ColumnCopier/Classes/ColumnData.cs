// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : ColumnData.cs
// Author           : Christian
// Created          : 06-01-2017
//
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-01-2017
// ***********************************************************************
// <copyright file="ColumnData.cs" company="Christian Webber">
//		Copyright © 2017
// </copyright>
// <summary>
//      The ColumnData class.
// </summary>
//
// Changelog:
//            - 2.0.0 (06-01-2017) - Initial version created.
using System.Collections.Generic;

/// <summary>
/// The ColumnCopier namespace.
/// </summary>
namespace ColumnCopier.Classes
{
    /// <summary>
    /// Class ColumnData.
    /// </summary>
    public class ColumnData
    {
        #region Public Fields

        /// <summary>
        /// The current next line
        /// </summary>
        public int CurrentNextLine;

        /// <summary>
        /// The rows
        /// </summary>
        public List<string> Rows;

        #endregion Public Fields
    }
}