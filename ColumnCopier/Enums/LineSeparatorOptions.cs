// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : LineSeparatorOptions.cs
// Author           : Christian
// Created          : 06-06-2017
// 
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-06-2017
// ***********************************************************************
// <copyright file="LineSeparatorOptions.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the LineSeparatorOptions enum.
// </summary>
//
// Changelog: 
//            - 2.0.0 (06-06-2017) - Initial version created.
// ***********************************************************************
namespace ColumnCopier.Enums
{
    public enum LineSeparatorOptions
    {
        /// <summary>
        /// ,
        /// </summary>
        Comma = 0,

        /// <summary>
        /// 
        /// </summary>
        Nothing = 1,

        /// <summary>
        /// ", "
        /// </summary>
        DoubleQuoteComma = 2,

        /// <summary>
        /// ( , )
        /// </summary>
        ParenthesisComma = 3,

        /// <summary>
        /// (' ', ' ')
        /// </summary>
        SingleQuoteParenthesisComma = 4,

        /// <summary>
        /// (" ", " ")
        /// </summary>
        DoubleQuoteParenthesisComma = 5,

        /// <summary>
        /// ;
        /// </summary>
        SemiColon = 6,
    }
}