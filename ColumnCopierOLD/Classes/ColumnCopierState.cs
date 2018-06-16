// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : ColumnCopierState.cs
// Author           : Christian
// Created          : 06-06-2017
// 
// Version          : 2.1.0
// Last Modified By : Christian
// Last Modified On : 06-07-2017
// ***********************************************************************
// <copyright file="ColumnCopierState.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the ColumnCopierState class.
// </summary>
//
// Changelog: 
//            - 2.2.0 (07-14-2017) - Added support for SQL providers and added functionality for returning the current request.
//            - 2.1.0 (06-07-2017) - Renamed and moved most fields/properties to the State class. Revised saving/loading system to use JSON data serialization.
//            - 2.0.0 (06-06-2017) - Initial version created.
// ***********************************************************************
using ColumnCopier.Classes.SqlSupport;
using ColumnCopier.Enums;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ColumnCopier.Classes
{
    /// <summary>
    /// Class CCState.
    /// </summary>
    public class ColumnCopierState
    {
        #region Private Fields

        /// <summary>
        /// The state
        /// </summary>
        private State state;

        /// <summary>
        /// The save guard
        /// </summary>
        private Guard saveGuard = new Guard();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnCopierState"/> class.
        /// </summary>
        public ColumnCopierState()
        {
            state = new State()
            {
                History = new List<string>(),
                PreservedRequests = new List<string>(),
                RequestHistory = new Dictionary<string, Classes.Request>(),
            };
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether [clean input data].
        /// </summary>
        /// <value><c>true</c> if [clean input data]; otherwise, <c>false</c>.</value>
        public bool CleanInputData
        {
            get { return state.CleanInputData; }
            set { state.CleanInputData = value; }
        }

        /// <summary>
        /// Gets or sets the current request.
        /// </summary>
        /// <value>The current request.</value>
        public string CurrentRequest
        {
            get { return state.CurrentRequest; }
            set { state.CurrentRequest = value; }
        }

        /// <summary>
        /// Gets the index of the current request.
        /// </summary>
        /// <value>The index of the current request.</value>
        public int CurrentRequestIndex
        {
            get { return GetRequestHistory().FindIndex(x => x == CurrentRequest); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [data has headers].
        /// </summary>
        /// <value><c>true</c> if [data has headers]; otherwise, <c>false</c>.</value>
        public bool DataHasHeaders
        {
            get { return state.DataHasHeaders; }
            set { state.DataHasHeaders = value; }
        }

        /// <summary>
        /// Gets or sets the default index of the column.
        /// </summary>
        /// <value>The default index of the column.</value>
        public int DefaultColumnIndex
        {
            get { return state.DefaultColumnIndex; }
            set { state.DefaultColumnIndex = value; }
        }

        /// <summary>
        /// Gets or sets the default name of the column.
        /// </summary>
        /// <value>The default name of the column.</value>
        public string DefaultColumnName
        {
            get { return state.DefaultColumnName; }
            set { state.DefaultColumnName = value; }
        }

        /// <summary>
        /// Gets or sets the default column name match.
        /// </summary>
        /// <value>The default column name match.</value>
        public int DefaultColumnNameMatch
        {
            get { return state.DefaultColumnNameMatchThreshold; }
            set { state.DefaultColumnNameMatchThreshold = value; }
        }

        /// <summary>
        /// Gets or sets the default column priority.
        /// </summary>
        /// <value>The default column priority.</value>
        public DefaultColumnPriority DefaultColumnPriority
        {
            get { return (DefaultColumnPriority)state.DefaultColumnPriorityOption; }
            set { state.DefaultColumnPriorityOption = (int)value; }
        }

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <value>The history.</value>
        public Dictionary<string, Request> History
        {
            get { return state.RequestHistory; }
        }

        /// <summary>
        /// Gets the history log.
        /// </summary>
        /// <value>The history log.</value>
        public List<string> HistoryLog
        {
            get { return state.History; }
        }

        /// <summary>
        /// Gets or sets the index of the line separator option.
        /// </summary>
        /// <value>The index of the line separator option.</value>
        public int LineSeparatorOptionIndex
        {
            get { return state.LineSeparatorOptionIndex; }
            set { state.LineSeparatorOptionIndex = value; }
        }

        /// <summary>
        /// Gets or sets the line separator option inter.
        /// </summary>
        /// <value>The line separator option inter.</value>
        public string LineSeparatorOptionInter
        {
            get { return state.LineSeparatorOptionInter; }
            set { state.LineSeparatorOptionInter = value; }
        }

        /// <summary>
        /// Gets or sets the line separator option post.
        /// </summary>
        /// <value>The line separator option post.</value>
        public string LineSeparatorOptionPost
        {
            get { return state.LineSeparatorOptionPost; }
            set { state.LineSeparatorOptionPost = value; }
        }

        /// <summary>
        /// Gets or sets the line separator option pre.
        /// </summary>
        /// <value>The line separator option pre.</value>
        public string LineSeparatorOptionPre
        {
            get { return state.LineSeparatorOptionPre; }
            set { state.LineSeparatorOptionPre = value; }
        }

        /// <summary>
        /// Gets or sets the maximum history.
        /// </summary>
        /// <value>The maximum history.</value>
        public int MaxHistory
        {
            get { return state.MaxHistory; }
            set { state.MaxHistory = value; }
        }


        /// <summary>
        /// Gets the preserved request count.
        /// </summary>
        /// <value>The preserved request count.</value>
        public int PreservedRequestCount
        {
            get { return state.PreservedRequests.Count; }
        }

        /// <summary>
        /// Gets or sets the program opacity.
        /// </summary>
        /// <value>The program opacity.</value>
        public int ProgramOpacity
        {
            get { return state.ProgramOpacity; }
            set { state.ProgramOpacity = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [remove empty lines].
        /// </summary>
        /// <value><c>true</c> if [remove empty lines]; otherwise, <c>false</c>.</value>
        public bool RemoveEmptyLines
        {
            get { return state.RemoveEmptyLines; }
            set { state.RemoveEmptyLines = value; }
        }

        /// <summary>
        /// Gets or sets the save file.
        /// </summary>
        /// <value>The save file.</value>
        public string SaveFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show on top].
        /// </summary>
        /// <value><c>true</c> if [show on top]; otherwise, <c>false</c>.</value>
        public bool ShowOnTop
        {
            get { return state.ShowOnTop; }
            set { state.ShowOnTop = value; }
        }

        /// <summary>
        /// Gets or sets the SQL connection provider.
        /// </summary>
        /// <value>The SQL connection provider.</value>
        public SqlConnectionProviders SqlConnectionProvider
        {
            get { return (SqlConnectionProviders)state.SqlConnectionProvider; }
            set { state.SqlConnectionProvider = (int)value; }
        }

        /// <summary>
        /// Gets or sets the SQL connection string.
        /// </summary>
        /// <value>The SQL connection string.</value>
        public string SqlConnectionString
        {
            get { return state.SqlConnectionString; }
            set { state.SqlConnectionString = value; }
        }

        /// <summary>
        /// Gets or sets the SQL provider.
        /// </summary>
        /// <value>The SQL provider.</value>
        public ASqlProvider SqlProvider { get; set; }

        /// <summary>
        /// Gets or sets the SQL select query.
        /// </summary>
        /// <value>The SQL select query.</value>
        public string SqlSelectQuery
        {
            get { return state.SqlSelectQuery; }
            set { state.SqlSelectQuery = value; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the new request.
        /// </summary>
        /// <param name="text">The text.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void AddNewRequest(string text)
        {
            var request = new Request(state.RequestId++, text, DataHasHeaders, CleanInputData, RemoveEmptyLines,
                DefaultColumnIndex, DefaultColumnName, DefaultColumnNameMatch, DefaultColumnPriority);
            AddRequestToHistory(request);
        }

        /// <summary>
        /// Adds the request to history.
        /// </summary>
        /// <param name="request">The request.</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void AddRequestToHistory(Request request)
        {
            state.RequestHistory.Add(request.Name, request);
            state.History.Add(request.Name);

            // do we need to clean any of the history?
            if (state.History.Count > MaxHistory)
                CleanHistory(state.History.Count - MaxHistory);
        }

        /// <summary>
        /// Cleans the history.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="respectPreservedRequests">if set to <c>true</c> [respect preserved requests].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void CleanHistory(int number, bool respectPreservedRequests = true)
        {
            // modify the number we have to delete by scanning for preserved requests
            if (respectPreservedRequests)
                number -= PreservedRequestCount;

            if (number > 0)
            {
                var i = 0;
                for (var j = 0; j < number && i < state.History.Count; j++)
                {
                    if (respectPreservedRequests && state.RequestHistory[state.History[i]].IsPreserved)
                    {
                        j--;
                    }
                    else
                    {
                        state.RequestHistory.Remove(state.History[i]);
                        state.History.RemoveAt(i);
                        i--;
                    }
                    i++;
                }
            }
        }

        /// <summary>
        /// Currents the request column names.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public List<string> CurrentRequestColumnNames()
        {
            return !string.IsNullOrWhiteSpace(CurrentRequest)
                ? new List<string>(state.RequestHistory[CurrentRequest].GetColumnNames())
                : new List<string>();
        }

        /// <summary>
        /// Currents the request current column text.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public string CurrentRequestCurrentColumnText()
        {
            return !string.IsNullOrWhiteSpace(CurrentRequest)
                ? state.RequestHistory[CurrentRequest].GetCurrentColumnText()
                : string.Empty;
        }

        /// <summary>
        /// Deletes the current request.
        /// </summary>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void DeleteCurrentRequest()
        {
            state.RequestHistory.Remove(CurrentRequest);
            state.History.Remove(CurrentRequest);
        }

        /// <summary>
        /// Exports the current request.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public string ExportCurrentRequest()
        {
            return !string.IsNullOrWhiteSpace(CurrentRequest)
                ? state.RequestHistory[CurrentRequest].ExportRequest()
                : string.Empty;
        }

        /// <summary>
        /// Gets the current request.
        /// </summary>
        /// <returns>ColumnData.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public Request GetCurrentRequest()
        {
            return !string.IsNullOrWhiteSpace(CurrentRequest)
                ? state.RequestHistory[CurrentRequest]
                : null;
        }

        /// <summary>
        /// Gets the request history.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public List<string> GetRequestHistory()
        {
            var result = new List<string>();
            for (var i = state.History.Count - 1; i >= 0; i--)
                result.Add(state.History[i]);

            return result;
        }

        /// <summary>
        /// Gets the request history position.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public int GetRequestHistoryPosition(string name)
        {
            return state.History.FindIndex(x => x == name);
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Added support for loading SQL connection providers.
        ///             - 2.1.0 (06-07-2017) - Support for the new State class. Revised save system to use JSON serialization.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public Ternary Load()
        {
            if (saveGuard.CheckSet)
            {
                if (!File.Exists(SaveFile))
                {
                    saveGuard.Reset();
                    return Ternary.False;
                }

                var file = SaveFile;
                var isCompressed = Path.GetExtension(file) == Constants.Instance.SaveExtensionCompressed;
                if (isCompressed)
                {
                    var destinationPath = Path.Combine(Path.GetDirectoryName(file), "TEMPORARY");
                    System.IO.Compression.ZipFile.ExtractToDirectory(file, destinationPath);
                    file = Path.Combine(destinationPath, Path.GetFileName(file));
                }

                var reader = new StreamReader(file);
                if (reader == null)
                {
                    saveGuard.Reset();
                    return Ternary.False;
                }

                var deserializer = new DataContractJsonSerializer(typeof(State));
                var newState = (State)deserializer.ReadObject(reader.BaseStream);
                reader.Close();

                // is this a readable save version for us?
                if (newState.SaveVersion != Constants.SaveVersion && newState.SaveVersion < Constants.SaveVersionMinimum)
                {
                    saveGuard.Reset();
                    return Ternary.Neutral;
                }

                state = newState;

                switch ((SqlConnectionProviders)state.SqlConnectionProvider)
                {
                    case SqlConnectionProviders.MySql:
                        SqlProvider = new SqlSupport.MySqlProvider();
                        break;
                    case SqlConnectionProviders.PostgreSQL:
                        SqlProvider = new SqlSupport.PostgreSqlProvider();
                        break;
                    case SqlConnectionProviders.SqlServer:
                        SqlProvider = new SqlSupport.SqlServerProvider();
                        break;
                    case SqlConnectionProviders.None:
                        SqlProvider = null;
                        break;
                }

                saveGuard.Reset();
                return Ternary.True;
            }
            return Ternary.False;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class. Revised save system to use JSON serialization.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public Ternary Save()
        {
            if (saveGuard.CheckSet)
            {
                // serialize the save data...
                var serializer = new DataContractJsonSerializer(typeof(Request));
                var stream = new MemoryStream();
                serializer.WriteObject(stream, state);
                var saveString = Encoding.Default.GetString(stream.ToArray());
                stream.Close();
                
                // save the file...
                File.WriteAllText(SaveFile, saveString);

                // compress?
                var file = SaveFile;
                var isCompressed = Path.GetExtension(file) == Constants.Instance.SaveExtensionCompressed;
                if (isCompressed)
                {
                    var destinationPath = Path.Combine(Path.GetDirectoryName(file), "TEMPORARY");
                    Directory.CreateDirectory(destinationPath);
                    File.Move(file, Path.Combine(destinationPath, Path.GetFileName(file)));
                    System.IO.Compression.ZipFile.CreateFromDirectory(destinationPath, file);
                    Directory.Delete(destinationPath, true);
                }

                // return that we succeeded!
                saveGuard.Reset();
                return Ternary.True;
            }
            return Ternary.False;
        }

        /// <summary>
        /// Sets the current request preservation toggle.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        ///  Changelog:
        ///             - 2.1.0 (06-07-2017) - Support for the new State class.
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void SetCurrentRequestPreservationToggle(bool newState)
        {
            if (!string.IsNullOrWhiteSpace(CurrentRequest))
            {
                state.RequestHistory[CurrentRequest].IsPreserved = newState;

                if (newState)
                {
                    if (!state.PreservedRequests.Contains(CurrentRequest))
                        state.PreservedRequests.Add(CurrentRequest);
                }
                else
                {
                    if (state.PreservedRequests.Contains(CurrentRequest))
                        state.PreservedRequests.Remove(CurrentRequest);
                }
            }
        }

        #endregion Public Methods
    }
}