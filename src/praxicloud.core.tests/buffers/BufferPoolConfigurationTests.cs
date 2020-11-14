// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.buffers
{
    #region using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using praxicloud.core.buffers;
    using System.Diagnostics.CodeAnalysis;
    #endregion

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class BufferPoolConfigurationTests
    {
        #region Persistence
        /// <summary>
        /// Confirms the persisted value matches the initialized
        /// </summary>
        /// <param name="capacity">Capacity value</param>
        /// <param name="cleanOnReturn">True if buffer should be cleaned on return</param>
        /// <param name="size">The size of the buffer</param>
        [DataTestMethod]
        [DataRow(0, false, 0)]
        [DataRow(0, true, 0)]
        [DataRow(1, false, 0)]
        [DataRow(1, true, 0)]
        [DataRow(0, false, 1)]
        [DataRow(0, true, 1)]
        [DataRow(1, true, 1)]
        [DataRow(1024, true, 0)]
        [DataRow(1024, false, 0)]
        [DataRow(0, true, 1024)]
        [DataRow(0, false, 1024)]
        public void TestPersistence(int capacity, bool cleanOnReturn, int size)
        {
            var configuration = new BufferPoolConfiguration
            {
                Capacity = capacity,
                CleanOnReturn = cleanOnReturn,
                Size = size
            };

            Assert.IsTrue(configuration.Capacity == capacity, "Capacity is not expected value");
            Assert.IsTrue(configuration.CleanOnReturn == cleanOnReturn, "Clean on return is not expected value");
            Assert.IsTrue(configuration.Size == size, "Size is not expected value");
        }
        #endregion
    }
}
