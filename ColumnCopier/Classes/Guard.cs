// ***********************************************************************
// Assembly         : ColumnCopier
// Component        : Guard.cs
// Author           : Christian
// Created          : 09-30-2016
// 
// Version          : 2.0.0
// Last Modified By : Christian
// Last Modified On : 06-05-2017
// ***********************************************************************
// <copyright file="Guard.cs" company="Christian Webber">
//		Copyright ©  2016 - 2017
// </copyright>
// <summary>
//      Defines the Guard class.
// </summary>
//
// Changelog: 
//            - 2.0.0 (06-05-2017) - Added Check method (which won't set the toggle).
//            - 2.0.0 (06-01-2017) - Reorganized.
//            - 1.2.0 (09-30-2016) - Initial version created.
// ***********************************************************************
using System.Threading;

namespace ColumnCopier.Classes
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
        /// Gets a value indicating whether this <see cref="Guard"/> is check.
        /// </summary>
        /// <value><c>true</c> if check; otherwise, <c>false</c>.</value>
        public bool Check
        {
            get { return state == TRUE; }
        }

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