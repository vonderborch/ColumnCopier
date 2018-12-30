using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ColumnCopier.Enums;

namespace ColumnCopier.Request
{
    [DataContract]
    [KnownType(typeof(DefaultColumnSettings))]
    public class DefaultColumnSettings
    {
        [DataMember]
        public int DefaultColumnIndex { get; set; } = 0;

        [DataMember]
        public string DefaultColumnNameMatch { get; set; } = string.Empty;

        [DataMember]
        public int ColumnNameMatchSimilarity { get; set; } = 5;

        [DataMember]
        public DefaultColumnPriority DefaultColumnPriority { get; set; } = DefaultColumnPriority.Number;
    }
}
