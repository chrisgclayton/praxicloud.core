// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.containers
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics.CodeAnalysis;
    using praxicloud.core.containers;
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// Tests the health services
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]

    public class HealthChecks
    {
        /// <summary>
        /// A simple test that iterates 10 times
        /// </summary>
        [TestMethod]
        public void SimpleTest()
        {
            var probe = new HealthCheck();

            for(var index = 0; index < 10; index++)
            {
                probe.IsHealthyAsync().GetAwaiter().GetResult();
            }

            Assert.IsTrue(probe.Count == 10, "Probe count not expected");
        }
    }

    /// <summary>
    /// Basically health check interface
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class HealthCheck : IHealthCheck
    {
        /// <summary>
        /// The number of times it has been called
        /// </summary>
        private long _count = 0;

        /// <summary>
        /// The number of times it has been called
        /// </summary>
        public long Count => _count;

        /// <inheritdoc />
        public Task<bool> IsHealthyAsync()
        {
            Interlocked.Increment(ref _count);

            return Task.FromResult(true);
        }
    }
}
