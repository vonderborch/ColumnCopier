using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnCopier
{
    public class Request
    {
        public Request(string rawText, int requestNumber, bool hasColumnHeaders)
        {
            ID = requestNumber;
            Name = $"Request {requestNumber}";
            ParseText(rawText, hasColumnHeaders);
        }

        public Request(string xmlData)
        {

        }

        public string ToXmlText()
        {
            var str = new StringBuilder();

            str.AppendLine("<Request>");

            str.AppendLine("<ColumnKeys>");
            foreach (var key in columnKeys)
                str.AppendLine(string.Format("<{0}>{1}</{0}>", key.Value, key.Key));
            str.AppendLine("</ColumnKeys>");

            str.AppendLine("<ColumnsData>");
            foreach (var key in columnKeys)
            {
                str.AppendLine("<Column>");
                str.AppendLine(string.Format("<Name>{0}</Name>", key.Value));
                for (int i = 0; i < columnsData[key.Value].Count; i++)
                    str.AppendLine(string.Format("<Row>{0}</Row>", columnsData[key.Value][i]));
                str.AppendLine("</Column>");
            }
            str.AppendLine("</ColumnsData>");

            str.AppendLine("</Request>");

            return str.ToString();
        }

        private void ParseText(string text, bool hasColumnHeaders)
        {
            var rawRows = text.Split('\n');

            for (int i = 0; i < rawRows.Length; i++)
            {
                var row = CleanText(rawRows[i]);

                var columns = row.Split('\t');
                
                if (i == 0)
                {
                    if (!hasColumnHeaders)
                    {
                        for (int j = 0; j < columns.Length; j++)
                        {
                            var name = $"Column{j}";
                            columnKeys.Add(j, name);
                            columnsData.Add(name, new List<string>());
                        }
                        NumberOfColumns = columnKeys.Count;
                    }
                    else
                    {
                        for (int j = 0; j < columns.Length; j++)
                        {
                            var name = columns[j];
                            columnKeys.Add(j, name);
                            columnsData.Add(name, new List<string>());
                        }
                        NumberOfColumns = columnKeys.Count;
                        continue;
                    }
                }

                for (int j = 0; j < columns.Length; j++)
                {
                    columnsData[columnKeys[j]].Add(columns[j]);
                }
            }

            CurrentColumn = columnKeys[0];
        }

        private string CleanText(string text)
        {
            return text.Trim();
        }

        public Dictionary<int, string> ColumnKeys
        {
            get { return new Dictionary<int, string>(columnKeys); }
        }

        public Dictionary<string, List<string>> ColumnsData
        {
            get { return new Dictionary<string, List<string>>(columnsData); }
        }

        public int ID { get; private set; } = -1;
        public string Name { get; private set; } = "";
        public int CurrentRowId { get; private set; } = 0;
        public string CurrentColumn { get; set; } = "";
        public int CurrentColumnInteger { get; private set; }
        public int NumberOfColumns { get; private set; }
        public int CurrentColumnNumberOfRows { get; private set; }

        private Dictionary<int, string> columnKeys = new Dictionary<int, string>();
        private Dictionary<string, List<string>> columnsData = new Dictionary<string, List<string>>();
        
        public string GetNextLine()
        {
            int rowId = CurrentRowId++;
            if (rowId >= columnsData[CurrentColumn].Count)
                rowId = 0;

            return columnsData[CurrentColumn][rowId];
        }

        public List<string> GetColumnTextLines(string columnName)
        {
            if (!string.IsNullOrEmpty(columnName) && columnsData.ContainsKey(columnName))
            {
                CurrentColumn = columnName;
                CurrentColumnNumberOfRows = columnsData[columnName].Count;
                return new List<string>(columnsData[columnName]);
            }

            return new List<string>();
        }

        public List<string> GetColumnTextLines(int columnNumber)
        {
            var column = columnKeys.ContainsKey(columnNumber)
                ? columnKeys[columnNumber]
                : null;

            if (column != null)
                CurrentColumnInteger = columnNumber;

            return GetColumnTextLines(column);
        }

        public string GetColumnText(string columnName)
        {
            if (!string.IsNullOrEmpty(columnName) && columnsData.ContainsKey(columnName))
            {
                CurrentColumn = columnName;
                CurrentColumnNumberOfRows = columnsData[columnName].Count;
                var str = new StringBuilder();

                for (int i = 0; i < columnsData[columnName].Count; i++)
                    str.AppendLine(columnsData[columnName][i]);

                return str.ToString();
            }

            return "";
        }

        public string GetColumnText(int columnNumber)
        {
            var column = columnKeys.ContainsKey(columnNumber)
                ? columnKeys[columnNumber]
                : null;

            if (column != null)
                CurrentColumnInteger = columnNumber;

            return GetColumnText(column);
        }
    }
}
