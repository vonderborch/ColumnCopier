// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Constants.cs
// Author           : Christian
// Created          : 05-30-2017
//
// Version          : 2.2.2
// Last Modified By : Christian
// Last Modified On : 05-17-2018
// ***********************************************************************
// <copyright file="Constants.cs" company="Christian Webber">
//		Copyright ©  2016 - 2018
// </copyright>
// <summary>
//      The Constants class.
// </summary>
//
// Changelog:
//            - 2.2.2 (05-17-2018) - Version number bump.
//            - 2.2.1 (08-07-2017) - Version number bump.
//            - 2.2.0 (07-14-2017) - Multiple column copying support.
//            - 2.2.0 (07-13-2017) - Sql input support.
//            - 2.1.0 (06-07-2017) - Exception message constants and bumped save version and program version. Also added a minimum save version constant to indicate the minimum readable save version of this program.
//            - 2.0.0 (06-06-2017) - New constants + comments.
//            - 2.0.0 (05-31-2017) - Moved more constants over here.
//            - 1.3.0 (05-30-2017) - Initial code.
// ***********************************************************************
using ColumnCopier.Classes.SqlSupport;
using ColumnCopier.Enums;
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
        public static int ProgramVersion { get; private set; } = 221;

        /// <summary>
        /// Gets the save version.
        /// </summary>
        /// <value>The save version.</value>
        public static int SaveVersion { get; private set; } = 22;

        /// <summary>
        /// Gets the minimum readable save version.
        /// </summary>
        /// <value>The minimum readable save version.</value>
        public static int SaveVersionMinimum { get; private set; } = 21;

        /// <summary>
        /// Gets the character replacements.
        /// </summary>
        /// <value>The character replacements.</value>
        public Dictionary<char, string> CharacterReplacements
        {
            get { return characterReplacements; }
        }

        /// <summary>
        /// Gets the character new line.
        /// </summary>
        /// <value>The character new line.</value>
        public string CharNewLine { get; private set; } = Environment.NewLine;

        /// <summary>
        /// Gets the character tab.
        /// </summary>
        /// <value>The character tab.</value>
        public string CharTab { get; private set; } = "\t";

        /// <summary>
        /// Gets the column line seperators.
        /// </summary>
        /// <value>The column line seperators.</value>
        public Dictionary<ColumnLineSeparatorOptions, Tuple<string, int>> ColumnLineSeperators { get; private set; } = new Dictionary<ColumnLineSeparatorOptions, Tuple<string, int>>()
        {
            { ColumnLineSeparatorOptions.Nothing, new Tuple<string, int>("", 1) },
            { ColumnLineSeparatorOptions.NewLine, new Tuple<string, int>("\\n", 0) },
        };

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
        /// Gets the format stat current column.
        /// </summary>
        /// <value>The format stat current column.</value>
        public string FormatStatCurrentColumn { get; private set; } = "Current Column #: {0} ({1})";

        /// <summary>
        /// Gets the format stat number columns.
        /// </summary>
        /// <value>The format stat number columns.</value>
        public string FormatStatNumberColumns { get; private set; } = "# of Columns: {0}";

        /// <summary>
        /// Gets the format stat number rows.
        /// </summary>
        /// <value>The format stat number rows.</value>
        public string FormatStatNumberRows { get; private set; } = "# of Rows: {0}";

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
        /// Gets the git hub status down.
        /// </summary>
        /// <value>The git hub status down.</value>
        public string GitHubStatusDown { get; private set; } = "GithubDown";

        /// <summary>
        /// Gets the git hub status good.
        /// </summary>
        /// <value>The git hub status good.</value>
        public string GitHubStatusGood { get; private set; } = "ReleaseFound";

        /// <summary>
        /// Gets the git hub status release unavailable.
        /// </summary>
        /// <value>The git hub status release unavailable.</value>
        public string GitHubStatusReleaseUnavailable { get; private set; } = "UnableToFindRelease";

        /// <summary>
        /// Gets the git hub status URL.
        /// </summary>
        /// <value>The git hub status URL.</value>
        public string GitHubStatusUrl { get; private set; } = "https://status.github.com/api/status.json";

        /// <summary>
        /// Gets the input query change history request.
        /// </summary>
        /// <value>The input query change history request.</value>
        public string InputQueryChangeHistoryRequest { get; private set; } = "Please select a request to change to:";

        /// <summary>
        /// Gets the input query default column name.
        /// </summary>
        /// <value>The input query default column name.</value>
        public string InputQueryDefaultColumnName { get; private set; } = "Default column name?";

        /// <summary>
        /// Gets the input query default column number.
        /// </summary>
        /// <value>The input query default column number.</value>
        public string InputQueryDefaultColumnNumber { get; private set; } = "Default column number?";

        /// <summary>
        /// Gets the input query for line replacement post-text.
        /// </summary>
        /// <value>The input query for line replacement post-text.</value>
        public string InputQueryLineReplacementPost { get; private set; } = "What should the post-line text be?";

        /// <summary>
        /// Gets the input query for line replacement pre-text.
        /// </summary>
        /// <value>The input query for line replacement pre-text.</value>
        public string InputQueryLineReplacementPre { get; private set; } = "What should the pre-line text be?";

        /// <summary>
        /// Gets the input query for line separators.
        /// </summary>
        /// <value>The input query for line separators.</value>
        public string InputQueryLineReplacementSeparator { get; private set; } = "What should replace line seperators?";

        /// <summary>
        /// Gets the input query max history.
        /// </summary>
        /// <value>The input query max history.</value>
        public string InputQueryMaxHistory { get; private set; } = "How many requests should be saved?";

        /// <summary>
        /// Gets the input query name similarity value.
        /// </summary>
        /// <value>The input query name similarity value.</value>
        public string InputQueryNameSimilarityValue { get; private set; } = "Default column name similarity threshold?";

        /// <summary>
        /// Gets the input query next line copy line.
        /// </summary>
        /// <value>The input query next line copy line.</value>
        public string InputQueryNextLineCopyLine { get; private set; } = "What should the next line to copy be?";

        /// <summary>
        /// Gets the input query program opacity.
        /// </summary>
        /// <value>The input query program opacity.</value>
        public string InputQueryProgramOpacity { get; private set; } = "Program opacity?";

        /// <summary>
        /// Gets the input query SQL connection string.
        /// </summary>
        /// <value>The input query SQL connection string.</value>
        public string InputQuerySqlConnectionString { get; private set; } = "SQL Connection String?";

        /// <summary>
        /// Gets the input query SQL select query.
        /// </summary>
        /// <value>The input query SQL select query.</value>
        public string InputQuerySqlSelectQuery { get; private set; } = "SQL Select Query?";

        /// <summary>
        /// Gets the line seperators.
        /// </summary>
        /// <value>The line seperators.</value>
        public Dictionary<LineSeparatorOptions, Tuple<string, string, string, int>> LineSeperators { get; private set; } = new Dictionary<LineSeparatorOptions, Tuple<string, string, string, int>>()
        {
            { LineSeparatorOptions.Nothing, new Tuple<string, string, string, int>("", "", "", 1) },
            { LineSeparatorOptions.Comma, new Tuple<string, string, string, int>(", ", "", "", 0) },
            { LineSeparatorOptions.DoubleQuoteComma, new Tuple<string, string, string, int>("\", \"", "", "", 2) },
            { LineSeparatorOptions.DoubleQuoteParenthesisComma, new Tuple<string, string, string, int>("\", \"", "(\"", "\")", 5) },
            { LineSeparatorOptions.ParenthesisComma, new Tuple<string, string, string, int>(", ", "(", ")", 3) },
            { LineSeparatorOptions.SemiColon, new Tuple<string, string, string, int>("; ", "", "", 6) },
            { LineSeparatorOptions.SingleQuoteParenthesisComma, new Tuple<string, string, string, int>("', '", "('", "')", 4) },
        };

        /// <summary>
        /// Gets the message body exception.
        /// </summary>
        /// <value>The message body exception.</value>
        public string MessageBodyException { get; private set; } = "Do you want to file an issue report for this exception ({0})?";

        /// <summary>
        /// Gets the message title latest release unavailable.
        /// </summary>
        /// <value>The message title latest release unavailable.</value>
        public string MessageBodyGitHubDown { get; private set; } = "GitHub is down, please try again later.";

        /// <summary>
        /// Gets the message body latest release unavailable.
        /// </summary>
        /// <value>The message body latest release unavailable.</value>
        public string MessageBodyLatestReleaseUnavailable { get; private set; } = "Unable to discover the latest release!";

        /// <summary>
        /// Gets the message body new release available.
        /// </summary>
        /// <value>The message body new release available.</value>
        public string MessageBodyNewReleaseAvailable { get; private set; } = "A newly released version is available, version {0}. Would you like to download the update?";

        /// <summary>
        /// Gets the message body no new release.
        /// </summary>
        /// <value>The message body no new release.</value>
        public string MessageBodyNoNewRelease { get; private set; } = "You already have the latest version of this program!";

        /// <summary>
        /// Gets the message body no request history.
        /// </summary>
        /// <value>The message body no request history.</value>
        public string MessageBodyNoRequestHistory { get; private set; } = "There are currently no requests available.";

        /// <summary>
        /// Gets the message body no SQL provider.
        /// </summary>
        /// <value>The message body no SQL provider.</value>
        public string MessageBodyNoSqlProvider { get; private set; } = "The selected provider, {0}, is unavailable (DLL(s) not found). {2}{2} Mssing DLL's: {1}.";

        /// <summary>
        /// Gets the message title exception.
        /// </summary>
        /// <value>The message title exception.</value>
        public string MessageTitleException { get; private set; } = "An Exception Has Occured!";

        /// <summary>
        /// Gets the message title latest release unavailable.
        /// </summary>
        /// <value>The message title latest release unavailable.</value>
        public string MessageTitleGitHubDown { get; private set; } = "GitHub Unavailable";

        /// <summary>
        /// Gets the message title latest release unavailable.
        /// </summary>
        /// <value>The message title latest release unavailable.</value>
        public string MessageTitleLatestReleaseUnavailable { get; private set; } = "Unable to Find Latest Release";

        /// <summary>
        /// Gets the message title new release available.
        /// </summary>
        /// <value>The message title new release available.</value>
        public string MessageTitleNewReleaseAvailable { get; private set; } = "Update Available";

        /// <summary>
        /// Gets the message title no new release.
        /// </summary>
        /// <value>The message title no new release.</value>
        public string MessageTitleNoNewRelease { get; private set; } = "No Updates";

        /// <summary>
        /// Gets the message title no request history.
        /// </summary>
        /// <value>The message title no request history.</value>
        public string MessageTitleNoRequestHistory { get; private set; } = "No Request History";

        /// <summary>
        /// Gets the message title no SQL provider.
        /// </summary>
        /// <value>The message title no SQL provider.</value>
        public string MessageTitleNoSqlProvider { get; private set; } = "SQL Provider Unavailable";

        /// <summary>
        /// Gets the needed files my SQL connection.
        /// </summary>
        /// <value>The needed files my SQL connection.</value>
        public List<string> NeededFilesMySqlConnection { get; private set; } = new List<string>() { "MySql.Data.dll" };

        /// <summary>
        /// Gets the needed files none connection.
        /// </summary>
        /// <value>The needed files none connection.</value>
        public List<string> NeededFilesNoneConnection { get; private set; } = new List<string>() { };

        /// <summary>
        /// Gets the needed files postgre SQL connection.
        /// </summary>
        /// <value>The needed files postgre SQL connection.</value>
        public List<string> NeededFilesPostgreSqlConnection { get; private set; } = new List<string>() { "Npgsql.dll", "System.Threading.Tasks.Extensions.dll" };

        /// <summary>
        /// Gets the needed files SQL server connection.
        /// </summary>
        /// <value>The needed files SQL server connection.</value>
        public List<string> NeededFilesSqlServerConnection { get; private set; } = new List<string>() { };

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
        /// Gets the splitters column.
        /// </summary>
        /// <value>The splitters column.</value>
        public char SplittersColumn { get; private set; } = '\t';

        /// <summary>
        /// Gets the string splitters.
        /// </summary>
        /// <value>The string splitters.</value>
        public string[] SplittersRow { get; private set; } = { "\r\n", "\n", "\r" };

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
        /// Gets the URL help.
        /// </summary>
        /// <value>The URL help.</value>
        public string UrlHelp { get; private set; } = "https://github.com/vonderborch/ColumnCopier/wiki";

        /// <summary>
        /// Gets the URL support.
        /// </summary>
        /// <value>The URL support.</value>
        public string UrlSupport { get; private set; } = "https://github.com/vonderborch/ColumnCopier/issues";

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