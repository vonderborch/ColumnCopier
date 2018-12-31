// ***********************************************************************
// Assembly         : ColumnCopier
// Author           : Christian
// Created          : 2018-06-19
//
// Version          : 3.0.0
// Last Modified By : Christian
// Last Modified On : 2018-09-16
// ***********************************************************************
// <copyright file="ColumnData.cs" company="">
//     Copyright ©  2016 - 2018
// </copyright>
// <summary>
//      Stores information on a column
// </summary>
//
// Changelog: 
//            - 3.0.0 (2018-09-16) - Now stores information on the name of the column.
//            - 2.0.0 (2017-06-06) - Initial version created.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.Serialization;
using System.Text;
using ColumnCopier.Helpers;

namespace ColumnCopier.Classes
{
    /// <summary>
    /// Class ColumnData.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ColumnData))]
    public class ColumnData
    {
        /// <summary>
        /// The column's name
        /// </summary>
        [DataMember]
        public string Name = string.Empty;

        /// <summary>
        /// The current line
        /// </summary>
        [DataMember]
        public int CurrentLine = 0;

        /// <summary>
        /// The rows
        /// </summary>
        [DataMember]
        public List<string> Rows = new List<string>();

        private string text = string.Empty;

        public ColumnData(string name)
        {
            Name = name;
            CurrentLine = 0;
            Rows = new List<string>();
            text = string.Empty;
        }

        public string GetAndIncrementRowText()
        {
            if (CurrentLine >= Rows.Count) CurrentLine = 0;

            return Rows[CurrentLine++];
        }

        public string GetRowText(int row)
        {
            if (row < 0 || row >= Rows.Count)
            {
                throw new System.Exception($"Row [{row}] is out of bounds for column {Name}!");
            }

            return Rows[row];
        }

        public string Text
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(text)) return text;

                var str = new StringBuilder();

                foreach (var line in Rows)
                    str.AppendLine(StringHelpers.ConvertFromSafeText(line));

                text = str.ToString();
                return text;
            }

            set
            {
                Rows = new List<string>(StringHelpers.ConvertToSafeText(value)
                    .Split(CoreConstants.SplittersRows, StringSplitOptions.None));

                text = string.Empty;
            }
        }
    }
}
