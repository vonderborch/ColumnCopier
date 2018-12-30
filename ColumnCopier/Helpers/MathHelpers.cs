using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnCopier.Helpers
{
    public static class MathHelpers
    {
        public static int Clamp(int value, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            return value < minValue
                ? minValue
                : value > maxValue
                    ? maxValue
                    : value;
        }
    }
}
