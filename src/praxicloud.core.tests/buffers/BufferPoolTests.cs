// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.buffers
{
    #region using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using praxicloud.core.buffers;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    #endregion

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class BufferPoolTests
    {
        #region Lifecycle
        /// <summary>
        /// Creates the pool, takes a buffer and returns it to the pool
        /// </summary>
        [TestMethod]
        public void BufferLifecycleValidation()
        {
            var pool = new BufferPool(1, 2, true);

            var buffer = pool.Take();
            var bufferSize = buffer.Length;

            var returnResult = pool.Return(buffer);

            Assert.IsTrue(bufferSize == 1, "Buffer size of retrieved array is not expected");
            Assert.IsTrue(returnResult, "Return result was expected to be true");
            Assert.IsTrue(pool.ClearOnReturn == true, "Clean on return does not match expected value");
            Assert.IsTrue(pool.BufferSize == 1, "Size does not match expected value");
        }

        /// <summary>
        /// Validates that the buffers are cleaned when requested
        /// </summary>
        /// <param name="size">The size of the buffer</param>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(25)]
        [DataRow(86)]
        public void BufferCleanValidation(int size)
        {
            var pool = new BufferPool(size, 1, true);

            var buffer = pool.Take();
            var bufferSize = buffer.Length;

            for(var index = 0; index < buffer.Length; index++)
            {
                buffer[index] = 123;
            }

            var returnResult = pool.Return(buffer);

            buffer = pool.Take();

            var uncleanFound = false;

            for (var index = 0; index < buffer.Length; index++)
            {
                if(buffer[index] != 0)
                {
                    uncleanFound = true;
                }
            }

            Assert.IsFalse(uncleanFound, "Unexpected unclean buffer found");
            Assert.IsTrue(bufferSize == size, "Buffer size of retrieved array is not expected");
            Assert.IsTrue(returnResult, "Return result was expected to be true");
            Assert.IsTrue(pool.ClearOnReturn == true, "Clean on return does not match expected value");
            Assert.IsTrue(pool.BufferSize == size, "Size does not match expected value");
        }

        /// <summary>
        /// Validates that the buffers are not cleaned when requested
        /// </summary>
        /// <param name="size">The size of the buffer</param>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(25)]
        [DataRow(86)]
        public void BufferNotCleanedValidation(int size)
        {
            var pool = new BufferPool(size, 1, false);

            var buffer = pool.Take();
            var bufferSize = buffer.Length;

            for (var index = 0; index < buffer.Length; index++)
            {
                buffer[index] = 123;
            }

            var returnResult = pool.Return(buffer);

            buffer = pool.Take();

            var cleanFound = false;

            for (var index = 0; index < buffer.Length; index++)
            {
                if (buffer[index] == 0)
                {
                    cleanFound = true;
                }
            }

            Assert.IsFalse(cleanFound, "Unexpected clean buffer found");
            Assert.IsTrue(bufferSize == size, "Buffer size of retrieved array is not expected");
            Assert.IsTrue(returnResult, "Return result was expected to be true");
            Assert.IsTrue(pool.ClearOnReturn == false, "Clean on return does not match expected value");
            Assert.IsTrue(pool.BufferSize == size, "Size does not match expected value");
        }

        /// <summary>
        /// Creates the pool, takes a buffer and returns it to the pool
        /// </summary>
        [TestMethod]
        public void BufferLifecycleBeyondCapacityValidation()
        {
            var pool = new BufferPool(5, 1, false);

            var buffer = pool.Take();
            var bufferSize = buffer.Length;

            var buffer2 = pool.Take();
            var buffer2Size = buffer.Length;

            var returnResult = pool.Return(buffer);
            var return2Result = pool.Return(buffer2);

            Assert.IsTrue(bufferSize == 5, "Buffer size of retrieved array is not expected");
            Assert.IsTrue(buffer2Size == 5, "Buffer size of retrieved array is not expected");
            Assert.IsTrue(returnResult, "Return result was expected to be true");
            Assert.IsTrue(pool.ClearOnReturn == false, "Clean on return does not match expected value");
            Assert.IsTrue(pool.BufferSize == 5, "Size does not match expected value");
            Assert.IsFalse(return2Result, "Second buffer not expected to be true on return");
        }

        /// <summary>
        /// Creates the pool, takes a buffer and returns it to the pool
        /// </summary>
        [TestMethod]
        public void BufferLifecycleBeyondCapacityInterfaceValidation()
        {
            IBufferPool pool = new BufferPool(5, 1, false);

            var buffer = pool.Take();
            var bufferSize = buffer.Length;

            var buffer2 = pool.Take();
            var buffer2Size = buffer.Length;

            var returnResult = pool.Return(buffer);
            var return2Result = pool.Return(buffer2);

            Assert.IsTrue(bufferSize == 5, "Buffer size of retrieved array is not expected");
            Assert.IsTrue(buffer2Size == 5, "Buffer size of retrieved array is not expected");
            Assert.IsTrue(returnResult, "Return result was expected to be true");
            Assert.IsTrue(pool.BufferSize == 5, "Size does not match expected value");
            Assert.IsFalse(return2Result, "Second buffer not expected to be true on return");
        }

        /// <summary>
        /// A pool take and return beyond capacity
        /// </summary>
        [TestMethod]
        public void BufferLifecycleBeyondCapacity2Validation()
        {
            var pool = new BufferPool(5, 10, false);
            var bufferList = new List<byte[]>();
            var returnResult = new bool[20];

            for(var index = 0; index < 20; index++)
            {
                bufferList.Add(pool.Take());
            }

            for (var index = 0; index < 20; index++)
            {
                returnResult[index] = pool.Return(bufferList[index]);
            }

            Assert.IsTrue(returnResult.Where(item => item).Count() == 10, "Expected 10 true");
            Assert.IsTrue(returnResult.Where(item => !item).Count() == 10, "Expected 10 false");
        }
        #endregion
        #region Pool Creation
        /// <summary>
        /// Create the buffer pool using the distinct constructor parameters
        /// </summary>
        /// <param name="size">The size of the buffer</param>
        /// <param name="capacity">The capacity of the pool</param>
        /// <param name="cleanOnReturn">True to clean the buffer on return</param>
        [DataTestMethod]
        [DataRow(1, 1, false)]
        [DataRow(2, 234, false)]
        [DataRow(10, 23, false)]
        [DataRow(62, 87, false)]
        [DataRow(934, 12, false)]
        [DataRow(1024, 15, false)]
        [DataRow(45, 10, false)]
        [DataRow(63, 45, false)]
        public void BufferCreationDistinctParametersSuccess(int size, int capacity, bool cleanOnReturn)
        {
            var pool = new BufferPool(size, capacity, cleanOnReturn);

            var buffer = pool.Take();
            var bufferSize = buffer.Length;

            var returnResult = pool.Return(buffer);

            Assert.IsTrue(bufferSize == size, "Buffer size of retrieved array is not expected");
            Assert.IsTrue(returnResult, "Return result was expected to be true");
            Assert.IsTrue(pool.ClearOnReturn == cleanOnReturn, "Clean on return does not match expected value");
            Assert.IsTrue(pool.BufferSize == size, "Size does not match expected value");
        }

        /// <summary>
        /// Create the buffer pool using the configuration object constructor parameters
        /// </summary>
        /// <param name="size">The size of the buffer</param>
        /// <param name="capacity">The capacity of the pool</param>
        /// <param name="cleanOnReturn">True to clean the buffer on return</param>
        [DataTestMethod]
        [DataRow(1, 1, false)]
        [DataRow(2, 234, false)]
        [DataRow(10, 23, false)]
        [DataRow(62, 87, false)]
        [DataRow(934, 12, false)]
        [DataRow(1024, 15, false)]
        [DataRow(45, 10, false)]
        [DataRow(63, 45, false)]
        public void BufferCreationSuccess(int size, int capacity, bool cleanOnReturn)
        {
            var configuration = new BufferPoolConfiguration
            {
                Capacity = capacity,
                Size = size,
                CleanOnReturn = cleanOnReturn
            };
            var pool = new BufferPool(configuration);

            var buffer = pool.Take();
            var bufferSize = buffer.Length;

            var returnResult = pool.Return(buffer);

            Assert.IsTrue(bufferSize == size, "Buffer size of retrieved array is not expected");
            Assert.IsTrue(returnResult, "Return result was expected to be true");
            Assert.IsTrue(pool.ClearOnReturn == cleanOnReturn, "Clean on return does not match expected value");
            Assert.IsTrue(pool.BufferSize == size, "Size does not match expected value");
        }
        #endregion
    }
}
