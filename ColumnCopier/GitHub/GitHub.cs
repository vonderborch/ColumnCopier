// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Main.cs
// Author           : Christian
// Created          : 05-30-2017
//
// Version          : 1.3.0
// Last Modified By : Christian
// Last Modified On : 05-30-2017
// ***********************************************************************
// <copyright file="Main.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Main class.
// </summary>
//
// Changelog:
//            - 1.3.0 (05-30-2017) - Initial code for detecting the latest release of the app.
// ***********************************************************************

using System;
using System.IO;
using System.Net;

namespace ColumnCopier.GitHub
{
    public class GitHub
    {
        #region Public Fields

        /// <summary>
        /// The git hub API URL
        /// </summary>
        public static readonly Uri GitHubApiUrl = new Uri("https://api.github.com/");
        /// <summary>
        /// The git hub dot COM URL
        /// </summary>
        public static readonly Uri GitHubDotComUrl = new Uri("https://github.com/");
        /// <summary>
        /// The repo owner
        /// </summary>
        public static readonly string RepoOwner = "vonderborch";
        /// <summary>
        /// The name of the repo
        /// </summary>
        public static readonly string RepoRepoName = "ColumnCopier";

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        /// Gets the latest release.
        /// </summary>
        /// <returns>Release.</returns>
        ///  Changelog:
        ///             - 1.3.0 (05-30-2017) - Initial version.
        public static Release GetLatestRelease()
        {
            try
            {
                var uri = $"{GitHubApiUrl}repos/{RepoOwner}/{RepoRepoName}/releases/latest";
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; rv:15.0) Gecko/20120716 Firefox/15.0a2");

                Stream data = client.OpenRead(uri);
                StreamReader reader = new StreamReader(data);

                var release = new Release();
                var line = reader.ReadLine();
                do
                {
                    var splitLine = line.ToString().Split(':');

                    if (splitLine.Length > 1)
                    {
                        var key = splitLine[0];
                        switch (key)
                        {
                            case "  \"html_url\"":
                                release.html_url = $"{splitLine[1].Replace("\"", "")}:{splitLine[2].Replace("\"", "")}";
                                break;

                            case "  \"tag_name\"":
                                release.tag_name = splitLine[1].Replace("\"", "");
                                break;
                        }

                        if (!string.IsNullOrEmpty(release.tag_name) && !string.IsNullOrEmpty(release.html_url))
                            break;
                    }
                    line = reader.ReadLine();
                } while (line != null);

                release.html_url = release.html_url.Remove(release.html_url.Length - 1);
                release.tag_name = release.tag_name.Remove(release.tag_name.Length - 1);
                return release;
            }
            catch (Exception ex)
            {
                //add_error("Can't read html page " + url + " : " + e.Message);
                return null;
            }
        }

        #endregion Public Methods
    }
}