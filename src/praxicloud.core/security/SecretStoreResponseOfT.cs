// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.security
{
    /// <summary>
    /// Stores a response from a secret store
    /// </summary>
    public class SecretStoreResponse<T> : SecretStoreResponse where T : class
    {
        #region Properties
        /// <summary>
        /// The value of the response
        /// </summary>
        public T Value { get; set; } = default;
        #endregion
        #region Methods
        /// <summary>
        /// Implicit conversion to the type where the value if successfully executed is returned
        /// </summary>
        /// <param name="response">The secrete store response to cast</param>
        public static implicit operator T(SecretStoreResponse<T> response)
        {
            return response.Value;
        }
        #endregion
    }
}

