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
    public class GuardExceptionTests
    {
        #region Test Entry Points
        /// <summary>
        /// Valid values for name and message in various combinations should not generate an exception
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="message">The message that represents the exception</param>
        [DataTestMethod]
        [DataRow("name1", "message1")]
        [DataRow(",[]?{}//\\", "message1")]
        [DataRow("parametername", "message1")]
        [DataRow("parameter name", "message1")]
        [DataRow(" parameter name", "message1")]
        [DataRow("parameter name ", "message1")]
        [DataRow("  parameter name  ", "message1")]
        [DataRow("           parameter name         ", "message1")]
        [DataRow("       parameter name       ", "message1")]
        [DataRow("a", "message1")]
        [DataRow("1", "message1")]
        [DataRow("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "message1")]
        [DataRow("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "message1")]
        [DataRow("name1", "message")]
        [DataRow("name1", "message1")]
        [DataRow("name1", "1")]
        [DataRow("name1", "a")]
        [DataRow("name1", "111111")]
        [DataRow("name1", "This is a test message with spaces in it")]
        [DataRow("name1", " message1")]
        [DataRow("name1", "message1 ")]
        [DataRow("name1", " message1 ")]
        [DataRow("name1", ",[]?{}//\\")]
        [DataRow("name1", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\r\nadfasdfllllllllllllllliiiiiii\tasdfasdf")]
        [DataRow("", " message1 ")]
        [DataRow(null, " message1 ")]
        [DataRow(" ", " message1 ")]
        [DataRow("name1", " ")]
        [DataRow(" ", " ")]
        public void PropertyAssignment(string name, string message)
        {
            var exception = new GuardException(message, name);
                       
            Assert.IsTrue(string.Equals(exception.ParamName, name, System.StringComparison.Ordinal), $"Parameter name ({ exception.ParamName ?? "null" }) does not match name ({ name ?? "null" })");

            var messageSuffix = string.IsNullOrEmpty(name) ? "" : $" (Parameter '{ name }')";
            var expectedMessage = $"{ message }{messageSuffix}";
            Assert.IsTrue(string.Equals(exception.Message, expectedMessage, System.StringComparison.Ordinal), $"Parameter message ({ exception.Message ?? "null" }) does not match message ({ expectedMessage })");
        }

        /// <summary>
        /// Valid values for name and message in various combinations should not generate an exception
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="message">The message that represents the exception</param>
        [DataTestMethod]
        [DataRow("name1", "")]
        [ExpectedException(typeof(ArgumentException))]
        public void MessageNullFailure(string name, string message)
        {
            var exception = new GuardException(message, name);

            Assert.IsTrue(string.Equals(exception.ParamName, name, System.StringComparison.Ordinal), $"Parameter name ({ exception.ParamName ?? "null" }) does not match name ({ name ?? "null" })");

            var messageSuffix = string.IsNullOrEmpty(name) ? "" : $" (Parameter '{ name }')";
            var expectedMessage = $"{ message }{messageSuffix}";
            Assert.IsTrue(string.Equals(exception.Message, expectedMessage, System.StringComparison.Ordinal), $"Parameter message ({ exception.Message ?? "null" }) does not match message ({ expectedMessage })");
        }

        /// <summary>
        /// Confirms arguement exception can catch the exception
        /// </summary>
        [TestMethod]
        public void CatchBaseType()
        {
            var exceptionType = 0;

            try
            {
                throw new GuardException("bases type test", "base");
            }
            catch(ArgumentException)
            {
                exceptionType = 1;
            }

            Assert.IsTrue(exceptionType == 1, "The exception was not an ArgumentException");
        }
        #endregion
    }
}
