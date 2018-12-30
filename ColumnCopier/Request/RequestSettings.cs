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
    [KnownType(typeof(RequestSettings))]
    public class RequestSettings
    {
        [DataMember]
        public HeaderMode HeaderMode { get; set; } = HeaderMode.HasHeaders;

        [DataMember]
        public bool RemoveBlankLines { get; set; } = false;

        [DataMember]
        public bool CleanInputText { get; set; } = true;

        public RequestSettings()
        {

        }

        public RequestSettings(RequestSettings oldSettings)
        {
            HeaderMode = oldSettings.HeaderMode;
            RemoveBlankLines = oldSettings.RemoveBlankLines;
            CleanInputText = oldSettings.CleanInputText;
        }
    }
}
