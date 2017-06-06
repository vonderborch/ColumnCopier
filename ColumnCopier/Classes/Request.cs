// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Request.cs
// Author           : Christian
// Created          : 08-18-2016
//
// Version          : 1.3.0
// Last Modified By : Christian
// Last Modified On : 05-30-2017
// ***********************************************************************
// <copyright file="Request.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Request class.
// </summary>
//
// Changelog:
//            - 2.0.0 (xx-xx-2017) - Rebuilt!
//            - 1.3.0 (05-30-2017) - More extensive text cleaning.
//            - 1.2.0 (09-30-2016) - Added preserve request toggle support.
//            - 1.1.6 (09-29-2016) - Fixed bug when loading old saves (bumped save version), fixed bug when copying invalid characters, fixed bug with no data in the clipboard
//            - 1.1.5 (09-21-2016) - Added request exporting functionality.
//            - 1.1.4 (09-21-2016) - Enhanced robustness of new line splitter. Adjusted saving of ColumnKeys to split the value between two items to prevent a crash during saving.
//            - 1.1.3 (08-30-2016) - Removed string. format to new format approach when saving a request.
//            - 1.0.0 (08-22-2016) - Finished initial code.
//            - 0.0.0 (08-18-2016) - Initial version created.
// ***********************************************************************
using ColumnCopier.Enums;
using ColumnCopier.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ColumnCopier.Classes
{
    /// <summary>
    /// Request class.
    /// </summary>
    public class Request
    {
        #region Private Fields

        private Dictionary<int, string> columnKeys = new Dictionary<int, string>();
        private Dictionary<string, ColumnData> columnsData = new Dictionary<string, ColumnData>();
        private int currentColumn = 0;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Used when importing data.
        /// Initializes a new instance of the <see cref="Request" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="rawText">The raw text.</param>
        /// <param name="hasHeaders">Whether the text has headers or not.</param>
        /// <param name="cleanText">Whether to clean the raw text or not.</param>
        /// <param name="removeEmptyLines">Whether to remove empty lines or not.</param>
        /// <param name="defaultColumnIndex">Default index of the column.</param>
        /// <param name="defaultColumnName">Default name of the column.</param>
        /// <param name="columnNameMatch">The column name match.</param>
        /// <param name="defaultColumnPriority">The default column priority.</param>
        public Request(int id, string rawText, bool hasHeaders, bool cleanText, bool removeEmptyLines, 
            int defaultColumnIndex, string defaultColumnName, int columnNameMatch, DefaultColumnPriority defaultColumnPriority)
        {
            Id = id;
            Name = string.Format(Constants.Instance.FormatRequestName, id);

            ParseText(rawText, hasHeaders, cleanText, removeEmptyLines);
            CalculateDefaultColumn(defaultColumnIndex, defaultColumnName, columnNameMatch, defaultColumnPriority);
        }

        /// <summary>
        /// Used when loading a Request from a save file.
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public Request(XElement node)
        {
            foreach (var mainCategory in node.Elements())
            {
                var mainCategoryName = mainCategory.Name.ToString();

                if (mainCategoryName == "Name")
                {
                    Name = mainCategory.Value;
                }
                else if (mainCategoryName == "Id")
                {
                    Id = Converters.ConvertToInt(mainCategory.Value);
                }
                else if (mainCategoryName == "IsPreserved")
                {
                    IsPreserved = Converters.ConvertToBool(mainCategory.Value);
                }
                else if (mainCategoryName == "CurrentColumnIndex")
                {
                    CurrentColumnIndex = Converters.ConvertToIntWithClamp(mainCategory.Value, 0, 0);
                }
                else if (mainCategoryName == "ColumnKeys")
                {
                    foreach (var columnKeyMain in mainCategory.Elements())
                    {
                        if (columnKeyMain.Name.ToString() == "ColumnKey")
                        {
                            var key = -1;
                            var value = string.Empty;

                            foreach (var columnKeys in columnKeyMain.Elements())
                            {
                                switch (columnKeys.Name.ToString())
                                {
                                    case "Key":
                                        key = Converters.ConvertToIntWithClamp(columnKeys.Value, 0, 0);
                                        break;
                                    case "Value":
                                        value = columnKeys.Value;
                                        break;
                                }
                            }

                            columnKeys.Add(key, value);
                            //columnsData.Add(value, new ColumnData());
                        }
                    }
                }
                else if (mainCategoryName == "ColumnsData")
                {
                    foreach (var columnDataMain in mainCategory.Elements())
                    {
                        if (columnDataMain.Name.ToString() == "Column")
                        {
                            var name = string.Empty;
                            var data = new ColumnData();

                            foreach (var cd in columnDataMain.Elements())
                            {
                                var cdName = cd.Name.ToString();

                                if (cdName == "Name")
                                {
                                    name = cd.Value;
                                }
                                else if (cdName == "CurrentNextLine")
                                {
                                    data.CurrentNextLine = Converters.ConvertToIntWithClamp(cd.Value, 0, 0);
                                }
                                else if (cdName == "Rows")
                                {
                                    var rowData = new List<string>();
                                    foreach (var cdRow in cd.Elements())
                                    {
                                        if (cdRow.Name.ToString() == "Row")
                                        {
                                            rowData.Add(cdRow.Value);
                                        }
                                    }
                                    data.Rows = new List<string>(rowData);
                                }
                            }

                            columnsData.Add(name, data);
                        }
                    }
                }
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public int CurrentColumnIndex
        {
            get { return currentColumn; }
            private set { currentColumn = value; }
        }

        public string CurrentColumnName
        {
            get { return columnKeys[currentColumn]; }
        }

        public int Id { get; private set; }
        public bool IsPreserved { get; set; }
        public string Name { get; private set; }

        public int NumberOfColumns
        {
            get { return columnKeys.Count; }
        }

        public int CopyNextLineIndex
        {
            get { return columnsData[CurrentColumnName].CurrentNextLine; }
            set { columnsData[CurrentColumnName].CurrentNextLine = value; }
        }

        public int CurrentColumnRowCount
        {
            get { return columnsData[CurrentColumnName].Rows.Count; }
        }

        #endregion Public Properties

        #region Public Methods

        public string ConvertRequestToXml()
        {
            var str = new StringBuilder();

            str.AppendLine("<Request>");

            str.AppendLine($"<Name>{Name}</Name>");
            str.AppendLine($"<Id>{Id}</Id>");
            str.AppendLine($"<IsPreserved>{IsPreserved}</IsPreserved>");
            str.AppendLine($"<CurrentColumnIndex>{CurrentColumnIndex}</CurrentColumnIndex>");

            str.AppendLine("<ColumnKeys>");
            foreach (var key in columnKeys)
            {
                str.AppendLine("<ColumnKey>");
                str.AppendLine($"<Key>{key.Key}</Key>");
                str.AppendLine($"<Value>{XmlTextHelpers.ConvertForXml(key.Value)}</Value>");
                str.AppendLine("</ColumnKey>");
            }
            str.AppendLine("</ColumnKeys>");

            str.AppendLine("<ColumnsData>");
            foreach (var key in columnsData)
            {
                str.AppendLine("<Column>");
                str.AppendLine($"<Name>{XmlTextHelpers.ConvertForXml(key.Key)}</Name>");
                str.AppendLine($"<CurrentNextLine>{key.Value.CurrentNextLine}</CurrentNextLine>");
                str.AppendLine($"<Rows>");
                for (var i = 0; i < key.Value.Rows.Count; i++)
                {
                    str.AppendLine($"<Row>{XmlTextHelpers.ConvertForXml(key.Value.Rows[i])}</Row>");
                }
                str.AppendLine($"</Rows>");
                str.AppendLine("</Column>");
            }
            str.AppendLine("</ColumnsData>");

            str.AppendLine("</Request>");
            return str.ToString();
        }

        public string ExportRequest()
        {
            var str = new StringBuilder();

            // export headers
            var rawHeader = new StringBuilder();
            foreach (var item in columnKeys)
                rawHeader.AppendFormat("{0}\t", item.Value);
            var header = rawHeader.ToString();
            str.AppendFormat("{0}{1}", header.Remove(header.Length - 1), Environment.NewLine);

            // export rows
            var maxColumn = int.MinValue;
            foreach (var column in columnsData)
            {
                if (column.Value.Rows.Count > maxColumn)
                    maxColumn = column.Value.Rows.Count;
            }

            for (var i = 0; i < maxColumn; i++)
            {
                var rawRow = new StringBuilder();
                foreach (var column in columnsData)
                {
                    if (i < column.Value.Rows.Count)
                        rawRow.AppendFormat("{0}\t", column.Value.Rows[i]);
                }

                var row = rawRow.ToString();
                str.AppendFormat("{0}{1}", row.Remove(row.Length - 1), Environment.NewLine);
            }

            return XmlTextHelpers.ConvertFromXml(str.ToString());
        }

        public string GetCurrentColumnNextLineText()
        {
            var text = XmlTextHelpers.ConvertFromXml(columnsData[CurrentColumnName].Rows[columnsData[CurrentColumnName].CurrentNextLine++]);

            if (columnsData[CurrentColumnName].CurrentNextLine >= CurrentColumnRowCount)
                columnsData[CurrentColumnName].CurrentNextLine = 0;

            return text;
        }

        public List<string> GetColumnNames()
        {
            var result = new List<string>();
            foreach (var key in columnKeys.Values)
                result.Add(XmlTextHelpers.ConvertFromXml(key));

            return result;
        }

        public string GetCurrentColumnText()
        {
            var str = new StringBuilder();
            foreach (var line in columnsData[CurrentColumnName].Rows)
                str.AppendLine(XmlTextHelpers.ConvertFromXml(line));

            return str.ToString();
        }

        public List<string> GetColumnRawText()
        {
            var result = new List<string>();
            foreach (var line in columnsData[CurrentColumnName].Rows)
                result.Add(XmlTextHelpers.ConvertFromXml(line));

            return result;
        }

        public bool SetCurrentColumn(int i)
        {
            if (!columnKeys.ContainsKey(i))
                return false;

            currentColumn = i;
            return true;
        }

        #endregion Public Methods

        #region Private Methods

        private void CalculateDefaultColumn(int defaultColumnIndex, string defaultColumnName, int columnNameMatch, DefaultColumnPriority defaultColumnPriority)
        {
            var index = -1;

            switch (defaultColumnPriority)
            {
                case DefaultColumnPriority.Number:
                    if (defaultColumnIndex >= 0)
                        index = defaultColumnIndex;
                    break;
                case DefaultColumnPriority.Name:
                    var currentClosestDistance = int.MaxValue;
                    var currentClosestId = int.MinValue;
                    foreach (var pair in columnKeys)
                    {
                        var distance = MathHelpers.ComputeDifference(pair.Value, defaultColumnName);
                        if (distance < currentClosestDistance)
                        {
                            currentClosestDistance = distance;
                            currentClosestId = pair.Key;
                        }
                    }

                    if (currentClosestDistance <= columnNameMatch)
                        index = currentClosestId;
                    break;
            }

            currentColumn = MathHelpers.ClampInt(index, 0, columnKeys.Count - 1);
        }

        private void ParseText(string text, bool hasHeaders, bool cleanText, bool removeEmptyLines)
        {
            var rawRows = text.Split(Constants.Instance.SplittersRow, removeEmptyLines == true ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

            for (var i = 0; i < rawRows.Length; i++)
            {
                // clean the row and then split it into columns
                var row = cleanText ? XmlTextHelpers.ConvertForXml(rawRows[i]) : rawRows[i];
                var columns = row.Split(Constants.Instance.SplittersColumn);

                // if we're on the header row...
                if (i == 0)
                {
                    // if we've been told to generate headers ourselves...
                    if (!hasHeaders)
                    {
                        for (var j = 0; j < columns.Length; j++)
                        {
                            var name = string.Format(Constants.Instance.FormatColumnName, j);
                            columnKeys.Add(j, name);
                            columnsData.Add(name, new ColumnData()
                                {
                                    CurrentNextLine = 0,
                                    Rows = new List<string>()
                                });
                        }
                    }
                    // if we've been told to use the first row as headers...
                    else
                    {
                        for (var j = 0; j < columns.Length; j++)
                        {
                            var name = columns[j];
                            columnKeys.Add(j, name);
                            columnsData.Add(name, new ColumnData()
                                {
                                    CurrentNextLine = 0,
                                    Rows = new List<string>()
                                });
                        }
                        // now go to i=1 since we've already used this row of data
                        continue;
                    }
                }

                // now add the columns to their appropriate column...
                for (var j = 0; j < columns.Length; j++)
                {
                    columnsData[columnKeys[j]].Rows.Add(columns[j] == null ? string.Empty : columns[j]);
                }
            }

            // find out the meta data about this request...
            if (true) ;
        }

        #endregion Private Methods
    }
}