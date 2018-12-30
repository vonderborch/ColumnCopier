using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ColumnCopier.Request;

namespace ColumnCopier
{
    [DataContract]
    [KnownType(typeof(State))]
    [KnownType(typeof(Request.Request))]
    [KnownType(typeof(ColumnData))]
    public class State
    {
        [DataMember]
        public List<string> History;

        [DataMember]
        public List<string> PreservedRequests;

        [DataMember]
        public Dictionary<string, Request.Request> RequestHistory;
        

    }
}
