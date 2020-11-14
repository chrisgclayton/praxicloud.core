// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.buffers
{
    /// <summary>
    /// Configuration details for the buffer pool
    /// </summary>
    public class BufferPoolConfiguration
    {
        #region Properties
        /// <summary>
        /// The number of buffers in the pool manager
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// The size of the buffers used in the pool manager
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// True if the buffers should be cleared on return
        /// </summary>
        public bool CleanOnReturn { get; set; }
        #endregion
    }
}
