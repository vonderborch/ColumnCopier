// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : GitHub.cs
// Author           : Christian
// Created          : 05-30-2017
//
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="GitHub.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The GitHub class.
// </summary>
//
// Changelog:
//            - 2.0.0 (06-06-2017) - Added Github Status Check
//            - 2.0.0 (05-31-2017) - Moved public fields to Constants.cs
//            - 1.3.0 (05-30-2017) - Initial code for detecting the latest release of the app.
// ***********************************************************************

using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;

/// <summary>
/// The GitHub namespace.
/// </summary>
namespace ColumnCopier.GitHub
{
    /// <summary>
    /// Class GitHub.
    /// </summary>
    public class GitHub
    {
        #region Public Methods

        /// <summary>
        /// Gets the latest release.
        /// </summary>
        /// <returns>Release.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Moved public fields to Constants.cs
        ///             - 2.0.0 (05-31-2017) - Moved public fields to Constants.cs
        ///             - 1.3.0 (05-30-2017) - Initial version.
        public static Release GetLatestRelease()
        {
            return CheckGithubStatus()
                ? GetGithubLatestRelease()
                : new Release() { html_url = string.Empty, tag_name = string.Empty, Status = Constants.Instance.GitHubStatusDown };
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Checks the github status.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Initial version
        private static bool CheckGithubStatus()
        {
            var uri = Constants.Instance.GitHubStatusUrl;
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; rv:15.0) Gecko/20120716 Firefox/15.0a2");

            Stream data = client.OpenRead(uri);
            StreamReader reader = new StreamReader(data);

            if (reader == null) return false;

            var deserializer = new DataContractJsonSerializer(typeof(Status));
            var status = (Status)deserializer.ReadObject(reader.BaseStream);

            return status.status != "major"
                ? true
                : false;
        }

        /// <summary>
        /// Gets the github latest release.
        /// </summary>
        /// <returns>Release.</returns>
        ///  Changelog:
        ///             - 2.0.0 (06-06-2017) - Revised to use a deserializer
        ///             - 2.0.0 (06-06-2017) - Initial version
        private static Release GetGithubLatestRelease()
        {
            try
            {
                var uri = $"{Constants.Instance.GitHubApiUrl}repos/{Constants.Instance.GitHubRepoOwner}/{Constants.Instance.GitHubRepository}/releases/latest";
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; rv:15.0) Gecko/20120716 Firefox/15.0a2");

                Stream data = client.OpenRead(uri);
                StreamReader reader = new StreamReader(data);
                if (reader == null) throw new Exception("No stream!");

                var deserializer = new DataContractJsonSerializer(typeof(Release));
                var release = (Release)deserializer.ReadObject(reader.BaseStream);

                if (release == null) new Release() { Status = Constants.Instance.GitHubStatusReleaseUnavailable };

                release.Status = Constants.Instance.GitHubStatusGood;
                return release;
            }
            catch (Exception ex)
            {
                //add_error("Can't read html page " + url + " : " + e.Message);
                return new Release() { html_url = string.Empty, tag_name = string.Empty, Status = Constants.Instance.GitHubStatusReleaseUnavailable };
            }
        }

        #endregion Private Methods
    }
}