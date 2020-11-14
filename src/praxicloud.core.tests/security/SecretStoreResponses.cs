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
    public class SecretStoreResponses
    {
        /// <summary>
        /// Checks that the values set into the secret store response are the same as retrieved
        /// </summary>
        /// <param name="statusCode">The status code to store</param>
        /// <param name="timeToExecute">The time it took to execute</param>
        /// <param name="includeException">True if an application exception should be used</param>
        [DataTestMethod]
        [DataRow(0, "00:00:23.123", false)]
        [DataRow(100, "00:00:23.423", false)]
        [DataRow(200, "00:00:23.523", false)]
        [DataRow(299, "00:00:23.623", false)]
        [DataRow(300, null, false)]
        [DataRow(400, null, false)]
        [DataRow(200, "00:00:23.523", true)]
        public void PersistenceTest(int statusCode, string timeToExecute, bool includeException)
        {
            Exception exception = includeException ? new ApplicationException("ABC") : null;
            TimeSpan? timeToExecuteValue = null;

            var response = new SecretStoreResponse
            {
                Exception = exception,
                IsSuccessCode = statusCode >= 200 && statusCode < 300,
                StatusCode = statusCode
            };

            if(!string.IsNullOrWhiteSpace(timeToExecute))
            {
                timeToExecuteValue = TimeSpan.Parse(timeToExecute);
                response.TimeToExecute = timeToExecuteValue;
            }

            Assert.IsTrue(response.Exception == exception, "Exception not expected value");
            Assert.IsTrue(response.StatusCode == statusCode, "Status Code not expected value");
            Assert.IsTrue(response.IsSuccessCode == (statusCode >= 200 && statusCode < 300), "Is Success code not expected value");
            Assert.IsTrue(response.Exception == exception, "Exception not expected value");
            Assert.IsTrue((response.TimeToExecute ?? TimeSpan.Zero) == (timeToExecuteValue ?? TimeSpan.Zero), "Time to execute not expected value");
            Assert.IsTrue(response == response.IsSuccessCode, "Implicit conversion is expected to be same as IsSuccessCode");
        }

        /// <summary>
        /// Checks that the values set into the secret store response are the same as retrieved
        /// </summary>
        /// <param name="statusCode">The status code to store</param>
        /// <param name="timeToExecute">The time it took to execute</param>
        /// <param name="includeException">True if an application exception should be used</param>
        /// <param name="payload">The value of the generic element</param>
        [DataTestMethod]
        [DataRow(0, "00:00:23.123", false, "payload1")]
        [DataRow(100, "00:00:23.423", false, "payload1")]
        [DataRow(200, "00:00:23.523", false, "payload1")]
        [DataRow(299, "00:00:23.623", false, null)]
        [DataRow(300, null, false, "")]
        [DataRow(400, null, false, " ")]
        [DataRow(200, "00:00:23.523", true, "payload1")]

        public void PersistenceGenericTest(int statusCode, string timeToExecute, bool includeException, string payload)
        {
            Exception exception = includeException ? new ApplicationException("ABC") : null;
            TimeSpan? timeToExecuteValue = null;

            var response = new SecretStoreResponse<string>
            {
                Exception = exception,
                IsSuccessCode = statusCode >= 200 && statusCode < 300,
                StatusCode = statusCode,
                Value = payload
            };

            if (!string.IsNullOrWhiteSpace(timeToExecute))
            {
                timeToExecuteValue = TimeSpan.Parse(timeToExecute);
                response.TimeToExecute = timeToExecuteValue;
            }

            Assert.IsTrue(response.Exception == exception, "Exception not expected value");
            Assert.IsTrue(response.StatusCode == statusCode, "Status Code not expected value");
            Assert.IsTrue(response.IsSuccessCode == (statusCode >= 200 && statusCode < 300), "Is Success code not expected value");
            Assert.IsTrue(response.Exception == exception, "Exception not expected value");
            Assert.IsTrue((response.TimeToExecute ?? TimeSpan.Zero) == (timeToExecuteValue ?? TimeSpan.Zero), "Time to execute not expected value");
            Assert.IsTrue(string.Equals(response.Value, payload, StringComparison.Ordinal), "The value was not the expected value");
            Assert.IsTrue(response == response.IsSuccessCode, "Implicit conversion is expected to be same as IsSuccessCode");

            string responseText = response;                        
            Assert.IsTrue(responseText == payload, "Conversion to string shoudl be the payload");
        }
    }
}
