using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ColumnCopier
{
    [DataContract]
    [KnownType(typeof(State))]
    public class State
    {
        [DataMember]
        public List<string> History;

        [DataMember]
        public List<string> PreservedRequests;

        [DataMember]
        public Dictionary<string, RequestCacheLevel> RequestHistory;
    }
}
