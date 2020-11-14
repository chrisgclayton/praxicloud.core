// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.buffers
{    
    #region Using Clauses
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using praxicloud.core.security;
    #endregion

    /// <summary>
    /// A buffer pool used to reduce the number of array allocations and resulting Garbage Collections
    /// </summary>
    public class BufferPool : IBufferPool
    {
        #region Variables
        /// <summary>
        /// The first buffer in the pool, used to optimize performance
        /// </summary>
        private byte[] _firstBuffer;

        /// <summary>
        /// The buffers in the pool
        /// </summary>
        private readonly byte[][] _buffers;
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="configuration">Configuration details defining the allocation behavior of the buffer pool</param>
        public BufferPool(BufferPoolConfiguration configuration)
        {
            Guard.NotNull(nameof(configuration), configuration);
            Guard.NotLessThan("size", configuration.Size, 1);
            Guard.NotLessThan("capacity", configuration.Capacity, 1);         

            ClearOnReturn = configuration.CleanOnReturn;
            BufferSize = configuration.Size;
            _buffers = new byte[configuration.Capacity - 1][];
        }

        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="size">The size of the buffers in the pool</param>
        /// <param name="capacity">The capacitity of the buffer pool (number of buffers)</param>
        /// <param name="cleanOnReturn">True if the buffer should be cleared of contents when returned to the pool</param>
        public BufferPool(int size, int capacity, bool cleanOnReturn) : this(new BufferPoolConfiguration 
                                                                             {
                                                                                Capacity = capacity,
                                                                                Size = size,
                                                                                CleanOnReturn = cleanOnReturn
                                                                             })
        {
        }
        #endregion
        #region Properties
        /// <inheritdoc />
        public int BufferSize { get; }

        /// <summary>
        /// True if the buffer shoudl be cleared on return
        /// </summary>
        public bool ClearOnReturn { get; }
        #endregion
        #region Methods
        /// <inheritdoc />
        public byte[] Take()
        {
            byte[] buffer = _firstBuffer;

            if (buffer == null || buffer != Interlocked.CompareExchange(ref _firstBuffer, null, buffer)) buffer = ScanForAllocation();

            return buffer;
        }

        /// <summary>
        /// Gets an array from the pool or creates one if required
        /// </summary>
        /// <returns>The buffer from the pool or a newly created one if no available found</returns>
        [ExcludeFromCodeCoverage]
        private byte[] ScanForAllocation()
        {
            // Excluded from code coverage as the instance being set to null is tied to timing related checks. The rest of the code paths are covered
            var buffers = _buffers;
            byte[] instance = null;

            for (int index = 0; index < buffers.Length && instance == null; index++)
            {
                instance = buffers[index];
                if (instance != null && (instance != Interlocked.CompareExchange(ref buffers[index], null, instance))) instance = null;
            }

            return instance ?? new byte[BufferSize];
        }

        /// <inheritdoc />
        public bool Return(byte[] buffer)
        {
            Guard.NotNull(nameof(buffer), buffer);

            var success = false;

            if (ClearOnReturn)
            {
                Array.Clear(buffer, 0, buffer.Length);
            }

            if (_firstBuffer == null)
            {
                if (Interlocked.CompareExchange(ref _firstBuffer, buffer, null) == null)
                {
                    success = true;
                }
            }

            return success ? success : ScanForFree(buffer);
        }

        /// <summary>
        /// Iterates through the buffer list to find a slot to return it to
        /// </summary>
        /// <param name="buffer">The buffer to add back into the pool</param>
        /// <returns>True if it was successfully put into a slot</returns>
        private bool ScanForFree(byte[] buffer)
        {           
            var success = false;
            byte[][] buffers = _buffers;

            for (int index = 0; index < buffers.Length && !success; index++)
            {
                if (buffers[index] == null)
                {
                    if (Interlocked.CompareExchange(ref buffers[index], buffer, null) == null)
                    {
                        success = true;
                    }
                }
            }

            return success;
        }
        #endregion
    }
}
