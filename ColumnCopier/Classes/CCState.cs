// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : CCState.cs
// Author           : Christian
// Created          : 06-06-2017
// 
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="CCState.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the CCState class.
// </summary>
//
// Changelog: 
//            - 2.0.0 (06-06-2017) - Initial version created.
// ***********************************************************************
using ColumnCopier.Enums;
using ColumnCopier.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ColumnCopier.Classes
{
    /// <summary>
    /// Class CCState.
    /// </summary>
    public class CCState
    {
        #region Private Fields

        /// <summary>
        /// The current request
        /// </summary>
        private string currentRequest = "";

        /// <summary>
        /// The history
        /// </summary>
        private Dictionary<string, Request> history = new Dictionary<string, Request>();

        /// <summary>
        /// The history log
        /// </summary>
        private List<string> historyLog = new List<string>();

        /// <summary>
        /// The preserved requests
        /// </summary>
        private List<string> preservedRequests = new List<string>();

        /// <summary>
        /// The request identifier
        /// </summary>
        private int requestId = 0;

        /// <summary>
        /// The save guard
        /// </summary>
        private Guard saveGuard = new Guard();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CCState"/> class.
        /// </summary>
        public CCState()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether [clean input data].
        /// </summary>
        /// <value><c>true</c> if [clean input data]; otherwise, <c>false</c>.</value>
        public bool CleanInputData { get; set; } = true;

        /// <summary>
        /// Gets or sets the current request.
        /// </summary>
        /// <value>The current request.</value>
        public string CurrentRequest
        {
            get { return currentRequest; }
            set { currentRequest = value; }
        }

        /// <summary>
        /// Gets the index of the current request.
        /// </summary>
        /// <value>The index of the current request.</value>
        public int CurrentRequestIndex
        {
            get { return historyLog.FindIndex(x => x == CurrentRequest); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [data has headers].
        /// </summary>
        /// <value><c>true</c> if [data has headers]; otherwise, <c>false</c>.</value>
        public bool DataHasHeaders { get; set; } = true;
        /// <summary>
        /// Gets or sets the default index of the column.
        /// </summary>
        /// <value>The default index of the column.</value>
        public int DefaultColumnIndex { get; set; } = 0;
        /// <summary>
        /// Gets or sets the default name of the column.
        /// </summary>
        /// <value>The default name of the column.</value>
        public string DefaultColumnName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the default column name match.
        /// </summary>
        /// <value>The default column name match.</value>
        public int DefaultColumnNameMatch { get; set; } = 5;
        /// <summary>
        /// Gets or sets the default column priority.
        /// </summary>
        /// <value>The default column priority.</value>
        public DefaultColumnPriority DefaultColumnPriority { get; set; }

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <value>The history.</value>
        public Dictionary<string, Request> History
        {
            get { return history; }
        }

        /// <summary>
        /// Gets the history log.
        /// </summary>
        /// <value>The history log.</value>
        public List<string> HistoryLog
        {
            get { return historyLog; }
        }

        /// <summary>
        /// Gets or sets the index of the line separator option.
        /// </summary>
        /// <value>The index of the line separator option.</value>
        public int LineSeparatorOptionIndex { get; set; }
        /// <summary>
        /// Gets or sets the line separator option inter.
        /// </summary>
        /// <value>The line separator option inter.</value>
        public string LineSeparatorOptionInter { get; set; }
        /// <summary>
        /// Gets or sets the line separator option post.
        /// </summary>
        /// <value>The line separator option post.</value>
        public string LineSeparatorOptionPost { get; set; }
        /// <summary>
        /// Gets or sets the line separator option pre.
        /// </summary>
        /// <value>The line separator option pre.</value>
        public string LineSeparatorOptionPre { get; set; }
        /// <summary>
        /// Gets or sets the maximum history.
        /// </summary>
        /// <value>The maximum history.</value>
        public int MaxHistory { get; set; } = 10;

        /// <summary>
        /// Gets the preserved request count.
        /// </summary>
        /// <value>The preserved request count.</value>
        public int PreservedRequestCount
        {
            get { return preservedRequests.Count; }
        }

        /// <summary>
        /// Gets or sets the program opacity.
        /// </summary>
        /// <value>The program opacity.</value>
        public int ProgramOpacity { get; set; } = 100;
        /// <summary>
        /// Gets or sets a value indicating whether [remove empty lines].
        /// </summary>
        /// <value><c>true</c> if [remove empty lines]; otherwise, <c>false</c>.</value>
        public bool RemoveEmptyLines { get; set; } = true;
        /// <summary>
        /// Gets or sets the save file.
        /// </summary>
        /// <value>The save file.</value>
        public string SaveFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show on top].
        /// </summary>
        /// <value><c>true</c> if [show on top]; otherwise, <c>false</c>.</value>
        public bool ShowOnTop { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the new request.
        /// </summary>
        /// <param name="text">The text.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void AddNewRequest(string text)
        {
            var request = new Request(requestId++, text, DataHasHeaders, CleanInputData, RemoveEmptyLines,
                DefaultColumnIndex, DefaultColumnName, DefaultColumnNameMatch, DefaultColumnPriority);
            AddRequestToHistory(request);
        }

        /// <summary>
        /// Adds the new request.
        /// </summary>
        /// <param name="node">The node.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void AddNewRequest(XElement node)
        {
            var request = new Request(node);
            AddRequestToHistory(request);
        }

        /// <summary>
        /// Adds the request to history.
        /// </summary>
        /// <param name="request">The request.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void AddRequestToHistory(Request request)
        {
            history.Add(request.Name, request);
            historyLog.Add(request.Name);

            // do we need to clean any of the history?
            if (historyLog.Count > MaxHistory)
                CleanHistory(historyLog.Count - MaxHistory);
        }

        /// <summary>
        /// Cleans the history.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="respectPreservedRequests">if set to <c>true</c> [respect preserved requests].</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void CleanHistory(int number, bool respectPreservedRequests = true)
        {
            // modify the number we have to delete by scanning for preserved requests
            if (respectPreservedRequests)
                number -= PreservedRequestCount;

            if (number > 0)
            {
                var i = 0;
                for (var j = 0; j < number && i < historyLog.Count; j++)
                {
                    if (respectPreservedRequests && history[historyLog[i]].IsPreserved)
                    {
                        j--;
                    }
                    else
                    {
                        history.Remove(historyLog[i]);
                        historyLog.RemoveAt(i);
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
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public List<string> CurrentRequestColumnNames()
        {
            return !string.IsNullOrWhiteSpace(CurrentRequest)
                ? new List<string>(history[CurrentRequest].GetColumnNames())
                : new List<string>();
        }

        /// <summary>
        /// Currents the request current column text.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public string CurrentRequestCurrentColumnText()
        {
            return !string.IsNullOrWhiteSpace(CurrentRequest)
                ? history[CurrentRequest].GetCurrentColumnText()
                : string.Empty;
        }

        /// <summary>
        /// Deletes the current request.
        /// </summary>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void DeleteCurrentRequest()
        {
            history.Remove(CurrentRequest);
            historyLog.Remove(CurrentRequest);
        }

        /// <summary>
        /// Exports the current request.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public string ExportCurrentRequest()
        {
            return !string.IsNullOrWhiteSpace(CurrentRequest)
                ? history[CurrentRequest].ExportRequest()
                : string.Empty;
        }

        /// <summary>
        /// Gets the request history.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public List<string> GetRequestHistory()
        {
            var result = new List<string>();
            for (var i = historyLog.Count - 1; i >= 0; i--)
                result.Add(historyLog[i]);

            return result;
        }

        /// <summary>
        /// Gets the request history position.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public int GetRequestHistoryPosition(string name)
        {
            return historyLog.FindIndex(x => x == name);
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public bool Load()
        {
            if (saveGuard.CheckSet)
            {
                if (!File.Exists(SaveFile))
                {
                    saveGuard.Reset();
                    return false;
                }

                var file = SaveFile;
                bool isCompressed = Path.GetExtension(file) == Constants.Instance.SaveExtensionCompressed;
                if (isCompressed)
                {
                    var destinationPath = Path.Combine(Path.GetDirectoryName(file), "TEMPORARY");
                    System.IO.Compression.ZipFile.ExtractToDirectory(file, destinationPath);
                    file = Path.Combine(destinationPath, Path.GetFileName(file));
                }

                var doc = XDocument.Load(file);

                foreach (var mainCategory in doc.Root.Elements())
                {
                    var mainCategoryName = mainCategory.Name.ToString();

                    if (mainCategoryName == "SaveVersion")
                    {
                        if (Converters.ConvertToInt(mainCategory.Value.ToString()) != Constants.SaveVersion)
                        {
                            saveGuard.Reset();
                            return false;
                        }
                    }
                    else if (mainCategoryName == "Settings")
                    {
                        foreach (var settingNode in mainCategory.Elements())
                        {
                            switch (settingNode.Name.ToString())
                            {
                                case "ShowOnTop":
                                    ShowOnTop = Converters.ConvertToBool(settingNode.Value, false);
                                    break;

                                case "DataHasHeaders":
                                    DataHasHeaders = Converters.ConvertToBool(settingNode.Value, true);
                                    break;

                                case "CleanInputData":
                                    CleanInputData = Converters.ConvertToBool(settingNode.Value, true);
                                    break;

                                case "RemoveEmptyLines":
                                    RemoveEmptyLines = Converters.ConvertToBool(settingNode.Value, true);
                                    break;

                                case "LineSeparatorOptionIndex":
                                    LineSeparatorOptionIndex = Converters.ConvertToInt(settingNode.Value);
                                    break;

                                case "LineSeparatorOptionPre":
                                    LineSeparatorOptionPre = settingNode.Value;
                                    break;

                                case "LineSeparatorOptionPost":
                                    LineSeparatorOptionPost = settingNode.Value;
                                    break;

                                case "LineSeparatorOptionInter":
                                    LineSeparatorOptionInter = settingNode.Value;
                                    break;

                                case "DefaultColumnIndex":
                                    DefaultColumnIndex = Converters.ConvertToIntWithClamp(settingNode.Value, 0, 0);
                                    break;

                                case "DefaultColumnName":
                                    DefaultColumnName = settingNode.Value;
                                    break;

                                case "DefaultColumnNameMatch":
                                    DefaultColumnNameMatch = Converters.ConvertToIntWithClamp(settingNode.Value, 0, 0);
                                    break;

                                case "DefaultColumnPriority":
                                    DefaultColumnPriority = (DefaultColumnPriority)Converters.ConvertToInt(settingNode.Value);
                                    break;

                                case "MaxHistory":
                                    MaxHistory = Converters.ConvertToIntWithClamp(settingNode.Value, 0, 0);
                                    break;

                                case "ProgramOpacity":
                                    ProgramOpacity = Converters.ConvertToIntWithClamp(settingNode.Value, 100, 0, 100);
                                    break;

                                case "CurrentRequest":
                                    CurrentRequest = settingNode.Value;
                                    break;
                            }
                        }
                    }
                    else if (mainCategoryName == "History")
                    {
                        history.Clear();
                        foreach (var requestNode in mainCategory.Elements())
                        {
                            if (requestNode.Name == "Request")
                                AddNewRequest(requestNode);
                        }
                    }
                }

                saveGuard.Reset();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public bool Save()
        {
            if (saveGuard.CheckSet)
            {
                var str = new StringBuilder();

                str.AppendLine("<ColumnCopier>");

                str.AppendLine($"<SaveVersion>{Constants.SaveVersion}</SaveVersion>");
                str.AppendLine("<Settings>");
                str.AppendLine($"<ShowOnTop>{ShowOnTop}</ShowOnTop>");
                str.AppendLine($"<DataHasHeaders>{DataHasHeaders}</DataHasHeaders>");
                str.AppendLine($"<CleanInputData>{CleanInputData}</CleanInputData>");
                str.AppendLine($"<RemoveEmptyLines>{RemoveEmptyLines}</RemoveEmptyLines>");
                str.AppendLine($"<ProgramOpacity>{ProgramOpacity}</ProgramOpacity>");

                str.AppendLine($"<LineSeparatorOptionIndex>{LineSeparatorOptionIndex}</LineSeparatorOptionIndex>");
                str.AppendLine($"<LineSeparatorOptionPre>{LineSeparatorOptionPre}</LineSeparatorOptionPre>");
                str.AppendLine($"<LineSeparatorOptionPost>{LineSeparatorOptionPost}</LineSeparatorOptionPost>");
                str.AppendLine($"<LineSeparatorOptionInter>{LineSeparatorOptionInter}</LineSeparatorOptionInter>");

                str.AppendLine($"<DefaultColumnIndex>{DefaultColumnIndex}</DefaultColumnIndex>");
                str.AppendLine($"<DefaultColumnName>{DefaultColumnName}</DefaultColumnName>");
                str.AppendLine($"<DefaultColumnNameMatch>{DefaultColumnNameMatch}</DefaultColumnNameMatch>");
                str.AppendLine($"<DefaultColumnPriority>{(int)DefaultColumnPriority}</DefaultColumnPriority>");

                str.AppendLine($"<MaxHistory>{MaxHistory}</MaxHistory>");

                str.AppendLine($"<CurrentRequest>{currentRequest}</CurrentRequest>");
                str.AppendLine("</Settings>");

                str.AppendLine("<History>");
                if (!string.IsNullOrEmpty(currentRequest))
                {
                    foreach (var request in history)
                        str.AppendLine(request.Value.ConvertRequestToXml());
                }
                str.AppendLine("</History>");

                str.AppendLine("</ColumnCopier>");

                var result = str.ToString();
                var doc = XDocument.Parse(result);

                if (File.Exists(SaveFile))
                    File.Delete(SaveFile);

                doc.Save(SaveFile);
                var file = SaveFile;
                bool isCompressed = Path.GetExtension(file) == Constants.Instance.SaveExtensionCompressed;
                if (isCompressed)
                {
                    var destinationPath = Path.Combine(Path.GetDirectoryName(file), "TEMPORARY");
                    Directory.CreateDirectory(destinationPath);
                    File.Move(file, Path.Combine(destinationPath, Path.GetFileName(file)));
                    System.IO.Compression.ZipFile.CreateFromDirectory(destinationPath, file);
                    Directory.Delete(destinationPath, true);
                }

                saveGuard.Reset();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the current request preservation toggle.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public void SetCurrentRequestPreservationToggle(bool state)
        {
            if (!string.IsNullOrWhiteSpace(CurrentRequest))
            {
                history[CurrentRequest].IsPreserved = state;

                if (state)
                {
                    if (!preservedRequests.Contains(CurrentRequest))
                        preservedRequests.Add(CurrentRequest);
                }
                else
                {
                    if (preservedRequests.Contains(CurrentRequest))
                        preservedRequests.Remove(CurrentRequest);
                }
            }
        }

        #endregion Public Methods
    }
}