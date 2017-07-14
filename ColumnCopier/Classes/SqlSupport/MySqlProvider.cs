﻿// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : MySqlProvider.cs
// Author           : Christian
// Created          : 07-14-2017
// 
// Version          : 2.2.0
// Last Modified By : Christian
// Last Modified On : 07-14-2017
// ***********************************************************************
// <copyright file="MySqlProvider.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      MySql connection provider.
// </summary>
//
// Changelog: 
//            - 2.2.0 (07-14-2017) - Initial version created.
// ***********************************************************************
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;

namespace ColumnCopier.Classes.SqlSupport
{
    /// <summary>
    /// Class MySqlProvider.
    /// </summary>
    /// <seealso cref="ColumnCopier.Classes.SqlSupport.ASqlProvider" />
    public class MySqlProvider : ASqlProvider
    {
        #region Private Fields

        /// <summary>
        /// The connection
        /// </summary>
        private MySqlConnection conn;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Determines whether this instance [can connect to server].
        /// </summary>
        /// <returns><c>true</c> if this instance [can connect to server]; otherwise, <c>false</c>.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public override bool CanConnectToServer()
        {
            try
            {
                Connect();
            }
            catch
            {
                return false;
            }

            switch (conn.State)
            {
                case System.Data.ConnectionState.Open:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public override void Dispose()
        {
            conn.Close();
            conn.Dispose();
        }

        /// <summary>
        /// Executes the SQL query.
        /// </summary>
        /// <returns>System.String.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public override string ExecuteSqlQuery()
        {
            try
            {
                Connect();

                var cmd = new MySqlCommand(this.SqlQuery, conn);
                var reader = cmd.ExecuteReader();

                var result = new StringBuilder();
                var columns = new List<string>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    columns.Add(reader.GetName(i));
                    result.Append($"{columns[columns.Count - 1]}{Constants.Instance.CharTab}");
                }
                result.Append(Constants.Instance.CharNewLine);

                while (reader.Read())
                {
                    for (var i = 0; i < columns.Count; i++)
                        result.Append($"{reader[columns[i]].ToString()}{Constants.Instance.CharTab}");

                    result.Append(Constants.Instance.CharNewLine);
                }

                reader.Close();

                return result.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// SQLs the select query is valid.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        public override bool SqlSelectQueryIsValid()
        {
            try
            {
                Connect();
                var cmd = new MySqlCommand(this.SqlQuery, conn);
                cmd.Prepare();

                return cmd.IsPrepared;
            }
            catch
            {
                return false;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Connects this instance.
        /// </summary>
        ///  Changelog:
        ///             - 2.2.0 (07-14-2017) - Initial version.
        private void Connect()
        {
            if (conn == null)
            {
                conn = new MySqlConnection(this.SqlConnectionString);

                conn.Open();
            }
            else
            {
                switch (conn.State)
                {
                    case System.Data.ConnectionState.Open:
                    case System.Data.ConnectionState.Connecting:
                        break;

                    default:
                        conn.Open();
                        break;
                }
            }
        }

        #endregion Private Methods
    }
}