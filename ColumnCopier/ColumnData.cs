using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ColumnCopier
{
    [DataContract]
    [KnownType(typeof(ColumnData))]
    public class ColumnData
    {
        [DataMember]
        public int CurrentLine = 0;

        [DataMember]
        public List<string> Rows = new List<string>();
    }
}
