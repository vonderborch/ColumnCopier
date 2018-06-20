using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ColumnCopier
{
    [DataContract]
    [KnownType(typeof(RequestData))]
    public class RequestData
    {
        [DataMember]
        public Dictionary<int, string> Keys;
        [DataMember]
        public Dictionary<string, ColumnData> Values;

        [DataMember]
        public int CurrentColumnIndex { get; set; } = 0;

        [DataMember]
        public int Id { get; set; } = 0;

        [DataMember]
        public bool IsPreserved { get; set; } = false;

        [DataMember]
        public string Name { get; set; } = string.Empty;
    }
}
