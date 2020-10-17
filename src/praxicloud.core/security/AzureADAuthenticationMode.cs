// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.security
{
    /// <summary>
    /// The authentication mode
    /// </summary>
    public enum AzureADAuthenticationMode : byte
    {
        /// <summary>
        /// Managed Service Identity
        /// </summary>
        ManagedServiceIdentity = 0,

        /// <summary>
        /// Client Secret
        /// </summary>
        ClientSecret = 1
    }
}