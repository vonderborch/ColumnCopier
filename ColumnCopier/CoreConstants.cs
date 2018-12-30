using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnCopier
{
    public abstract class CoreConstants
    {
        public static string FormatColumnName { get; private set; } = "Column {0}";

        public static string SaveExtension { get; private set; } = ".ccx";

        public static string[] SplittersColumns { get; private set; } = { "\t" };

        public static string[] SplittersRows { get; private set; } = { "\r\n", "\n", "\r" };
    }
}
