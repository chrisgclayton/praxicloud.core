// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.security
{
    #region Using Clauses
    using System;
    #endregion

    /// <summary>
    /// Stores a response from a secret store
    /// </summary>
    public class SecretStoreResponse
    {
        #region Properties
        /// <summary>
        /// The status code that was returned
        /// </summary>
        public int StatusCode { get; set; } = 503;

        /// <summary>
        /// True if the status code indicates a success
        /// </summary>
        public bool IsSuccessCode { get; set; } = false;

        /// <summary>
        /// The time it took for the operation to execute
        /// </summary>
        public TimeSpan? TimeToExecute { get; set; }

        /// <summary>
        /// If an exception was raised it is populate in it
        /// </summary>
        public Exception Exception { get; set; }
        #endregion
        #region Methods
        /// <summary>
        /// Implicit conversion to boolean where true indicates successful execution
        /// </summary>
        /// <param name="response">The secrete store response to cast</param>
        public static implicit operator bool(SecretStoreResponse response)
        {
            return response.IsSuccessCode;
        }
        #endregion
    }
}
