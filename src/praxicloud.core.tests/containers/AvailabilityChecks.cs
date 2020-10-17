// Copyright (c) Chris Clayton. All rights reserved.
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
    /// Tests the availabilities services
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]

    public class AvailabilityChecks
    {
        /// <summary>
        /// A simple test that iterates 10 times
        /// </summary>
        [TestMethod]
        public void SimpleTest()
        {
            var probe = new AvailabilityCheck();

            for (var index = 0; index < 10; index++)
            {
                probe.IsAvailableAsync().GetAwaiter().GetResult();
            }

            Assert.IsTrue(probe.Count == 10, "Probe count not expected");
        }
    }

    /// <summary>
    /// Basically availability check interface
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class AvailabilityCheck : IAvailabilityCheck
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
        public Task<bool> IsAvailableAsync()
        {
            Interlocked.Increment(ref _count);

            return Task.FromResult(true);
        }
    }
}
