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
using ColumnCopier.Helpers;
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

        private Dictionary<int, string> columnKeys = new Dictionary<int, string>();
        private Dictionary<string, ColumnData> columnsData = new Dictionary<string, ColumnData>();
        private int currentColumn = 0;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Used when importing data.
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="rawText">The raw text.</param>
        /// <param name="hasHeaders">Whether the text has headers or not.</param>
        /// <param name="cleanText">Whether to clean the raw text or not.</param>
        /// <param name="removeEmptyLines">Whether to remove empty lines or not.</param>
        public Request(int id, string rawText, bool hasHeaders, bool cleanText, bool removeEmptyLines, 
            int defaultColumnIndex, string defaultColumnName, bool useNamePriority)
        {
            Id = id;
            Name = string.Format(Constants.Instance.FormatRequestName, id);

            ParseText(rawText, hasHeaders, cleanText, removeEmptyLines);
            CalculateDefaultColumn(defaultColumnIndex, defaultColumnName, useNamePriority);
        }

        /// <summary>
        /// Used when loading a Request from a save file.
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public Request(XElement node)
        {
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

        public int CopyNextLineIndex { get; set; }

        #endregion Public Properties

        #region Public Methods

        public string ConvertRequestToXml()
        {
            return null;
        }

        public string ExportRequest()
        {
            return null;
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

        public bool SetCurrentColumn(int i)
        {
            if (!columnKeys.ContainsKey(i))
                return false;

            currentColumn = i;
            return true;
        }

        #endregion Public Methods

        #region Private Methods

        private void CalculateDefaultColumn(int defaultColumnIndex, string defaultColumnName, bool useNamePriority)
        {
            currentColumn = columnKeys.Count > 0
                ? 0
                : 0;
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