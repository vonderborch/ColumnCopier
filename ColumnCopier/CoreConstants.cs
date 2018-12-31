using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnCopier
{
    public abstract class CoreConstants
    {
        public static string ProgramVersionString = "3.0.0";
        public static int ProgramVersion = 300;
        public static int SaveVersion = 30;
        public static int SaveVersionMinimum = 30;

        public static string FormatColumnName { get; private set; } = "Column {0}";

        public static string SaveExtension { get; private set; } = ".ccx";

        public static string[] SplittersColumns { get; private set; } = { "\t" };

        public static string[] SplittersRows { get; private set; } = { "\r\n", "\n", "\r" };
    }
}
