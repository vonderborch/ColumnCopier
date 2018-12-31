
using System.Runtime.Serialization;

namespace ColumnCopier.Classes
{
    [DataContract]
    [KnownType(typeof(LineSeperatorOption))]
    public class LineSeperatorOption
    {
        public string Name
        {
            get { return $"{PreString}-{InterString}-{PostString}"; }
        }

        [DataMember]
        public string PreString = string.Empty;

        [DataMember]
        public string InterString = string.Empty;

        [DataMember]
        public string PostString = string.Empty;

        public LineSeperatorOption()
        {

        }

        public LineSeperatorOption(string pre, string inter, string post)
        {
            PreString = pre;
            InterString = inter;
            PostString = post;
        }
    }
}
