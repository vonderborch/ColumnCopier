using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnCopier.Helpers
{
    public static class StringHelpers
    {
        public static string ConvertToSafeText(string text)
        {
            return text;
        }

        public static string ConvertFromSafeText(string text)
        {
            return text;
        }

        public static int ComputeDifference(string a, string b)
        {
            int n = a.Length;
            int m = b.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
                return m;
            if (m == 0)
                return n;

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    // Step 3a
                    int cost = (b[j - 1] == a[i - 1]) ? 0 : 1;

                    // Step 3b
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            // Step 4
            return d[n, m];
        }
    }
}
