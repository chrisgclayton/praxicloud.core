// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.security
{
    #region Using Clauses
    using System;
    using Azure.Core;
    using Azure.Identity;
    #endregion

    /// <summary>
    /// Azure AAD authentication for Oauth token access utilities
    /// </summary>
    public static class AzureOauthTokenAuthentication
    {
        #region Constants
        /// <summary>
        /// The maximum content size associated with logged content
        /// </summary>
        public const int DefaultLoggedContentSizeLimit = 4096;
        #endregion
        #region Variables
        /// <summary>
        /// The default authority host used to authenticate against
        /// </summary>
        public static readonly Uri DefaultAuthorityHost = new Uri("https://login.microsoftonline.com/");
        #endregion
        #region Methods
        /// <summary>
        /// Gets a managed service identity credential for retrieving Oauth tokens
        /// </summary>
        /// <returns>The associated token credential instance</returns>
        public static TokenCredential GetOauthTokenCredentialFromManagedIdentity()
        {
            return GetOauthTokenCredentialFromManagedIdentity((TokenCredentialOptions)null);
        }

        /// <summary>
        /// Gets a managed service identity credential for retrieving Oauth tokens
        /// </summary>
        /// <param name="options">The token credential options to use for authenticating</param>
        /// <returns>The associated token credential instance</returns>
        public static TokenCredential GetOauthTokenCredentialFromManagedIdentity(TokenCredentialOptions options)
        {
            return options == null ? new ManagedIdentityCredential() : new ManagedIdentityCredential(options: options);
        }

        /// <summary>
        /// Gets a managed service identity credential for retrieving Oauth tokens
        /// </summary>
        /// <param name="clientId">The user id to assign the managed identity to</param>
        /// <returns>The associated token credential instance</returns>
        public static TokenCredential GetOauthTokenCredentialFromManagedIdentity(string clientId)
        {
            return GetOauthTokenCredentialFromManagedIdentity(clientId, null);
        }

        /// <summary>
        /// Gets a managed service identity credential for retrieving Oauth tokens
        /// </summary>
        /// <param name="clientId">The user id to assign the managed identity to</param>
        /// <param name="options">The token credential options to use for authenticating</param>
        /// <returns>The associated token credential instance</returns>
        public static TokenCredential GetOauthTokenCredentialFromManagedIdentity(string clientId, TokenCredentialOptions options)
        {
            Guard.NotNullOrWhitespace(nameof(clientId), clientId);

            return options == null ? new ManagedIdentityCredential(clientId) : new ManagedIdentityCredential(clientId, options);
        }

        /// <summary>
        /// Retrieves a credential that can retrieve an OAuth token based on a client secret set
        /// </summary>
        /// <param name="tenantId">The Azure Active Directory tenant id</param>
        /// <param name="clientId">The Azure Active Directory client id (application id)</param>
        /// <param name="clientSecret">The Azure Active Directory client secret</param>
        /// <returns>The associated token credential instance</returns>
        public static TokenCredential GetOauthTokenCredentialFromClientSecret(string tenantId, string clientId, string clientSecret)
        {
            return GetOauthTokenCredentialFromClientSecret(tenantId, clientId, clientSecret, null);
        }

        /// <summary>
        /// Retrieves a credential that can retrieve an OAuth token based on a client secret set
        /// </summary>
        /// <param name="tenantId">The Azure Active Directory tenant id</param>
        /// <param name="clientId">The Azure Active Directory client id (application id)</param>
        /// <param name="clientSecret">The Azure Active Directory client secret</param>
        /// <param name="options">The token credential options to use for authenticating</param>
        /// <returns>The associated token credential instance</returns>
        public static TokenCredential GetOauthTokenCredentialFromClientSecret(string tenantId, string clientId, string clientSecret, TokenCredentialOptions options)
        {
            Guard.NotNullOrWhitespace(nameof(tenantId), tenantId);
            Guard.NotNullOrWhitespace(nameof(clientId), clientId);
            Guard.NotNullOrWhitespace(nameof(clientSecret), clientSecret);

            return options == null ? new ClientSecretCredential(tenantId, clientId, clientSecret) : new ClientSecretCredential(tenantId, clientId, clientSecret, options);
        }

        /// <summary>
        /// Creates a basic token credential options instance with the authority host set
        /// </summary>
        /// <param name="authorityHost">The authroity host to use or the default if null</param>
        /// <returns>An instance of the credential options</returns>
        public static TokenCredentialOptions GetOptions(Uri authorityHost = null)
        {
            var options = new TokenCredentialOptions();

            if (authorityHost != null)
            {
                options.AuthorityHost = authorityHost;
            }

            return options;
        }

        /// <summary>
        /// Configures the retries on an options instance
        /// </summary>
        /// <param name="options">The options instance</param>
        /// <param name="mode">The retry mode</param>
        /// <param name="maximumRetries">The maximum retries to be performed</param>
        /// <param name="delay">The delay before invoking the first retry</param>
        /// <param name="maximumDelay">The maximum delay allowed between retries</param>
        /// <param name="networkTimeout">The network timeout for a single operation</param>
        public static void ConfigureRetries(this TokenCredentialOptions options, RetryMode mode, int maximumRetries, TimeSpan delay, TimeSpan maximumDelay, TimeSpan networkTimeout)
        {
            options.Retry.Delay = delay;
            options.Retry.MaxDelay = maximumDelay;
            options.Retry.MaxRetries = maximumRetries;
            options.Retry.NetworkTimeout = networkTimeout;
            options.Retry.Mode = mode;
        }

        /// <summary>
        /// Configures the diagnostics information associated with a credential option
        /// </summary>
        /// <param name="options">The option instance</param>
        /// <param name="applicationId">The application id</param>
        /// <param name="isDistributedTracingEnabled">True if distributed tracing is in use</param>
        /// <param name="isLoggingContentEnabled">True if logging of content is enabled</param>
        /// <param name="isLoggingEnabled">True if logging is enabled</param>
        /// <param name="isTelemetryEnabled">True if telemetry is enabled</param>
        /// <param name="loggingContentSizeLimit">The maximum size of the content being logged</param>
        public static void ConfigureDiagnostics(this TokenCredentialOptions options, string applicationId, bool isDistributedTracingEnabled, bool isLoggingContentEnabled, bool isLoggingEnabled, bool isTelemetryEnabled, int loggingContentSizeLimit)
        {
            options.Diagnostics.ApplicationId = applicationId;
            options.Diagnostics.IsDistributedTracingEnabled = isDistributedTracingEnabled;
            options.Diagnostics.IsLoggingContentEnabled = isLoggingContentEnabled;
            options.Diagnostics.IsLoggingEnabled = isLoggingEnabled;
            options.Diagnostics.IsTelemetryEnabled = isTelemetryEnabled;
            options.Diagnostics.LoggedContentSizeLimit = loggingContentSizeLimit;
        }
        #endregion
    }
}
