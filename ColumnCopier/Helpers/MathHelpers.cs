// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : MathHelpers.cs
// Author           : Christian
// Created          : 06-01-2017
// 
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-01-2017
// ***********************************************************************
// <copyright file="MathHelpers.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the MathHelpers class.
// </summary>
//
// Changelog: 
//            - 2.0.0 (06-01-2017) - Initial version created.
// ***********************************************************************
using System;

namespace ColumnCopier.Helpers
{
    /// <summary>
    /// Class MathHelpers.
    /// </summary>
    public static class MathHelpers
    {
        #region Public Methods

        /// <summary>
        /// Clamps the int.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-01-2017) - Initial version.
        public static int ClampInt(int input, int min = int.MinValue, int max = int.MaxValue)
        {
            return input < min
                ? min
                : input > max
                    ? max
                    : input;
        }

        /// <summary>
        /// Computes the difference.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-01-2017) - Initial version.
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

        #endregion Public Methods
    }
}