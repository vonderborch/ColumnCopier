using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ColumnCopier.Classes;

namespace ColumnCopier
{
    [DataContract]
    [KnownType(typeof(State))]
    [KnownType(typeof(Request))]
    [KnownType(typeof(ColumnData))]
    [KnownType(typeof(DefaultColumnSettings))]
    [KnownType(typeof(RequestSettings))]
    public class State
    {
        [DataMember]
        public List<string> History;

        [DataMember]
        public List<string> PreservedRequests;

        [DataMember]
        public Dictionary<string, Request> RequestHistory;

        [DataMember]
        public DefaultColumnSettings DefaultColumnSettings;

        [DataMember]
        public RequestSettings RequestSettings;

        [DataMember]
        public List<LineSeperatorOption> LineSeperators;

        [DataMember]
        public int CurrentLineSeperatorIndex;

        [DataMember]
        public int MaxHistory { get; set; } = 10;

        [DataMember]
        public int ProgramOpacity { get; set; } = 100;

        [DataMember]
        public string CurrentRequestName { get; set; } = string.Empty;

        private Guard guard = new Guard();

        public Request CurrentRequest
        {
            get
            {
                return History.Count > 0
                    ? RequestHistory[CurrentRequestName]
                    : null;
            }
        }

        [DataMember]
        public ulong RequestId { get; set; } = 0;

        [DataMember]
        public bool ShowOnTop { get; set; } = false;

        public State()
        {
            RequestId = 0;
            MaxHistory = 10;
            ProgramOpacity = 100;
            CurrentRequestName = string.Empty;
            ShowOnTop = false;

            History = new List<string>();
            PreservedRequests = new List<string>();
            RequestHistory = new Dictionary<string, Request>();
            DefaultColumnSettings = new DefaultColumnSettings();
            RequestSettings = new RequestSettings();
            LineSeperators = new List<LineSeperatorOption>()
            {
                  new LineSeperatorOption("", ", ", "") // Comma-deliminated
                , new LineSeperatorOption("", "; ", "") // Semicolon-deliminated
                , new LineSeperatorOption("", "", "") // Nothing
                , new LineSeperatorOption("\"", "\", \"", "\"") // Double-Quote and Comma
                , new LineSeperatorOption("(", ", ", ")") // Parenthesis and comma
                , new LineSeperatorOption("('", "', '", "')") // Parenthesis, Single-Quote, and comma
                , new LineSeperatorOption("(\"", "\", \"", "\")") // Parenthesis, Double-Quote, and comma
            };
        }

        public bool AddNewLineSeperatorOption(string pre, string inter, string post)
        {
            var newName = $"{pre}-{inter}-{post}";

            for (var i = 0; i < LineSeperators.Count; i++)
            {
                if (newName.Equals(LineSeperators[i].Name))
                {
                    return false; // already exists!
                }
            }

            LineSeperators.Add(new LineSeperatorOption(pre, inter, post));

            return true;
        }

        public State(State oldState)
        {
            RequestId = oldState.RequestId;
            MaxHistory = oldState.MaxHistory;
            ProgramOpacity = oldState.ProgramOpacity;
            CurrentRequestName = oldState.CurrentRequestName;
            ShowOnTop = oldState.ShowOnTop;

            History = new List<string>(oldState.History);
            PreservedRequests = new List<string>(oldState.PreservedRequests);
            RequestHistory = new Dictionary<string, Request>(oldState.RequestHistory);
            DefaultColumnSettings = new DefaultColumnSettings(oldState.DefaultColumnSettings);
            RequestSettings = new RequestSettings(oldState.RequestSettings);
            LineSeperators = new List<LineSeperatorOption>(oldState.LineSeperators);
        }

        public void AddNewRequest(string text)
        {
            var newRequest = new Request(text, RequestSettings, RequestId++, DefaultColumnSettings);

            RequestHistory.Add(newRequest.Name, newRequest);
            History.Add(newRequest.Name);

            if (History.Count > MaxHistory)
                CleanHistory(History.Count - MaxHistory);

            CurrentRequestName = newRequest.Name;
        }

        public void DeleteCurrentRequest()
        {
            if (CurrentRequest.IsPreserved)
            {
                PreservedRequests.Remove(CurrentRequestName);
            }

            History.Remove(CurrentRequestName);
            RequestHistory.Remove(CurrentRequestName);

            CurrentRequestName = History.Count == 0
                ? string.Empty
                : History[History.Count - 1];
        }

        public void SetCurrentRequestName(string newName)
        {
            // throw if we have a request with the new name already
            if (History.Contains(newName))
            {
                throw new Exception($"A request with the name {newName} already exists!");
            }

            // update the preserved request name if it exists
            if (PreservedRequests.Contains(CurrentRequestName))
            {
                for (var i = 0; i < PreservedRequests.Count; i++)
                {
                    if (PreservedRequests[i] == CurrentRequestName)
                    {
                        PreservedRequests[i] = newName;
                    }
                }
            }

            // update the history list with the new name
            for (var i = 0; i < History.Count; i++)
            {
                if (History[i] == CurrentRequestName)
                {
                    History[i] = newName;
                }
            }

            // update the request history item
            var updatedRequest = new Request(RequestHistory[CurrentRequestName])
            {
                Name = newName
            };
            RequestHistory.Remove(CurrentRequestName);
            RequestHistory.Add(newName, updatedRequest);
        }

        public void CleanHistory(int maxItems, bool respectPreservedRequests = true)
        {
            if (respectPreservedRequests)
                maxItems -= PreservedRequests.Count;

            if (maxItems > MaxHistory)
            {
                for (int i = 0, j = 0; i < History.Count && j < maxItems; i++, j++)
                {
                    var isPreserved = PreservedRequests.Contains(History[i]);

                    if (respectPreservedRequests && isPreserved)
                    {
                        j--;
                    }
                    else
                    {
                        if (isPreserved)
                            PreservedRequests.Remove(History[i]);

                        RequestHistory.Remove(History[i]);
                        History.RemoveAt(i);

                        i--;
                    }
                }
            }

            CurrentRequestName = History.Count == 0
                ? string.Empty
                : History[History.Count - 1];
        }

        public string GetSerializedState()
        {
            var output = string.Empty;
            
            while (guard.Check)
            {
                Thread.Sleep(50);

                if (guard.CheckSet)
                {
                    var serializer = new DataContractSerializer(typeof(State));
                    var stream = new MemoryStream();
                    serializer.WriteObject(stream, this);
                    output = Encoding.Default.GetString(stream.ToArray());
                    stream.Close();

                    break;
                }
            }

            return output;
        }
    }
}
