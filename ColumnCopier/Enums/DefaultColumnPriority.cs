// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : DefaultColumnPriority.cs
// Author           : Christian
// Created          : 06-06-2017
// 
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="DefaultColumnPriority.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the DefaultColumnPriority enum.
// </summary>
//
// Changelog: 
//            - 2.0.0 (06-06-2017) - Initial version created.
// ***********************************************************************
namespace ColumnCopier.Enums
{
    public enum DefaultColumnPriority
    {
        /// <summary>
        /// Column name should have priority
        /// </summary>
        Name,
        /// <summary>
        /// Column number should have priority
        /// </summary>
        Number
    }
}