// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.performance
{
    #region Using Clauses
    using System.Threading;
    using praxicloud.core.security;
    using System;
    using System.Net;
    #endregion

    /// <summary>
    /// A class used to configure performance information
    /// </summary>
    public static class PerformanceManager
    {
        #region Methods
        /// <summary>
        /// Configures the threadpool settings based on the details provided in the performance configuration
        /// </summary>
        /// <param name="configuration">The configuration instance that the ThreadPool settings are based on</param>
        public static void ConfigureThreadPool(IPerformanceConfiguration configuration)
        {
            Guard.NotNull(nameof(configuration), configuration);

            ThreadPool.GetMinThreads(out var minimumWorkerThreads, out var minimumCompletionThreads);
            ThreadPool.GetMaxThreads(out var maximumWorkerThreads, out var maximumCompletionThreads);

            if (configuration.MinimumWorkerThreads.HasValue && configuration.MinimumWorkerThreads.Value > 0) minimumWorkerThreads = configuration.UseCoreMultiplier ? Environment.ProcessorCount * configuration.MinimumWorkerThreads.Value : configuration.MinimumWorkerThreads.Value;
            if (configuration.MinimumIoCompletionThreads.HasValue && configuration.MinimumIoCompletionThreads.Value > 0) minimumCompletionThreads = configuration.UseCoreMultiplier ? Environment.ProcessorCount * configuration.MinimumIoCompletionThreads.Value : configuration.MinimumIoCompletionThreads.Value;
            if (configuration.MaximumWorkerThreads.HasValue && configuration.MaximumWorkerThreads.Value > 0) maximumWorkerThreads = configuration.UseCoreMultiplier ? Environment.ProcessorCount * configuration.MaximumWorkerThreads.Value : configuration.MaximumWorkerThreads.Value;
            if (configuration.MaximumIoCompletionThreads.HasValue && configuration.MaximumIoCompletionThreads.Value > 0) maximumCompletionThreads = configuration.UseCoreMultiplier ? Environment.ProcessorCount * configuration.MaximumIoCompletionThreads.Value : configuration.MaximumIoCompletionThreads.Value;

            ThreadPool.SetMinThreads(minimumWorkerThreads, minimumCompletionThreads);
            ThreadPool.SetMaxThreads(maximumWorkerThreads, maximumCompletionThreads);
        }

        /// <summary>
        /// Configures the http settings based on the details provided in the performance configuration
        /// </summary>
        /// <param name="configuration">The configuration instance that the http settings are based on</param>
        public static void ConfigureHttp(IPerformanceConfiguration configuration)
        {
            Guard.NotNull(nameof(configuration), configuration);

            if(configuration.UseNagle.HasValue) ServicePointManager.UseNagleAlgorithm = configuration.UseNagle.Value;
            if (configuration.Expect100Continue.HasValue) ServicePointManager.Expect100Continue = configuration.Expect100Continue.Value;
            if (configuration.DefaultConnectionLimit.HasValue) ServicePointManager.DefaultConnectionLimit = configuration.DefaultConnectionLimit.Value;
        }
        #endregion
    }
}
