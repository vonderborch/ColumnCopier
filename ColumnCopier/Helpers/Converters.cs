// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Converters.cs
// Author           : Christian
// Created          : 05-31-2017
//
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 05-31-2017
// ***********************************************************************
// <copyright file="Converters.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Contains various text converter methods.
// </summary>
//
// Changelog:
//            - 2.0.0 (05-31-2017) - Initial Version
// ***********************************************************************

/// <summary>
/// The Helpers namespace.
/// </summary>
namespace ColumnCopier.Helpers
{
    /// <summary>
    /// Class Converters.
    /// </summary>
    public static class Converters
    {
        #region Public Methods

        /// <summary>
        /// Converts text to a bool.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 2.0.0 (05-31-2017) - Initial version.
        public static bool ConvertToBool(string text, bool defaultValue = false)
        {
            bool output;
            if (bool.TryParse(text, out output))
                return output;
            return defaultValue;
        }

        /// <summary>
        /// Converts text to an int.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Int32.</returns>
        ///  Changelog:
        ///             - 2.0.0 (05-31-2017) - Initial version.
        public static int ConvertToInt(string text, int defaultValue = 0)
        {
            int output;
            if (int.TryParse(text, out output))
                return output;
            return defaultValue;
        }

        /// <summary>
        /// Converts text to an int.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>System.Int32.</returns>
        /// Changelog:
        ///             - 2.0.0 (05-31-2017) - Initial version.
        public static int ConvertToIntWithClamp(string text, int defaultValue = 0, int min = int.MinValue, int max = int.MaxValue)
        {
            int output;
            if (!int.TryParse(text, out output))
                output = defaultValue;

            if (output < min)
                output = min;
            if (output > max)
                output = max;

            return output;
        }

        /// <summary>
        /// Converts text to a long.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Int64.</returns>
        ///  Changelog:
        ///             - 2.0.0 (05-31-2017) - Initial version.
        public static long ConvertToLong(string text, long defaultValue = 0)
        {
            long output;
            if (long.TryParse(text, out output))
                return output;
            return defaultValue;
        }

        /// <summary>
        /// Converts text to a uint.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.UInt32.</returns>
        ///  Changelog:
        ///             - 2.0.0 (05-31-2017) - Initial version.
        public static uint ConvertToUint(string text, uint defaultValue = 0)
        {
            uint output;
            if (uint.TryParse(text, out output))
                return output;
            return defaultValue;
        }

        /// <summary>
        /// Converts text to a ulong.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.UInt64.</returns>
        ///  Changelog:
        ///             - 2.0.0 (05-31-2017) - Initial version.
        public static ulong ConvertToUlong(string text, ulong defaultValue = 0)
        {
            ulong output;
            if (ulong.TryParse(text, out output))
                return output;
            return defaultValue;
        }

        #endregion Public Methods
    }
}