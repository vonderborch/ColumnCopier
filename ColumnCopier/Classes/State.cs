// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : State.cs
// Author           : Christian
// Created          : 06-07-2017
// 
// Version          : 2.1.0
// Last Modified By : Christian
// Last Modified On : 06-07-2017
// ***********************************************************************
// <copyright file="State.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the State class.
// </summary>
//
// Changelog: 
//            - 2.1.0 (06-07-2017) - Initial version created.
// ***********************************************************************
using ColumnCopier.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

/// <summary>
/// The Classes namespace.
/// </summary>
namespace ColumnCopier.Classes
{
    /// <summary>
    /// Class State.
    /// </summary>
    [DataContract]
    [KnownType(typeof(State))]
    [KnownType(typeof(Request))]
    [KnownType(typeof(RequestData))]
    public class State
    {
        #region Public Fields

        /// <summary>
        /// The history
        /// </summary>
        [DataMember]
        public List<string> History;

        /// <summary>
        /// The preserved requests
        /// </summary>
        [DataMember]
        public List<string> PreservedRequests;

        /// <summary>
        /// The request history
        /// </summary>
        [DataMember]
        public Dictionary<string, Request> RequestHistory;

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether [clean input data].
        /// </summary>
        /// <value><c>true</c> if [clean input data]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool CleanInputData { get; set; } = true;

        /// <summary>
        /// Gets or sets the current request.
        /// </summary>
        /// <value>The current request.</value>
        [DataMember]
        public string CurrentRequest { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether [data has headers].
        /// </summary>
        /// <value><c>true</c> if [data has headers]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool DataHasHeaders { get; set; } = true;

        /// <summary>
        /// Gets or sets the default index of the column.
        /// </summary>
        /// <value>The default index of the column.</value>
        [DataMember]
        public int DefaultColumnIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the default name of the column.
        /// </summary>
        /// <value>The default name of the column.</value>
        [DataMember]
        public string DefaultColumnName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the default column name match threshold.
        /// </summary>
        /// <value>The default column name match threshold.</value>
        [DataMember]
        public int DefaultColumnNameMatchThreshold { get; set; } = 5;

        /// <summary>
        /// Gets or sets the default column priority option.
        /// </summary>
        /// <value>The default column priority option.</value>
        [DataMember]
        public int DefaultColumnPriorityOption { get; set; } = (int)DefaultColumnPriority.Number;

        /// <summary>
        /// Gets or sets the index of the line separator option.
        /// </summary>
        /// <value>The index of the line separator option.</value>
        [DataMember]
        public int LineSeparatorOptionIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the line separator option inter.
        /// </summary>
        /// <value>The line separator option inter.</value>
        [DataMember]
        public string LineSeparatorOptionInter { get; set; } = ", ";

        /// <summary>
        /// Gets or sets the line separator option post.
        /// </summary>
        /// <value>The line separator option post.</value>
        [DataMember]
        public string LineSeparatorOptionPost { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the line separator option pre.
        /// </summary>
        /// <value>The line separator option pre.</value>
        [DataMember]
        public string LineSeparatorOptionPre { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum history.
        /// </summary>
        /// <value>The maximum history.</value>
        [DataMember]
        public int MaxHistory { get; set; } = 10;

        /// <summary>
        /// Gets or sets the program opacity.
        /// </summary>
        /// <value>The program opacity.</value>
        [DataMember]
        public int ProgramOpacity { get; set; } = 100;

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
        [DataMember]
        public int RequestId { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether [remove empty lines].
        /// </summary>
        /// <value><c>true</c> if [remove empty lines]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool RemoveEmptyLines { get; set; } = true;

        /// <summary>
        /// Gets or sets the save version.
        /// </summary>
        /// <value>The save version.</value>
        [DataMember]
        public int SaveVersion { get; set; } = Constants.SaveVersion;

        /// <summary>
        /// Gets or sets a value indicating whether [show on top].
        /// </summary>
        /// <value><c>true</c> if [show on top]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool ShowOnTop { get; set; } = false;

        /// <summary>
        /// Gets or sets the SQL connection provider.
        /// </summary>
        /// <value>The SQL connection provider.</value>
        [DataMember]
        public int SqlConnectionProvider { get; set; } = 0;

        /// <summary>
        /// Gets or sets the SQL connection string.
        /// </summary>
        /// <value>The SQL connection string.</value>
        [DataMember]
        public string SqlConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SQL select query.
        /// </summary>
        /// <value>The SQL select query.</value>
        [DataMember]
        public string SqlSelectQuery { get; set; } = string.Empty;

        #endregion Public Properties
    }
}