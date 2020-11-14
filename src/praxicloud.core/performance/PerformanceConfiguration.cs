// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.performance
{
    /// <summary>
    /// Configuration details for performance optimizations of the host process
    /// </summary>
    public class PerformanceConfiguration : IPerformanceConfiguration
    {
        #region Properties
        /// <inheritdoc />
        public int? MinimumWorkerThreads { get; set; }

        /// <inheritdoc />
        public int? MaximumWorkerThreads { get; set; }

        /// <inheritdoc />
        public int? MinimumIoCompletionThreads { get; set; }

        /// <inheritdoc />
        public int? MaximumIoCompletionThreads { get; set; }

        /// <inheritdoc />
        public bool UseCoreMultiplier { get; set;  }

        /// <inheritdoc />
        public bool? UseNagle { get; set; }

        /// <inheritdoc />
        public bool? Expect100Continue { get; set; }

        /// <inheritdoc />
        public int? DefaultConnectionLimit { get; set; }
        #endregion
    }
}
