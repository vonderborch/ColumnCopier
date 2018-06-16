// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : XmlTextHelpers.cs
// Author           : Christian
// Created          : 05-31-2017
//
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 05-31-2017
// ***********************************************************************
// <copyright file="XmlTextHelpers.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Contains text to and from XML parsing methods.
// </summary>
//
// Changelog:
//            - 2.0.0 (05-31-2017) - Initial Version
// ***********************************************************************

using System.Text;

/// <summary>
/// The Helpers namespace.
/// </summary>
namespace ColumnCopier.Helpers
{
    /// <summary>
    /// Class XmlTextHelpers.
    /// </summary>
    public static class XmlTextHelpers
    {
        #region Public Methods

        /// <summary>
        /// Converts for XML.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.0.0 (05-31-2017) - Initial version.
        public static string ConvertForXml(string text)
        {
            // trim the text...
            text = text.Trim();

            // now we go through and replace invalid characters...
            StringBuilder result = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                string replace;
                if (Constants.Instance.CharacterReplacements.TryGetValue(c, out replace))
                    result.Append(replace);
                else
                    result.Append(c);
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts from XML.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.0.0 (05-31-2017) - Initial version.
        public static string ConvertFromXml(string text)
        {
            // TO-DO: Find a better way to do this...
            // scan through the XML-reserved character list and replace any instances
            foreach (var pair in Constants.Instance.StringReplacements)
                text = text.Replace(pair.Key, pair.Value);

            return text;
        }

        #endregion Public Methods
    }
}