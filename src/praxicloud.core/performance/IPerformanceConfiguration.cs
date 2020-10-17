// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.performance
{
    /// <summary>
    /// Configuration details for performance optimizations of the host process
    /// </summary>
    public interface IPerformanceConfiguration
    {
        #region Properties
        /// <summary>
        /// The minimum number of worker threads in the thread pool
        /// </summary>
        int? MinimumWorkerThreads { get; }

        /// <summary>
        /// The maximum number of worker threads in the thread pool
        /// </summary>
        int? MaximumWorkerThreads { get; }

        /// <summary>
        /// The minimum number of I/O completion port threads in the thread pool
        /// </summary>
        int? MinimumIoCompletionThreads { get; }

        /// <summary>
        /// The maximum number of I/O completion port threads in the thread pool
        /// </summary>
        int? MaximumIoCompletionThreads { get; }

        /// <summary>
        /// True if the ThreadPool configuration values should be multiplied by the number of cores
        /// </summary>
        bool UseCoreMultiplier { get; }

        /// <summary>
        /// True if the NAGLE algorithm should be used for HTTP connections
        /// </summary>
        bool? UseNagle { get; }

        /// <summary>
        /// True if the Expect100Contiue should be used for the HTTP connections
        /// </summary>
        bool? Expect100Continue { get; }

        /// <summary>
        /// The default number of connections to a single HTTP host
        /// </summary>
        int? DefaultConnectionLimit { get; }
        #endregion
    }
}
