// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using praxicloud.core.security;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security;
    #endregion

    /// <summary>
    /// A set of tests to validate the conversion to and from secure strings
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SecureStringTests
    {
        #region Validation
        /// <summary>
        /// Converts a plain text string to a secure string and back 
        /// </summary>
        [DataTestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("    ")]
        [DataRow("a")]
        [DataRow("1")]
        [DataRow(";")]
        [DataRow("}")]
        [DataRow("test it out")]
        [DataRow(" test it out")]
        [DataRow("test it out ")]
        [DataRow(" test it out ")]
        public void SimpleReconversion(string value)
        {
            var cipher = value.GetSecureString();
            var plain = cipher.SecureStringToString();

            Assert.IsFalse(string.Equals(value, cipher.ToString(), StringComparison.Ordinal), "Cipher ToString() not expected value");
            Assert.IsTrue(string.Equals(value, plain, StringComparison.Ordinal), "Plain text not expected value");
        }

        /// <summary>
        /// Checks for expected failure on converting null to secure string
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void NullToSecureString()
        {
            string value = null;
            var cipher = value.GetSecureString();
        }

        /// <summary>
        /// Checks for expected failure on converting null secure string to strong
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void NullToString()
        {
            SecureString cipher = null;
            var plain = cipher.SecureStringToString();
        }
        #endregion
    }
}
