// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using praxicloud.core.security;
    using System;
    using System.Diagnostics.CodeAnalysis;
    #endregion

    /// <summary>
    /// A set of tests to validate the GuardException type is operating as execpted
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TokenCredentialTests
    {
        #region Client Secret Credentials
        /// <summary>
        /// Creates a credential to retrieve Oauth tokens
        /// </summary>
        /// <param name="tenantId">The Azure Active Directory tenant</param>
        /// <param name="clientId">The Azure Active Directory client (application) id</param>
        /// <param name="clientSecret">The Azure Active Directory client secret</param>
        [DataTestMethod]
        [DataRow("tenantId", "clientId", "clientSecret")]
        public void ClientSecretCredentialTest(string tenantId, string clientId, string clientSecret)
        {            
            var credential = AzureOauthTokenAuthentication.GetOauthTokenCredentialFromClientSecret(tenantId, clientId, clientSecret);

            Assert.IsNotNull(credential, "Credential should not be null");
        }

        /// <summary>
        /// Creates a credential to retrieve Oauth tokens
        /// </summary>
        /// <param name="tenantId">The Azure Active Directory tenant</param>
        /// <param name="clientId">The Azure Active Directory client (application) id</param>
        /// <param name="clientSecret">The Azure Active Directory client secret</param>
        /// <param name="useHostString">True if the host string should be provided to the options</param>
        [DataTestMethod]
        [DataRow("tenantId", "clientId", "clientSecret", false)]
        [DataRow("tenantId", "clientId", "clientSecret", true)]
        public void ClientSecretCredentialWithOptionsTest(string tenantId, string clientId, string clientSecret, bool useHostString)
        {
            var options = AzureOauthTokenAuthentication.GetOptions(useHostString ? AzureOauthTokenAuthentication.DefaultAuthorityHost : null);
            var credential = AzureOauthTokenAuthentication.GetOauthTokenCredentialFromClientSecret(tenantId, clientId, clientSecret, options);

            Assert.IsNotNull(credential, "Credential should not be null");
        }

        /// <summary>
        /// Creates a credential to retrieve Oauth tokens but with failures due to parameter tests
        /// </summary>
        /// <param name="tenantId">The Azure Active Directory tenant</param>
        /// <param name="clientId">The Azure Active Directory client (application) id</param>
        /// <param name="clientSecret">The Azure Active Directory client secret</param>
        /// <param name="useHostString">True if the host string should be provided to the options</param>
        [DataTestMethod]
        [DataRow(null, "clientId", "clientSecret", false)]
        [DataRow("tenantId", null, "clientSecret", false)]
        [DataRow("tenantId", "clientId", null, false)]
        [DataRow(null, null, "clientSecret", false)]
        [DataRow(null, "clientId", null, false)]
        [DataRow("tenantId", null, null, false)]
        [ExpectedException(typeof(GuardException))]
        public void ClientSecretCredentialWithOptionsFailureTest(string tenantId, string clientId, string clientSecret, bool useHostString)
        {
            var options = AzureOauthTokenAuthentication.GetOptions(useHostString ? AzureOauthTokenAuthentication.DefaultAuthorityHost : null);
            var credential = AzureOauthTokenAuthentication.GetOauthTokenCredentialFromClientSecret(tenantId, clientId, clientSecret, options);

            Assert.IsNotNull(credential, "Credential should not be null");
        }
        #endregion
        #region Managed Identity Credentials
        /// <summary>
        /// Creates a credential to retrieve Oauth tokens
        /// </summary>
        /// <param name="clientId">The managed identity provider client id</param>
        [DataTestMethod]
        [DataRow("Client1")]
        [DataRow("Cl2")]
        public void ManagedIdentityCredentialTest(string clientId)
        {
            var credential = AzureOauthTokenAuthentication.GetOauthTokenCredentialFromManagedIdentity(clientId);

            Assert.IsNotNull(credential, "Credential should not be null");
        }

        /// <summary>
        /// Creates a credential to retrieve Oauth tokens
        /// </summary>
        /// <param name="clientId">The managed identity provider client id</param>
        /// <param name="useHostString">True if the host string should be provided to the options</param>
        [DataTestMethod]
        [DataRow("Client1", true)]
        [DataRow("Cl2", true)]
        [DataRow("Client1", false)]
        [DataRow("Cl2", false)]
        public void ManagedIdentityWithOptionsTest(string clientId,bool useHostString)
        {
            var options = AzureOauthTokenAuthentication.GetOptions(useHostString ? AzureOauthTokenAuthentication.DefaultAuthorityHost : null);
            var credential = AzureOauthTokenAuthentication.GetOauthTokenCredentialFromManagedIdentity(clientId, options);

            Assert.IsNotNull(credential, "Credential should not be null");
        }

        /// <summary>
        /// Creates a credential to retrieve Oauth tokens expecting an exception due to parameter violation
        /// </summary>
        /// <param name="clientId">The managed identity provider client id</param>
        [DataTestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        [ExpectedException(typeof(GuardException))]
        public void ManagedIdentityCredential1FailureTest(string clientId)
        {
            var credential = AzureOauthTokenAuthentication.GetOauthTokenCredentialFromManagedIdentity(clientId);
        }

        /// <summary>
        /// Creates a credential to retrieve Oauth tokens
        /// </summary>
        /// <param name="clientId">The managed identity provider client id</param>
        [TestMethod]
        public void ManagedIdentityCredentialBaseTest()
        {
            var credential = AzureOauthTokenAuthentication.GetOauthTokenCredentialFromManagedIdentity();

            Assert.IsNotNull(credential, "Credential should not be null");
        }

        /// <summary>
        /// Creates a credential to retrieve Oauth tokens
        /// </summary>
        /// <param name="useHostString">True if the host string should be provided to the options</param>
        [DataTestMethod]
        [DataRow(true)]
        [DataRow(true)]
        [DataRow(false)]
        [DataRow(false)]
        public void ManagedIdentityBaseWithOptionsTest(bool useHostString)
        {
            var options = AzureOauthTokenAuthentication.GetOptions(useHostString ? AzureOauthTokenAuthentication.DefaultAuthorityHost : null);
            var credential = AzureOauthTokenAuthentication.GetOauthTokenCredentialFromManagedIdentity(options);

            Assert.IsNotNull(credential, "Credential should not be null");
        }
        #endregion
        #region Options
        /// <summary>
        /// Creates a token option and configures the retry options on it
        /// </summary>
        [TestMethod]
        public void ConfigureOptionRetries()
        {
            var options = AzureOauthTokenAuthentication.GetOptions();

            AzureOauthTokenAuthentication.ConfigureRetries(options, Azure.Core.RetryMode.Exponential, 100, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2));

            Assert.IsTrue(options.Retry.Delay == TimeSpan.FromSeconds(5), "Delay is not expected");
            Assert.IsTrue(options.Retry.MaxDelay == TimeSpan.FromMinutes(1), "MaximumDelay is not expected");
            Assert.IsTrue(options.Retry.NetworkTimeout == TimeSpan.FromMinutes(2), "Network Timeout is not expected");
            Assert.IsTrue(options.Retry.MaxRetries == 100, "Maximum retries is not expected");
        }

        /// <summary>
        /// Creates a token option and configures the diagnostic options on it
        /// </summary>
        [TestMethod]
        public void ConfigureOptionDiagnostics()
        {
            var options = AzureOauthTokenAuthentication.GetOptions();

            AzureOauthTokenAuthentication.ConfigureDiagnostics(options, "app1", true, true, true, true, 5000);

            Assert.IsTrue(options.Diagnostics.ApplicationId == "app1", "Application Id is not expected");
            Assert.IsTrue(options.Diagnostics.IsDistributedTracingEnabled, "DistributedTracingEnabled is not expected");
            Assert.IsTrue(options.Diagnostics.IsLoggingContentEnabled, "LoggingContentEnabled is not expected");
            Assert.IsTrue(options.Diagnostics.IsLoggingEnabled, "LoggingEnabled is not expected");
            Assert.IsTrue(options.Diagnostics.IsTelemetryEnabled, "LoggingEnabled is not expected");
            Assert.IsTrue(options.Diagnostics.LoggedContentSizeLimit == 5000, "LoggedContentSizeLimit is not expected");
            Assert.IsTrue(options.Diagnostics.LoggedHeaderNames.Count > 0, "LoggedHeaderNames is not expected");
            Assert.IsTrue(options.Diagnostics.LoggedQueryParameters.Count == 0, "LoggedQueryParameters is not expected");
        }
        #endregion





    }
}
