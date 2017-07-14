// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : ASqlProvider.cs
// Author           : Christian
// Created          : 07-13-2017
// 
// Version          : 2.2.0
// Last Modified By : Christian
// Last Modified On : 07-13-2017
// ***********************************************************************
// <copyright file="ASqlProvider.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Base SQL connect provider.
// </summary>
//
// Changelog: 
//            - 2.2.0 (07-13-2017) - Initial version created.
// ***********************************************************************
using System;

namespace ColumnCopier.Classes.SqlSupport
{
    /// <summary>
    /// Class ASqlProvider.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public abstract class ASqlProvider : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the SQL connection string.
        /// </summary>
        /// <value>The SQL connection string.</value>
        public string SqlConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the SQL query.
        /// </summary>
        /// <value>The SQL query.</value>
        public string SqlQuery { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        ///  Changelog:
        ///             - 2.2.0 (07-13-2017) - Initial version.
        public abstract void Dispose();

        /// <summary>
        /// Determines whether this instance [can connect to server].
        /// </summary>
        /// <returns><c>true</c> if this instance [can connect to server]; otherwise, <c>false</c>.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-13-2017) - Initial version.
        public abstract bool CanConnectToServer();

        /// <summary>
        /// Executes the SQL query.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-13-2017) - Initial version.
        public abstract string ExecuteSqlQuery();

        /// <summary>
        /// SQLs the select query is valid.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-13-2017) - Initial version.
        public abstract bool SqlSelectQueryIsValid();

        #endregion Public Methods
    }
}