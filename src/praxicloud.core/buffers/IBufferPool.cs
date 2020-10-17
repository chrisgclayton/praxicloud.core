// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.buffers
{
    /// <summary>
    /// A buffer pool used to reduce the number of array allocations and as a result resulting garbage collections
    /// </summary>
    public interface IBufferPool
    {
        #region Properties
        /// <summary>
        /// Returns the size of the buffers that the pool allocates
        /// </summary>
        int BufferSize { get; }
        #endregion
        #region Methods
        /// <summary>
        /// Returns buffer to the pool.
        /// </summary>
        /// <param name="buffer">The buffer to add back into the pool</param>
        /// <returns>True if it was successfully put into a slot</returns>
        bool Return(byte[] buffer);

        /// <summary>
        /// Gets a byte array from the pool
        /// </summary>
        /// <returns>A byte array from the pool or creating if required</returns>
        byte[] Take();
        #endregion
    }
}