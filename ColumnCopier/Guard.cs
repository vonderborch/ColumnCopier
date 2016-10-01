// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Guard.cs
// Author           : vonderborch
// Created          : 09-30-2016
// 
// Version          : 1.2.0
// Last Modified By : vonderborch
// Last Modified On : 09-30-2016
// ***********************************************************************
// <copyright file="Guard.cs">
//		Copyright ©  2016
// </copyright>
// <summary>
//      Defines the Guard class.
// </summary>
//
// Changelog: 
//            - 1.2.0 (09-30-2016) - Initial version created.
// ***********************************************************************
using System.Threading;

namespace ColumnCopier
{
    /// <summary>
    /// A thread-safe boolean guard/flag.
    /// </summary>
    public class Guard
    {
        #region Private Fields

        /// <summary>
        /// The value for false
        /// </summary>
        private const int FALSE = 0;
        /// <summary>
        /// The value for true
        /// </summary>
        private const int TRUE = 1;

        /// <summary>
        /// The current state
        /// </summary>
        private int state = FALSE;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether [check set].
        /// </summary>
        /// <value><c>true</c> if [check set]; otherwise, <c>false</c>.</value>
        public bool CheckSet
        {
            get { return Interlocked.Exchange(ref state, TRUE) == FALSE; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Resets this instance.
        /// </summary>
        ///  Changelog:
        ///             - 1.2.0 (09-30-2016) - Initial version.
        public void Reset()
        {
            Interlocked.Exchange(ref state, FALSE);
        }

        #endregion Public Methods
    }
}