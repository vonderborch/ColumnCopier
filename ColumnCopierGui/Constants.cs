using ColumnCopier;

namespace ColumnCopierGui
{
    public partial class Constants : CoreConstants
    {
        private Constants()
        {
            //InitializeXmlReplacements();
        }

        public static Constants Instance { get; } = new Constants();


    }
}