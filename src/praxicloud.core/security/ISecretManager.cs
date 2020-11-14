// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.security
{
    #region Using Clauses
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// A secret store
    /// </summary>
    public interface ISecretManager
    {
        #region Methods
        /// <summary>
        /// Retrieves a secret from the secret store
        /// </summary>
        /// <param name="secretName">The name of the secret to retrieve</param>
        /// <param name="version">The version of the secret or null for latest</param>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        /// <returns>The value is the secret that was retrieved or null if the operation was not successful, but returned an exception</returns>
        Task<SecretStoreResponse<string>> GetSecretAsync(string secretName, string version = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an object based on the secret content
        /// </summary>
        /// <typeparam name="T">The type of object to return the secret content as</typeparam>
        /// <param name="secretName">The name of the secret to retrieve</param>
        /// <param name="version">The version of the secret or null for latest</param>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        /// <returns>The value is the object returned from the store in the representation of the generic</returns>
        Task<SecretStoreResponse<T>> GetSecretAsync<T>(string secretName, string version = null, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Retrieves the latest version of the certificate from the key vault with the associated policies
        /// </summary>
        /// <param name="certificateName">The name of the certificate</param>
        /// <param name="version">The version of the certificate or null for latest</param>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        /// <returns>The value is the certificate or null if the operation was not successful, but returned an exception</returns>
        Task<SecretStoreResponse<X509Certificate2>> GetCertificateAsync(string certificateName, string version = null, CancellationToken cancellationToken = default);
        #endregion
    }
}
