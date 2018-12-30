// ***********************************************************************
// Assembly         : ColumnCopier
// Author           : Christian
// Created          : 2018-06-19
//
// Version          : 3.0.0
// Last Modified By : Christian
// Last Modified On : 2018-09-16
// ***********************************************************************
// <copyright file="Request.cs" company="">
//     Copyright ©  2016 - 2018
// </copyright>
// <summary>
//      Stores information on a column
// </summary>
//
// Changelog: 
//            - 3.0.0 (2018-09-16) - Merged Request and RequestData classes
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
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ColumnCopier.Enums;
using ColumnCopier.Helpers;

namespace ColumnCopier.Request
{
    /// <summary>
    /// Class Request.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Request))]
    [KnownType(typeof(ColumnData))]
    public class Request
    {
        [DataMember]
        public List<ColumnData> Columns = new List<ColumnData>();

        private Dictionary<int, string> _columnNames = null;

        [DataMember]
        public int CurrentColumnIndex { get; set; } = 0;

        [DataMember]
        public int Id { get; set; } = 0;

        [DataMember]
        public bool IsPreserved { get; set; } = false;

        [DataMember]
        public string Name { get; set; } = string.Empty;

        [DataMember]
        public RequestSettings Settings { get; set; } = null;

        public string CurrentColumnName => Columns[CurrentColumnIndex].Name;

        public string CurrentColumnText
        {
            get { return Columns[CurrentColumnIndex].Text; }
            set { Columns[CurrentColumnIndex].Text = value; }
        }

        public string GetRowText(int row)
        {
            return Columns[CurrentColumnIndex].GetRowText(row);
        }

        public Dictionary<int, string> ColumnNames
        {
            get
            {
                if (_columnNames == null)
                {
                    _columnNames = new Dictionary<int, string>();

                    for (var i = 0; i < Columns.Count; i++)
                    {
                        _columnNames.Add(i, Columns[i].Name);
                    }
                }

                return _columnNames;
            }
        }

        public Request()
        {

        }

        public Request(Request request)
        {

        }

        public Request(string text, RequestSettings settings, int requestId, DefaultColumnSettings columnSettings)
        {
            Id = requestId;

            ImportRequest(text, settings);
            CalculateDefaultColumn(columnSettings);
        }

        public string ExportRequest()
        {
            var output = new StringBuilder();
            var maxLength = int.MinValue;

            // export headers (and determine longest column)
            foreach (var column in Columns)
            {
                output.AppendFormat("{0}\t", column.Name);

                if (column.Rows.Count > maxLength)
                {
                    maxLength = column.Rows.Count;
                }
            }
            output.AppendFormat("{0}", Environment.NewLine);

            // export rows
            var rawRow = new StringBuilder();
            for (var i = 0; i < maxLength; i++)
            {
                for (var j = 0; j < Columns.Count; j++)
                {
                    rawRow.AppendFormat("{0}\t", Columns[j].Rows.Count <= j ? Columns[j].Rows[i] : String.Empty);
                }

                var row = rawRow.ToString();
                output.AppendFormat("{0}{1}", row.Remove(row.Length - 1), Environment.NewLine);
                rawRow.Clear();
            }

            // output the text
            return StringHelpers.ConvertFromSafeText(output.ToString());
        }

        private void CalculateDefaultColumn(DefaultColumnSettings columnSettings)
        {
            var index = -1;

            switch (columnSettings.DefaultColumnPriority)
            {
                case DefaultColumnPriority.Number:
                    index = columnSettings.DefaultColumnIndex;
                    break;
                case DefaultColumnPriority.Name:
                    var currentClosestDistance = int.MaxValue;

                    foreach (var pair in ColumnNames)
                    {
                        var distance =
                            StringHelpers.ComputeDifference(pair.Value, columnSettings.DefaultColumnNameMatch);

                        if (distance >= currentClosestDistance || distance > columnSettings.ColumnNameMatchSimilarity)
                            continue;

                        currentClosestDistance = distance;
                        index = pair.Key;
                    }

                    break;
            }

            CurrentColumnIndex = MathHelpers.Clamp(index, 0, Columns.Count - 1);
        }

        private void ImportRequest(string text, RequestSettings settings)
        {
            // split the text by line breaks...
            var rawRows = StringHelpers.ConvertToSafeText(text).Split(CoreConstants.SplittersRows,
                settings.RemoveBlankLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            
            // for the first row...
            var rowStart = 0;
            {
                var columns = rawRows[0].Split(CoreConstants.SplittersColumns, StringSplitOptions.None);

                for (var j = 0; j < columns.Length; j++)
                {
                    var name = string.Empty;

                    switch (settings.HeaderMode)
                    {
                        // if we can expect row headers, we'll use the cell name
                        case HeaderMode.HasHeaders:
                            name = columns[j];
                            rowStart = 1;
                            break;
                        // otherwise, we'll generate unique names for each column
                        case HeaderMode.BestGuess:
                        case HeaderMode.NoHeaders:
                        default:
                            name = string.Format(CoreConstants.FormatColumnName, j);
                            break;
                    }
                    
                    Columns.Add(new ColumnData(name));
                }
            }

            // for each row...
            for (var i = rowStart; i < rawRows.Length; i++)
            {
                // split the row into the columns within the row
                var columns = rawRows[i].Split(CoreConstants.SplittersColumns, StringSplitOptions.None);

                for (var j = 0; j < columns.Length; j++)
                {
                    // then add either the cell or an empty string (depending on whether the cell is empty or not)
                    Columns[j].Rows.Add(columns[j] ?? string.Empty);
                }
            }

            // TODO: code for renaming columns if in HeaderMode.BestGuess mode

            // TODO: find out any needed metadata for this request
        }
    }
}
