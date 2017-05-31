// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : GitHub.cs
// Author           : Christian
// Created          : 05-30-2017
//
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 05-31-2017
// ***********************************************************************
// <copyright file="GitHub.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The GitHub class.
// </summary>
//
// Changelog:
//            - 2.0.0 (05-31-2017) - Moved public fields to Constants.cs
//            - 1.3.0 (05-30-2017) - Initial code for detecting the latest release of the app.
// ***********************************************************************

using System;
using System.IO;
using System.Net;

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
        ///             - 2.0.0 (05-31-2017) - Moved public fields to Constants.cs
        ///             - 1.3.0 (05-30-2017) - Initial version.
        public static Release GetLatestRelease()
        {
            try
            {
                var uri = $"{Constants.Instance.GitHubApiUrl}repos/{Constants.Instance.GitHubRepoOwner}/{Constants.Instance.GitHubRepository}/releases/latest";
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