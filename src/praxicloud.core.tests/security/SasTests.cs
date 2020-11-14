// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using praxicloud.core.security;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    #endregion

    /// <summary>
    /// A set of tests to validate the conversion to and from secure strings
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SasTests
    {
        #region SAS Methods
        /// <summary>
        /// Simple generation and decomposition of the created token
        /// </summary>
        /// <param name="resourceUri">The resource URI for the token</param>
        /// <param name="key">The key used to sign the token</param>
        /// <param name="policyName">The policy name the key is associated with</param>
        /// <param name="expiryInSeconds">The expiration time in seconds</param>
        [DataTestMethod]
        [DataRow("/localhost/device/events/", "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE=", "policy1", 120)]
        [DataRow("/localhost/device/", "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE=", "policy2", 45)]
        public void GenerateAndDecomposeToken(string resourceUri, string key, string policyName, int expiryInSeconds)
        {
            var token = SharedAccessTokens.GenerateSasToken(resourceUri, key, policyName, expiryInSeconds);

            System.Diagnostics.Debug.Print(token);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(token), "The token should not be null or whitespace");
            Assert.IsTrue(SharedAccessTokens.DecomposeSasToken(token, out var outputResourceUri, out var outputPolicyName, out var _, out var _, out var _), "Successfully decomposes");
            Assert.IsTrue(string.Equals(outputResourceUri, resourceUri, StringComparison.Ordinal), "Resource URI not expected");
            Assert.IsTrue(string.Equals(outputPolicyName, policyName, StringComparison.Ordinal), "Policy name not expected");
        }

        /// <summary>
        /// Simple generation and decomposition of the created token with additional token elements
        /// </summary>
        /// <param name="resourceUri">The resource URI for the token</param>
        /// <param name="key">The key used to sign the token</param>
        /// <param name="policyName">The policy name the key is associated with</param>
        /// <param name="expiryInSeconds">The expiration time in seconds</param>
        [DataTestMethod]
        [DataRow("/localhost/device/events/", "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE=", "policy1", 120)]
        [DataRow("/localhost/device/", "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE=", "policy2", 45)]
        public void GenerateAndDecomposeTokenWithAdditionalElements(string resourceUri, string key, string policyName, int expiryInSeconds)
        {
            var token = SharedAccessTokens.GenerateSasToken(resourceUri, key, policyName, expiryInSeconds);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(token), "The token should not be null or whitespace");
            Assert.IsFalse(SharedAccessTokens.DecomposeSasToken($"{ token }&ad=additionalstuff", out var _, out var _, out var _, out var _, out var _), "Successfully decomposes");
        }

        /// <summary>
        /// Simple generation, decomposition and validation of the created token
        /// </summary>
        /// <param name="resourceUri">The resource URI for the token</param>
        /// <param name="key">The key used to sign the token</param>
        /// <param name="policyName">The policy name the key is associated with</param>
        /// <param name="expiryInSeconds">The expiration time in seconds</param>
        [DataTestMethod]
        [DataRow("/localhost/device/events/", "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE=", "policy1", 120)]
        [DataRow("/localhost/device/", "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE=", "policy2", 45)]
        public void GenerateAndValidateToken(string resourceUri, string key, string policyName, int expiryInSeconds)
        {
            var token = SharedAccessTokens.GenerateSasToken(resourceUri, key, policyName, expiryInSeconds);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(token), "The token should not be null or whitespace");
            Assert.IsTrue(SharedAccessTokens.DecomposeSasToken(token, out var outputResourceUri, out var outputPolicyName, out var _, out var stringToValidate, out var signature), "Successfully decomposes");
            Assert.IsTrue(SharedAccessTokens.IsSignatureValid(signature, key, stringToValidate), "The signature was not valid");
            Assert.IsTrue(string.Equals(outputResourceUri, resourceUri, StringComparison.Ordinal), "Resource URI not expected");
            Assert.IsTrue(string.Equals(outputPolicyName, policyName, StringComparison.Ordinal), "Policy name not expected");
        }

        /// <summary>
        /// Validation failure of the token due to null or empty values
        /// </summary>
        /// <param name="signature">The signature to provide</param>
        /// <param name="key">The key to use for validation</param>
        /// <param name="stringToValidate">The string to sign</param>
        [DataTestMethod]
        [DataRow(null, "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE", "validatethis")]
        [DataRow("DemoSignature", null, "validatethis")]
        [DataRow(null, null, null)]
        [DataRow(null, null, "validatethis")]
        [DataRow("DemoSignature", null, null)]
        [DataRow(null, "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE", null)]
        [DataRow(" ", "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE", "validatethis")]
        [DataRow("", "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE", "validatethis")]
        [DataRow("0", "BMpm9o9Q9ahLkIEyR+jUgpob69H5Vc1qU7yZzHRSoxE", "validatethis")]
        public void ValidationFailure(string signature, string key, string stringToValidate)
        {
            byte[] signatureBytes;

            if(signature == "0")
            {
                signatureBytes = new byte[0];
            }
            else if(string.IsNullOrWhiteSpace(signature))
            {
                signatureBytes = null;
            }
            else
            {
                signatureBytes = Encoding.ASCII.GetBytes(signature);
            }

            Assert.IsFalse(SharedAccessTokens.IsSignatureValid(signatureBytes, key, stringToValidate), "The signature was not valid");
        }

        /// <summary>
        /// Tests the failure modes due to missing elements
        /// </summary>
        [TestMethod]
        public void DecompositionMissingElementsFailure()
        {            
            Assert.IsFalse(SharedAccessTokens.DecomposeSasToken("SharedAccessSignature sig=uRaXRcD2L%2BxXDzq5nSu%2FSEQw0CsQT%2Fx1AmEKaWocenw%3D&se=1599667990&skn=policy1", out var _, out var _, out var _, out var _, out var _), "Not expected to pass decomposition missing resource");
            Assert.IsFalse(SharedAccessTokens.DecomposeSasToken("SharedAccessSignature sr=%2Flocalhost%2Fdevice%2Fevents%2F&se=1599667990&skn=policy1", out var _, out var _, out var _, out var _, out var _), "Not expected to pass decomposition missing signature");
            Assert.IsFalse(SharedAccessTokens.DecomposeSasToken("SharedAccessSignature sr=%2Flocalhost%2Fdevice%2Fevents%2F&sig=uRaXRcD2L%2BxXDzq5nSu%2FSEQw0CsQT%2Fx1AmEKaWocenw%3D&se=1599667990", out var _, out var _, out var _, out var _, out var _), "Not expected to pass decomposition missing policy name");
            Assert.IsFalse(SharedAccessTokens.DecomposeSasToken("SharedAccessSignature sr=%2Flocalhost%2Fdevice%2Fevents%2F&sig=uRaXRcD2L%2BxXDzq5nSu%2FSEQw0CsQT%2Fx1AmEKaWocenw%3D&skn=policy1", out var _, out var _, out var _, out var _, out var _), "Not expected to pass decomposition missing expires at");
        }
        #endregion
    }
}
