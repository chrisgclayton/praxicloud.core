// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using praxicloud.core.containers;
    using praxicloud.core.security;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    #endregion

    /// <summary>
    /// A set of tests to validate the GuardException type is operating as execpted
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CertificateTests
    {
        #region Validation
        /// <summary>
        /// Ensures no policy errors succeed
        /// </summary>
        [TestMethod]
        public void ValidateNoViolationsAllowed()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "security", "TestValidationCertificate.pfx");
            var certificate = new X509Certificate2(path, "abc123");
            var policyErrors = SslPolicyErrors.None;
            var chain = X509Chain.Create();


            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
            chain.Build(certificate);

            var isValid = CertificateValidation.CertificateValidationCallBack(this, certificate, chain, policyErrors);

            Assert.IsTrue(isValid, "The certificate is expected to pass validation");
        }


        /// <summary>
        /// Ensures no policy errors with self signed allowed
        /// </summary>
        [TestMethod]
        public void ValidateSelfSignedAllowedNoViolation()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "security", "TestValidationCertificate.pfx");
            var certificate = new X509Certificate2(path, "abc123");
            var chain = X509Chain.Create();

            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
            chain.Build(certificate);

            var isValid = CertificateValidation.CertificateValidationCallBackAllowsSelfSigned(this, certificate, chain, SslPolicyErrors.None);

            Assert.IsTrue(isValid, "The certificate is expected to pass validation");
        }

        /// <summary>
        /// Ensures no policy errors with self signed allowed
        /// </summary>
        [TestMethod]
        public void ValidateSelfSignedAllowed()
        {
            //     var path =  Path.Combine(Environment.CurrentDirectory, "security", "TestValidationCertificate.pfx");
            var path = Path.Combine(Environment.CurrentDirectory, "security", ContainerEnvironment.IsLinux ? "my_contoso_local.pfx" : "TestValidationCertificate.pfx");
            var certificate = new X509Certificate2(path, "abc123");
            var chain = X509Chain.Create();

            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
            chain.Build(certificate);

            var isValid = CertificateValidation.CertificateValidationCallBackAllowsSelfSigned(this, certificate, chain, SslPolicyErrors.RemoteCertificateChainErrors);

            Assert.IsTrue(isValid, "The certificate is expected to pass validation");
        }

        /// <summary>
        /// Ensures no policy errors with self signed allowed if no chain provided
        /// </summary>
        [TestMethod]
        public void ValidateSelfSignedAllowedWithNoChain()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "security", "TestValidationCertificate.pfx");
            var certificate = new X509Certificate2(path, "abc123");

            var isValid = CertificateValidation.CertificateValidationCallBackAllowsSelfSigned(this, certificate, null, SslPolicyErrors.RemoteCertificateChainErrors);

            Assert.IsTrue(isValid, "The certificate is expected to pass validation");
        }
        #endregion
    }
}
