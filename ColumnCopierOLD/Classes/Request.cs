// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Request.cs
// Author           : Christian
// Created          : 08-18-2016
//
// Version          : 2.2.2
// Last Modified By : Christian
// Last Modified On : 05-17-2018
// ***********************************************************************
// <copyright file="Request.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Request class.
// </summary>
//
// Changelog:
//            - 2.2.2 (05-17-2018) - Fixed column name generation from header row crashing if column name already exists.
//            - 2.2.0 (07-14-2017) - Added method to get the text of any column and any row of any column.
//            - 2.1.0 (06-07-2017) - Moved most fields/properties to the RequestData class. Revised saving/loading system to use JSON data serialization.
//            - 2.0.0 (06-06-2017) - Rebuilt!
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
using System.Runtime.Serialization;
using System.Text;

namespace ColumnCopier.Classes
{
    /// <summary>
    /// Request class.
    /// </summary>
    [DataContract]
    [KnownType(typeof(RequestData))]
    public class Request
    {
        #region Private Fields

        [DataMember]
        private RequestData request;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        public Request()
        {
            request = new RequestData()
            {
                ColumnData = new Dictionary<string, Classes.ColumnData>(),
                ColumnKeys = new Dictionary<int, string>(),
            };
        }

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
            request = new RequestData()
            {
                ColumnData = new Dictionary<string, Classes.ColumnData>(),
                ColumnKeys = new Dictionary<int, string>(),
            };
            Id = id;
            Name = string.Format(Constants.Instance.FormatRequestName, id);

            ParseText(rawText, hasHeaders, cleanText, removeEmptyLines);
            CalculateDefaultColumn(defaultColumnIndex, defaultColumnName, columnNameMatch, defaultColumnPriority);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the index of the current column.
        /// </summary>
        /// <value>The index of the current column.</value>
        public int CurrentColumnIndex
        {
            get { return request.CurrentColumnIndex; }
            private set { request.CurrentColumnIndex = value; }
        }

        /// <summary>
        /// Gets the name of the current column.
        /// </summary>
        /// <value>The name of the current column.</value>
        public string CurrentColumnName
        {
            get { return request.ColumnKeys[CurrentColumnIndex]; }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id
        {
            get { return request.Id; }
            private set { request.Id = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is preserved.
        /// </summary>
        /// <value><c>true</c> if this instance is preserved; otherwise, <c>false</c>.</value>
        public bool IsPreserved
        {
            get { return request.IsPreserved; }
            set { request.IsPreserved = value; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return request.Name; }
            private set { request.Name = value; }
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <value>The number of columns.</value>
        public int NumberOfColumns
        {
            get { return request.ColumnKeys.Count; }
        }

        /// <summary>
        /// Gets or sets the index of the copy next line.
        /// </summary>
        /// <value>The index of the copy next line.</value>
        public int CopyNextLineIndex
        {
            get { return request.ColumnData[CurrentColumnName].CurrentNextLine; }
            set { request.ColumnData[CurrentColumnName].CurrentNextLine = value; }
        }

        /// <summary>
        /// Gets the current column row count.
        /// </summary>
        /// <value>The current column row count.</value>
        public int CurrentColumnRowCount
        {
            get { return request.ColumnData[CurrentColumnName].Rows.Count; }
        }

        /// <summary>
        /// Gets the request data.
        /// </summary>
        /// <value>The request data.</value>
        public RequestData RequestData
        {
            get { return request; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Converts the request to XML.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public string ConvertRequestToXml()
        {
            var str = new StringBuilder();
            /*

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
            */
            return str.ToString();
        }

        /// <summary>
        /// Exports the request.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public string ExportRequest()
        {
            var str = new StringBuilder();

            // export headers
            var rawHeader = new StringBuilder();
            foreach (var item in request.ColumnKeys)
                rawHeader.AppendFormat("{0}\t", item.Value);
            var header = rawHeader.ToString();
            str.AppendFormat("{0}{1}", header.Remove(header.Length - 1), Environment.NewLine);

            // export rows
            var maxColumn = int.MinValue;
            foreach (var column in request.ColumnData)
            {
                if (column.Value.Rows.Count > maxColumn)
                    maxColumn = column.Value.Rows.Count;
            }

            for (var i = 0; i < maxColumn; i++)
            {
                var rawRow = new StringBuilder();
                foreach (var column in request.ColumnData)
                {
                    if (i < column.Value.Rows.Count)
                        rawRow.AppendFormat("{0}\t", column.Value.Rows[i]);
                }

                var row = rawRow.ToString();
                str.AppendFormat("{0}{1}", row.Remove(row.Length - 1), Environment.NewLine);
            }

            return XmlTextHelpers.ConvertFromXml(str.ToString());
        }

        /// <summary>
        /// Gets the column names.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public List<string> GetColumnNames()
        {
            var result = new List<string>();
            foreach (var key in request.ColumnKeys.Values)
                result.Add(XmlTextHelpers.ConvertFromXml(key));

            return result;
        }

        /// <summary>
        /// Gets the column row text.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="rowId">The row identifier.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public string GetColumnRowText(string columnName, int rowId)
        {
            return request.ColumnData[columnName].Rows[rowId];
        }

        /// <summary>
        /// Gets the column text.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public string GetColumnText(string columnName)
        {
            var str = new StringBuilder();
            foreach (var line in request.ColumnData[columnName].Rows)
                str.AppendLine(XmlTextHelpers.ConvertFromXml(line));

            return str.ToString();
        }

        /// <summary>
        /// Gets the current column next line text.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public string GetCurrentColumnNextLineText()
        {
            var text = XmlTextHelpers.ConvertFromXml(request.ColumnData[CurrentColumnName].Rows[request.ColumnData[CurrentColumnName].CurrentNextLine++]);

            if (request.ColumnData[CurrentColumnName].CurrentNextLine >= CurrentColumnRowCount)
                request.ColumnData[CurrentColumnName].CurrentNextLine = 0;

            return text;
        }

        /// <summary>
        /// Gets the current column text.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public string GetCurrentColumnText()
        {
            var str = new StringBuilder();
            foreach (var line in request.ColumnData[CurrentColumnName].Rows)
                str.AppendLine(XmlTextHelpers.ConvertFromXml(line));

            return str.ToString();
        }

        /// <summary>
        /// Gets the column raw text.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public List<string> GetColumnRawText()
        {
            var result = new List<string>();
            foreach (var line in request.ColumnData[CurrentColumnName].Rows)
                result.Add(XmlTextHelpers.ConvertFromXml(line));

            return result;
        }

        /// <summary>
        /// Sets the current column.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
        public bool SetCurrentColumn(int i)
        {
            if (!request.ColumnKeys.ContainsKey(i))
                return false;

            CurrentColumnIndex = i;
            return true;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Calculates the default column.
        /// </summary>
        /// <param name="defaultColumnIndex">Default index of the column.</param>
        /// <param name="defaultColumnName">Default name of the column.</param>
        /// <param name="columnNameMatch">The column name match.</param>
        /// <param name="defaultColumnPriority">The default column priority.</param>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version.
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
                    foreach (var pair in request.ColumnKeys)
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

            CurrentColumnIndex = MathHelpers.ClampInt(index, 0, request.ColumnKeys.Count - 1);
        }

        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hasHeaders">if set to <c>true</c> [has headers].</param>
        /// <param name="cleanText">if set to <c>true</c> [clean text].</param>
        /// <param name="removeEmptyLines">if set to <c>true</c> [remove empty lines].</param>
        ///  Changelog:
        ///             - 2.2.1 (06-06-2017) - Initial version.
        ///             - 2.0.0 (06-06-2017) - Initial version.
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
                            request.ColumnKeys.Add(j, name);
                            request.ColumnData.Add(name, new ColumnData()
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
                            var k = 0;
                            // find an unused name (if needed)
                            while (request.ColumnKeys.ContainsValue(name))
                            {
                                name = $"{name}{(++k)}";
                            }

                            request.ColumnKeys.Add(j, name);
                            request.ColumnData.Add(name, new ColumnData()
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
                    request.ColumnData[request.ColumnKeys[j]].Rows.Add(columns[j] == null ? string.Empty : columns[j]);
                }
            }

            // find out the meta data about this request...

        }

        #endregion Private Methods
    }
}