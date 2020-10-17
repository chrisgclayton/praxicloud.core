// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.security
{
    #region Using Clauses
    using System;
    #endregion

    /// <summary>
    /// An exception derived from ArgumentException to allow handlers to take special actions in handling
    /// </summary>
    public class GuardException : ArgumentException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GuardException class with a specified error message and the parameter name that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="paramName">The name of the parameter that caused the current exception. </param>
        public GuardException(string message, string paramName) : base(message, paramName)
        {
            if (string.IsNullOrEmpty(message)) throw new ArgumentException("Must not be null or empty", "message");
        }
        #endregion
    }
}