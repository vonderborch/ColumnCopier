// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Constants.cs
// Author           : Christian
// Created          : 05-30-2017
//
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 05-31-2017
// ***********************************************************************
// <copyright file="Constants.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Constants class.
// </summary>
//
// Changelog:
//            - 2.0.0 (05-31-2017) - Moved more constants over here.
//            - 1.3.0 (05-30-2017) - Initial code.
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// The ColumnCopier namespace.
/// </summary>
namespace ColumnCopier
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public class Constants
    {
        #region Private Fields

        /// <summary>
        /// The instance
        /// </summary>
        private static Constants instance = new Constants();
        /// <summary>
        /// The character replacements
        /// </summary>
        private Dictionary<char, string> characterReplacements;
        /// <summary>
        /// The string replacements
        /// </summary>
        private Dictionary<string, string> stringReplacements;

        #endregion Private Fields

        #region Private Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="Constants"/> class from being created.
        /// </summary>
        private Constants()
        {
            InitializeXmlReplacements();
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static Constants Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Gets the program version.
        /// </summary>
        /// <value>The program version.</value>
        public static int ProgramVersion { get; private set; } = 200;

        /// <summary>
        /// Gets the save version.
        /// </summary>
        /// <value>The save version.</value>
        public static int SaveVersion { get; private set; } = 20;

        public Dictionary<char, string> CharacterReplacements
        {
            get { return characterReplacements; }
        }

        /// <summary>
        /// Gets the name of the format column.
        /// </summary>
        /// <value>The name of the format column.</value>
        public string FormatColumnName { get; private set; } = "Column {0}";

        /// <summary>
        /// Gets the name of the format request.
        /// </summary>
        /// <value>The name of the format request.</value>
        public string FormatRequestName { get; private set; } = "Request {0}";

        /// <summary>
        /// Gets the git hub API URL.
        /// </summary>
        /// <value>The git hub API URL.</value>
        public Uri GitHubApiUrl { get; private set; } = new Uri("https://api.github.com/");

        /// <summary>
        /// Gets the git hub dot COM URL.
        /// </summary>
        /// <value>The git hub dot COM URL.</value>
        public Uri GitHubDotComUrl { get; private set; } = new Uri("https://github.com/");

        /// <summary>
        /// Gets the git hub repo owner.
        /// </summary>
        /// <value>The git hub repo owner.</value>
        public string GitHubRepoOwner { get; private set; } = "vonderborch";

        /// <summary>
        /// Gets the git hub repository.
        /// </summary>
        /// <value>The git hub repository.</value>
        public string GitHubRepository { get; private set; } = "ColumnCopier";

        /// <summary>
        /// Gets the save extension.
        /// </summary>
        /// <value>The save extension.</value>
        public string SaveExtension { get; private set; } = ".ccs";

        /// <summary>
        /// Gets the save extension compressed.
        /// </summary>
        /// <value>The save extension compressed.</value>
        public string SaveExtensionCompressed { get; private set; } = ".ccx";
        /// <summary>
        /// Gets the string replacement pattern.
        /// </summary>
        /// <value>The string replacement pattern.</value>
        public string StringReplacementPattern { get; private set; }

        /// <summary>
        /// Gets the string replacements.
        /// </summary>
        /// <value>The string replacements.</value>
        public Dictionary<string, string> StringReplacements
        {
            get { return stringReplacements; }
        }

        /// <summary>
        /// Gets the splitters column.
        /// </summary>
        /// <value>The splitters column.</value>
        public char SplittersColumn { get; private set; } = '\t';

        /// <summary>
        /// Gets the string splitters.
        /// </summary>
        /// <value>The string splitters.</value>
        public string[] SplittersRow { get; private set; } = { "\r\n", "\n", "\r" };

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Initializes the XML replacements.
        /// </summary>
        ///  Changelog:
        ///             - 2.0.0 (05-31-2017) - Initial version. Originally directly part of the constructor.
        private void InitializeXmlReplacements()
        {
            var replacements = new Dictionary<char, string>()
            {
                {'!', "&#33;" },
                {'"', "&#34;" },
                {'#', "&#35;" },
                {'$', "&#36;" },
                {'%', "&#37;" },
                {'&', "&#38;" },
                {'\'', "&#39;" },
                {'(', "&#40;" },
                {')', "&#41;" },
                {'*', "&#42;" },
                {'+', "&#43;" },
                {',', "&#44;" },
                {'-', "&#45;" },
                {'.', "&#46;" },
                {'/', "&#47;" },
                {':', "&#58;" },
                {';', "&#59;" },
                {'<', "&#60;" },
                {'=', "&#61;" },
                {'>', "&#62;" },
                {'?', "&#63;" },
                {'@', "&#64;" },
                {'[', "&#91;" },
                {'\\', "&#92;" },
                {']', "&#93;" },
                {'^', "&#94;" },
                {'_', "&#95;" },
                {'`', "&#96;" },
                {'{', "&#123;" },
                {'|', "&#124;" },
                {'}', "&#125;" },
                {'~', "&#126;" },
            };

            characterReplacements = new Dictionary<char, string>();
            stringReplacements = new Dictionary<string, string>();
            StringReplacementPattern = string.Empty;
            var pattern = new StringBuilder();
            pattern.Append("\b(");
            var i = 0;
            var max = replacements.Count - 1;
            foreach (var pair in replacements)
            {
                characterReplacements.Add(pair.Key, pair.Value);
                stringReplacements.Add(pair.Value, pair.Key.ToString());

                pattern.Append(string.Format("{0}{1}", pair.Value, i == max ? "" : "|"));
                i++;
            }
            pattern.Append(")\b");
            StringReplacementPattern = pattern.ToString();
        }

        #endregion Private Methods
    }
}