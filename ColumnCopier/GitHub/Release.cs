// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Release.cs
// Author           : Christian
// Created          : 05-30-2017
//
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="Release.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      The Release class.
// </summary>
//
// Changelog:
//            - 2.0.0 (06-06-2017) - Expanded to contain all information on the latest release.
//            - 2.0.0 (06-06-2017) - Added status field
//            - 1.3.0 (05-30-2017) - Initial code for detecting the latest release of the app.
// ***********************************************************************
using System.Collections.Generic;

namespace ColumnCopier.GitHub
{
    /// <summary>
    /// Class Asset.
    /// </summary>
    public class Asset
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the browser download URL.
        /// </summary>
        /// <value>The browser download URL.</value>
        public string browser_download_url { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string content_type { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>The created at.</value>
        public string created_at { get; set; }

        /// <summary>
        /// Gets or sets the download count.
        /// </summary>
        /// <value>The download count.</value>
        public int download_count { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public object label { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int size { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public string state { get; set; }

        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>The updated at.</value>
        public string updated_at { get; set; }

        /// <summary>
        /// Gets or sets the uploader.
        /// </summary>
        /// <value>The uploader.</value>
        public Uploader uploader { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Class Author.
    /// </summary>
    public class Author
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        /// <value>The avatar URL.</value>
        public string avatar_url { get; set; }

        /// <summary>
        /// Gets or sets the events URL.
        /// </summary>
        /// <value>The events URL.</value>
        public string events_url { get; set; }

        /// <summary>
        /// Gets or sets the followers URL.
        /// </summary>
        /// <value>The followers URL.</value>
        public string followers_url { get; set; }

        /// <summary>
        /// Gets or sets the following URL.
        /// </summary>
        /// <value>The following URL.</value>
        public string following_url { get; set; }

        /// <summary>
        /// Gets or sets the gists URL.
        /// </summary>
        /// <value>The gists URL.</value>
        public string gists_url { get; set; }

        /// <summary>
        /// Gets or sets the gravatar identifier.
        /// </summary>
        /// <value>The gravatar identifier.</value>
        public string gravatar_id { get; set; }

        /// <summary>
        /// Gets or sets the HTML URL.
        /// </summary>
        /// <value>The HTML URL.</value>
        public string html_url { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        /// <value>The login.</value>
        public string login { get; set; }

        /// <summary>
        /// Gets or sets the organizations URL.
        /// </summary>
        /// <value>The organizations URL.</value>
        public string organizations_url { get; set; }

        /// <summary>
        /// Gets or sets the received events URL.
        /// </summary>
        /// <value>The received events URL.</value>
        public string received_events_url { get; set; }

        /// <summary>
        /// Gets or sets the repos URL.
        /// </summary>
        /// <value>The repos URL.</value>
        public string repos_url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [site admin].
        /// </summary>
        /// <value><c>true</c> if [site admin]; otherwise, <c>false</c>.</value>
        public bool site_admin { get; set; }

        /// <summary>
        /// Gets or sets the starred URL.
        /// </summary>
        /// <value>The starred URL.</value>
        public string starred_url { get; set; }

        /// <summary>
        /// Gets or sets the subscriptions URL.
        /// </summary>
        /// <value>The subscriptions URL.</value>
        public string subscriptions_url { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Class NewRelease.
    /// </summary>
    public class Release
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the assets.
        /// </summary>
        /// <value>The assets.</value>
        public List<Asset> assets { get; set; }

        /// <summary>
        /// Gets or sets the assets URL.
        /// </summary>
        /// <value>The assets URL.</value>
        public string assets_url { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        public Author author { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        public string body { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>The created at.</value>
        public string created_at { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NewRelease"/> is draft.
        /// </summary>
        /// <value><c>true</c> if draft; otherwise, <c>false</c>.</value>
        public bool draft { get; set; }

        /// <summary>
        /// Gets or sets the HTML URL.
        /// </summary>
        /// <value>The HTML URL.</value>
        public string html_url { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NewRelease"/> is prerelease.
        /// </summary>
        /// <value><c>true</c> if prerelease; otherwise, <c>false</c>.</value>
        public bool prerelease { get; set; }

        /// <summary>
        /// Gets or sets the published at.
        /// </summary>
        /// <value>The published at.</value>
        public string published_at { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        /// <value>The name of the tag.</value>
        public string tag_name { get; set; }

        /// <summary>
        /// Gets or sets the tarball URL.
        /// </summary>
        /// <value>The tarball URL.</value>
        public string tarball_url { get; set; }

        /// <summary>
        /// Gets or sets the target commitish.
        /// </summary>
        /// <value>The target commitish.</value>
        public string target_commitish { get; set; }

        /// <summary>
        /// Gets or sets the upload URL.
        /// </summary>
        /// <value>The upload URL.</value>
        public string upload_url { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the zipball URL.
        /// </summary>
        /// <value>The zipball URL.</value>
        public string zipball_url { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Class Uploader.
    /// </summary>
    public class Uploader
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        /// <value>The avatar URL.</value>
        public string avatar_url { get; set; }

        /// <summary>
        /// Gets or sets the events URL.
        /// </summary>
        /// <value>The events URL.</value>
        public string events_url { get; set; }

        /// <summary>
        /// Gets or sets the followers URL.
        /// </summary>
        /// <value>The followers URL.</value>
        public string followers_url { get; set; }

        /// <summary>
        /// Gets or sets the following URL.
        /// </summary>
        /// <value>The following URL.</value>
        public string following_url { get; set; }

        /// <summary>
        /// Gets or sets the gists URL.
        /// </summary>
        /// <value>The gists URL.</value>
        public string gists_url { get; set; }

        /// <summary>
        /// Gets or sets the gravatar identifier.
        /// </summary>
        /// <value>The gravatar identifier.</value>
        public string gravatar_id { get; set; }

        /// <summary>
        /// Gets or sets the HTML URL.
        /// </summary>
        /// <value>The HTML URL.</value>
        public string html_url { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        /// <value>The login.</value>
        public string login { get; set; }

        /// <summary>
        /// Gets or sets the organizations URL.
        /// </summary>
        /// <value>The organizations URL.</value>
        public string organizations_url { get; set; }

        /// <summary>
        /// Gets or sets the received events URL.
        /// </summary>
        /// <value>The received events URL.</value>
        public string received_events_url { get; set; }

        /// <summary>
        /// Gets or sets the repos URL.
        /// </summary>
        /// <value>The repos URL.</value>
        public string repos_url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [site admin].
        /// </summary>
        /// <value><c>true</c> if [site admin]; otherwise, <c>false</c>.</value>
        public bool site_admin { get; set; }

        /// <summary>
        /// Gets or sets the starred URL.
        /// </summary>
        /// <value>The starred URL.</value>
        public string starred_url { get; set; }

        /// <summary>
        /// Gets or sets the subscriptions URL.
        /// </summary>
        /// <value>The subscriptions URL.</value>
        public string subscriptions_url { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string url { get; set; }

        #endregion Public Properties
    }
}