// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Request.cs
// Author           : Christian
// Created          : 08-18-2016
// 
// Version          : 1.1.6
// Last Modified By : Christian
// Last Modified On : 09-29-2016
// ***********************************************************************
// <copyright file="Request.cs" company="Christian Webber">
//		Copyright ©  2016
// </copyright>
// <summary>
//      The Request class.
// </summary>
//
// Changelog: 
//            - 1.1.6 (09-29-2016) - Fixed bug when loading old saves (bumped save version), fixed bug when copying invalid characters, fixed bug with no data in the clipboard
///           - 1.1.5 (09-21-2016) - Added request exporting functionality.
///           - 1.1.4 (09-21-2016) - Enhanced robustness of new line splitter. Adjusted saving of ColumnKeys to split the value between two items to prevent a crash during saving.
///           - 1.1.3 (08-30-2016) - Removed string. format to new format approach when saving a request.
//            - 1.0.0 (08-22-2016) - Finished initial code.
//            - 0.0.0 (08-18-2016) - Initial version created.
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ColumnCopier
{
    /// <summary>
    /// Request class.
    /// </summary>
    public class Request
    {
        #region Private Fields

        /// <summary>
        /// The column keys
        /// </summary>
        private Dictionary<int, string> columnKeys = new Dictionary<int, string>();

        private Dictionary<string, List<string>> columnsData = new Dictionary<string, List<string>>();

        /// <summary>
        /// The splitters to use for parsing text
        /// \r\n : default Windows, \n : default Unix, \r : Old
        /// </summary>
        private static string[] splitters = {"\r\n", "\n", "\r"};

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="rawText">The raw text.</param>
        /// <param name="requestNumber">The request number.</param>
        /// <param name="hasColumnHeaders">if set to <c>true</c> [has column headers].</param>
        /// <param name="cleanData">if set to <c>true</c> [clean data].</param>
        public Request(string rawText, int requestNumber, bool hasColumnHeaders, bool cleanData)
        {
            ID = requestNumber;
            Name = $"Request {requestNumber}";
            ParseText(rawText, hasColumnHeaders, cleanData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="xmlData">The XML data.</param>
        public Request(XElement xmlData)
        {
            foreach (var node in xmlData.Elements())
            {
                var nodeName = node.Name.ToString();
                if (nodeName == "Name")
                {
                    Name = node.Value;
                }
                else if (nodeName == "ID")
                {
                    ID = Main.ParseTextToInt(node.Value);
                }
                else if (nodeName == "ColumnKeys")
                {
                    foreach (var column in node.Elements())
                    {
                        var key = -1;
                        var value = "";

                        foreach (var columnItem in column.Elements())
                        {
                            switch (columnItem.Name.ToString())
                            {
                                case "Key":
                                    key = Main.ParseTextToInt(columnItem.Value);
                                    break;
                                case "Value":
                                    value = columnItem.Value;
                                    break;
                            }
                        }

                        columnKeys.Add(key, value);
                        columnsData.Add(value, new List<string>());
                    }
                    NumberOfColumns = columnKeys.Count;
                }
                else if (nodeName == "ColumnsData")
                {
                    foreach (var column in node.Elements())
                    {
                        var columnName = "";

                        foreach (var row in column.Elements())
                        {
                            var rowName = row.Name.ToString();
                            if (rowName == "Name")
                            {
                                columnName = row.Value;
                            }
                            else if (rowName == "Row")
                            {
                                columnsData[columnName].Add(row.Value);
                            }
                        }
                    }
                }
            }

            CurrentColumn = columnKeys[0];
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the column keys.
        /// </summary>
        /// <value>The column keys.</value>
        public Dictionary<int, string> ColumnKeys
        {
            get { return new Dictionary<int, string>(columnKeys); }
        }

        /// <summary>
        /// Gets the columns data.
        /// </summary>
        /// <value>The columns data.</value>
        public Dictionary<string, List<string>> ColumnsData
        {
            get { return new Dictionary<string, List<string>>(columnsData); }
        }

        /// <summary>
        /// Gets or sets the current column.
        /// </summary>
        /// <value>The current column.</value>
        public string CurrentColumn { get; set; } = "";

        /// <summary>
        /// Gets the current column integer.
        /// </summary>
        /// <value>The current column integer.</value>
        public int CurrentColumnInteger { get; private set; }

        /// <summary>
        /// Gets the current column number of rows.
        /// </summary>
        /// <value>The current column number of rows.</value>
        public int CurrentColumnNumberOfRows { get; private set; }

        /// <summary>
        /// Gets or sets the current row identifier.
        /// </summary>
        /// <value>The current row identifier.</value>
        public int CurrentRowId { get; set; } = 0;

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int ID { get; private set; } = -1;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <value>The number of columns.</value>
        public int NumberOfColumns { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 1.1.5 (09-21-2016) - Initial version.
        public string Export()
        {
            try
            {
                StringBuilder str = new StringBuilder();

                //// export headers
                StringBuilder rawHeader = new StringBuilder();
                foreach (var item in columnKeys)
                    rawHeader.AppendFormat("{0}\t", item.Value);

                var header = rawHeader.ToString();
                str.AppendFormat("{0}{1}", header.Remove(header.Length - 1), Environment.NewLine);

                //// export data
                // calculate longest column
                int maxColumn = int.MinValue;
                foreach (var column in ColumnsData)
                {
                    if (maxColumn < column.Value.Count)
                        maxColumn = column.Value.Count;
                }
                // export rows
                for (int i = 0; i < maxColumn; i++)
                {
                    StringBuilder rawRow = new StringBuilder();
                    foreach (var item in ColumnsData)
                    {
                        if (i < item.Value.Count)
                        {
                            rawRow.AppendFormat("{0}\t", item.Value[i]);
                        }
                    }

                    var row = rawRow.ToString();
                    str.AppendFormat("{0}{1}", row.Remove(row.Length - 1), Environment.NewLine);
                }

                return str.ToString();

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Gets the column text.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-18-2016) - Initial version.
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

        /// <summary>
        /// Gets the column text.
        /// </summary>
        /// <param name="columnNumber">The column number.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-18-2016) - Initial version.
        public string GetColumnText(int columnNumber)
        {
            var column = columnKeys.ContainsKey(columnNumber)
                ? columnKeys[columnNumber]
                : null;

            if (column != null)
                CurrentColumnInteger = columnNumber;

            return GetColumnText(column);
        }

        /// <summary>
        /// Gets the column text lines.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-18-2016) - Initial version.
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

        /// <summary>
        /// Gets the column text lines.
        /// </summary>
        /// <param name="columnNumber">The column number.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-18-2016) - Initial version.
        public List<string> GetColumnTextLines(int columnNumber)
        {
            var column = columnKeys.ContainsKey(columnNumber)
                ? columnKeys[columnNumber]
                : null;

            if (column != null)
                CurrentColumnInteger = columnNumber;

            return GetColumnTextLines(column);
        }

        /// <summary>
        /// Gets the next line.
        /// </summary>
        /// <param name="tmp">The temporary.</param>
        /// <returns>System.String.</returns>
        /// Changelog:
        /// - 1.0.0 (08-18-2016) - Initial version.
        ///  Changelog:
        ///             - 1.0.1 (08-23-2016) - Added argument for row id to copy.
        ///             - 1.0.0 (08-18-2016) - Initial version.
        public string GetNextLine(int? tmp = null)
        {
            int rowId = 0;
            if (tmp == null)
                rowId = CurrentRowId++;
            else
            {
                rowId = (int)tmp;
            }

            CurrentRowId = rowId + 1;
            if (CurrentRowId >= columnsData[CurrentColumn].Count)
                CurrentRowId = 0;

            return columnsData[CurrentColumn][rowId];
        }

        /// <summary>
        /// To the XML text.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 1.1.6 (09-29-2016) - Fixed bug when loading old saves (bumped save version).
        ///             - 1.1.4 (09-21-2016) - Adjusted saving of ColumnKeys to split the value between two items to prevent a crash during saving.
        ///             - 1.1.3 (08-30-2016) - Removed string.format to new format approach.
        ///             - 1.0.0 (08-18-2016) - Initial version.
        public string ToXmlText()
        {
            var str = new StringBuilder();

            str.AppendLine("<Request>");
            str.AppendLine($"<Name>{Name}</Name>");
            str.AppendLine($"<ID>{ID}</ID>");
            str.AppendLine("<ColumnKeys>");
            foreach (var key in columnKeys)
            {
                str.AppendLine("<ColumnKey>");
                str.AppendLine($"<Key>{key.Key}</Key>");
                str.AppendLine($"<Value>{key.Value}</Value>");
                str.AppendLine("</ColumnKey>");
            }
            str.AppendLine("</ColumnKeys>");

            str.AppendLine("<ColumnsData>");
            foreach (var key in columnKeys)
            {
                str.AppendLine("<Column>");
                str.AppendLine($"<Name>{key.Value}</Name>");
                for (int i = 0; i < columnsData[key.Value].Count; i++)
                    str.AppendLine($"<Row>{columnsData[key.Value][i]}</Row>");
                str.AppendLine("</Column>");
            }
            str.AppendLine("</ColumnsData>");

            str.AppendLine("</Request>");

            return str.ToString();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Cleans the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 1.0.0 (08-18-2016) - Initial version.
        private string CleanText(string text)
        {
            return text.Trim();
        }

        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hasColumnHeaders">if set to <c>true</c> [has column headers].</param>
        /// <param name="cleanData">if set to <c>true</c> [clean data].</param>
        ///  Changelog:
        ///             - 1.1.4 (09-21-2016) - Changed line break splitter...
        ///             - 1.0.0 (08-18-2016) - Initial version.
        private void ParseText(string text, bool hasColumnHeaders, bool cleanData)
        {
            var rawRows = text.Split(splitters, cleanData == true ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

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
                    }
                    else
                    {
                        for (int j = 0; j < columns.Length; j++)
                        {
                            var name = columns[j];
                            columnKeys.Add(j, name);
                            columnsData.Add(name, new List<string>());
                        }
                        continue;
                    }
                }
                
                for (int j = 0; j < columns.Length; j++)
                {
                    var item = columns[j].Trim();
                    if (!string.IsNullOrWhiteSpace(item))
                        columnsData[columnKeys[j]].Add(item);
                }
            }

            NumberOfColumns = columnKeys.Count;
            if (columnKeys.Count > 0)
                CurrentColumn = columnKeys[0];
            else
                CurrentColumn = "";
        }

        #endregion Private Methods
    }
}