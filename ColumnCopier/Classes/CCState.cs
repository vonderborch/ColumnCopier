﻿using ColumnCopier.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        private List<string> preservedRequests = new List<string>();

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
            return !string.IsNullOrWhiteSpace(CurrentRequest)
                ? new List<string>(history[CurrentRequest].GetColumnNames())
                : new List<string>();
        }

        public string CurrentRequestCurrentColumnText()
        {
            return !string.IsNullOrWhiteSpace(CurrentRequest)
                ? history[CurrentRequest].GetCurrentColumnText()
                : string.Empty;
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

            // do we need to clean any of the history?
            if (historyLog.Count > MaxHistory)
                CleanHistory(historyLog.Count - MaxHistory);
        }

        public void DeleteCurrentRequest()
        {
            history.Remove(CurrentRequest);
            historyLog.Remove(CurrentRequest);
        }

        public string ExportCurrentRequest()
        {
            return !string.IsNullOrWhiteSpace(CurrentRequest)
                ? history[CurrentRequest].ExportRequest()
                : string.Empty;
        }

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

        public int PreservedRequestCount
        {
            get { return preservedRequests.Count; }
        }

        public int LineSeparatorOptionIndex { get; set; }
        public string LineSeparatorOptionPre { get; set; }
        public string LineSeparatorOptionPost { get; set; }
        public string LineSeparatorOptionInter { get; set; }

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

                saveGuard.Reset();
                return true;
            }

            return false;
        }

        public bool Load()
        {
            if (saveGuard.CheckSet)
            {
                saveGuard.Reset();
                return true;
            }

            return false;
        }
    }
}