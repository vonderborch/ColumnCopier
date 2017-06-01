using ColumnCopier.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnCopier.Classes
{
    public class CCState
    {
        private string currentRequest = "";

        public string CurrentRequest
        {
            get { return currentRequest; }
            set { currentRequest = value; }
        }

        public DefaultColumnPriority DefaultColumnPriority { get; set; }

        private Dictionary<string, Request> history = new Dictionary<string, Request>();

        private List<string> historyLog = new List<string>();

        public Dictionary<string, Request> History
        {
            get { return history; }
        }

        public List<string> HistoryLog
        {
            get { return historyLog; }
        }

        public int MaxHistory { get; set; } = 10;

        private Guard saveGuard = new Guard();

        private int requestId = 0;

        public string SaveFile { get; set; }

        public bool ShowOnTop { get; set; }

        public bool DataHasHeaders { get; set; } = true;
        public bool CleanInputData { get; set; } = true;
        public bool RemoveEmptyLines { get; set; } = true;

        public int DefaultColumnIndex { get; set; } = 0;
        public int DefaultColumnNameMatch { get; set; } = 5;
        public string DefaultColumnName { get; set; } = string.Empty;

        public CCState()
        {

        }

        public List<string> CurrentRequestColumnNames()
        {
            return new List<string>(history[CurrentRequest].GetColumnNames());
        }

        public string CurrentRequestCurrentColumnText()
        {
            return history[CurrentRequest].GetCurrentColumnText();
        }

        public void AddNewRequest(string text)
        {
            var request = new Request(requestId++, text, DataHasHeaders, CleanInputData, RemoveEmptyLines, 
                DefaultColumnIndex, DefaultColumnName, DefaultColumnNameMatch, DefaultColumnPriority);
            AddRequestToHistory(request);

        }

        public List<string> GetRequestHistory()
        {
            var result = new List<string>();
            for (var i = historyLog.Count - 1; i >= 0; i--)
                result.Add(historyLog[i]);

            return result;
        }

        public int GetRequestHistoryPosition(string name)
        {
            return historyLog.FindIndex(x => x == name);
        }

        public void AddRequestToHistory(Request request)
        {
            history.Add(request.Name, request);
            historyLog.Add(request.Name);

            var item = 0;
            while (historyLog.Count > MaxHistory)
            {
                if (history[historyLog[item]].IsPreserved)
                {
                    item++;
                    if (item >= history.Count)
                        break;
                }
                else
                {
                    history.Remove(historyLog[item]);
                    historyLog.RemoveAt(item);
                }
            }
        }
    }
}
