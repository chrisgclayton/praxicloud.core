// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.performance
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using praxicloud.core.performance;
    using System.Threading;
    using System.Net;
    #endregion

    /// <summary>
    /// Tests the domain and task unhandled exception items
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PerformanceConfigurationTests
    {
        /// <summary>
        /// Validates the expected values are present when the performance configuration is not set
        /// </summary>
        [TestMethod]
        public void ConfigurationPersistenceNotSet()
        {
            var valuesNotSet = new PerformanceConfiguration();

            Assert.IsFalse(valuesNotSet.DefaultConnectionLimit.HasValue, "Not set default connection limit not expected");
            Assert.IsFalse(valuesNotSet.Expect100Continue.HasValue, "Not set expect 100 continue not expected");
            Assert.IsFalse(valuesNotSet.UseNagle.HasValue, "Not set use nagle not expected");
            Assert.IsFalse(valuesNotSet.MaximumIoCompletionThreads.HasValue, "Not set maximum completion not expected");
            Assert.IsFalse(valuesNotSet.MaximumWorkerThreads.HasValue, "Not set maximum worker not expected");
            Assert.IsFalse(valuesNotSet.MinimumIoCompletionThreads.HasValue, "Not set minimum completion not expected");
            Assert.IsFalse(valuesNotSet.MinimumWorkerThreads.HasValue, "Not set minimum worker not expected");
            Assert.IsFalse(valuesNotSet.UseCoreMultiplier, "Not set use core multiplier not expected");
        }

        /// <summary>
        /// Validates the expected values are present when the performance configuration is set
        /// </summary>
        [TestMethod]
        public void ConfigurationPersistenceSet()
        {
            var valuesSet = new PerformanceConfiguration
            {
                DefaultConnectionLimit = 123,
                Expect100Continue = true,
                UseNagle = true,
                UseCoreMultiplier = true,
                MinimumWorkerThreads = 234,
                MinimumIoCompletionThreads = 345,
                MaximumWorkerThreads = 456,
                MaximumIoCompletionThreads = 567
            };

            Assert.IsTrue(valuesSet.DefaultConnectionLimit.HasValue, "Not set default connection limit set not expected");
            Assert.IsTrue(valuesSet.Expect100Continue.HasValue, "Not set expect 100 continue set not expected");
            Assert.IsTrue(valuesSet.UseNagle.HasValue, "Not set use nagle set not expected");
            Assert.IsTrue(valuesSet.MaximumIoCompletionThreads.HasValue, "Not set maximum completion set not expected");
            Assert.IsTrue(valuesSet.MaximumWorkerThreads.HasValue, "Not set maximum worker set not expected");
            Assert.IsTrue(valuesSet.MinimumIoCompletionThreads.HasValue, "Not set minimum completion set not expected");
            Assert.IsTrue(valuesSet.MinimumWorkerThreads.HasValue, "Not set minimum worker set not expected");
            Assert.IsTrue(valuesSet.UseCoreMultiplier, "Not set use core multiplier not expected");
                          
            Assert.IsTrue(valuesSet.DefaultConnectionLimit.Value == 123, "Not set default connection limit not expected");
            Assert.IsTrue(valuesSet.Expect100Continue.Value, "Not set expect 100 continue not expected");
            Assert.IsTrue(valuesSet.UseNagle.Value, "Not set use nagle not expected");
            Assert.IsTrue(valuesSet.MaximumIoCompletionThreads.Value == 567, "Not set maximum completion not expected");
            Assert.IsTrue(valuesSet.MaximumWorkerThreads.Value == 456, "Not set maximum worker not expected");
            Assert.IsTrue(valuesSet.MinimumIoCompletionThreads.Value == 345, "Not set minimum completion not expected");
            Assert.IsTrue(valuesSet.MinimumWorkerThreads.Value == 234, "Not set minimum worker not expected");
        }

        /// <summary>
        /// Validates the expected values of the thread pool are set properly without the multiplier enabled
        /// </summary>
        [TestMethod]
        public void ConfigureThreadPoolWithoutMultiplier()
        {
            var valuesSet = new PerformanceConfiguration
            {
                UseCoreMultiplier = false,
                MinimumWorkerThreads = 40,
                MinimumIoCompletionThreads = 20,
                MaximumWorkerThreads = 80,
                MaximumIoCompletionThreads = 60
            };

            PerformanceManager.ConfigureThreadPool(valuesSet);

            ThreadPool.GetMinThreads(out var minimumWorkerThreads, out var minimumCompletionThreads);
            ThreadPool.GetMaxThreads(out var maximumWorkerThreads, out var maximumCompletionThreads);

            Assert.IsTrue(valuesSet.MaximumIoCompletionThreads.Value == maximumCompletionThreads, "Not set maximum completion not expected");
            Assert.IsTrue(valuesSet.MaximumWorkerThreads.Value == maximumWorkerThreads, "Not set maximum worker not expected");
            Assert.IsTrue(valuesSet.MinimumIoCompletionThreads.Value == minimumCompletionThreads, "Not set minimum completion not expected");
            Assert.IsTrue(valuesSet.MinimumWorkerThreads.Value == minimumWorkerThreads, "Not set minimum worker not expected");
        }

        /// <summary>
        /// Validates the expected values of the thread pool are set properly with the multiplier enabled
        /// </summary>
        [TestMethod]
        public void ConfigureThreadPoolWitMultiplier()
        {
            var valuesSet = new PerformanceConfiguration
            {
                UseCoreMultiplier = true,
                MinimumWorkerThreads = 40,
                MinimumIoCompletionThreads = 20,
                MaximumWorkerThreads = 80,
                MaximumIoCompletionThreads = 60
            };

            PerformanceManager.ConfigureThreadPool(valuesSet);

            ThreadPool.GetMinThreads(out var minimumWorkerThreads, out var minimumCompletionThreads);
            ThreadPool.GetMaxThreads(out var maximumWorkerThreads, out var maximumCompletionThreads);

            Assert.IsTrue((valuesSet.MaximumIoCompletionThreads.Value * Environment.ProcessorCount) == maximumCompletionThreads, "Not set maximum completion not expected");
            Assert.IsTrue((valuesSet.MaximumWorkerThreads.Value * Environment.ProcessorCount) == maximumWorkerThreads, "Not set maximum worker not expected");
            Assert.IsTrue((valuesSet.MinimumIoCompletionThreads.Value * Environment.ProcessorCount) == minimumCompletionThreads, "Not set minimum completion not expected");
            Assert.IsTrue((valuesSet.MinimumWorkerThreads.Value * Environment.ProcessorCount) == minimumWorkerThreads, "Not set minimum worker not expected");
        }

        /// <summary>
        /// Validates the expected values of the http values are set properly with the multiplier enabled
        /// </summary>
        [TestMethod]
        public void ConfigureHttp()
        {
            var valuesFalse = new PerformanceConfiguration
            {
                DefaultConnectionLimit = 26,
                UseNagle = false,
                Expect100Continue = false
            };
            var valuesTrue = new PerformanceConfiguration
            {
                DefaultConnectionLimit = 37,
                UseNagle = true,
                Expect100Continue = true
            };


            PerformanceManager.ConfigureHttp(valuesFalse);

            var falseConnectionLimit = ServicePointManager.DefaultConnectionLimit;
            var falseUseNagle = ServicePointManager.UseNagleAlgorithm;
            var falseExpect100 = ServicePointManager.Expect100Continue;

            PerformanceManager.ConfigureHttp(valuesTrue);

            var trueConnectionLimit = ServicePointManager.DefaultConnectionLimit;
            var trueUseNagle = ServicePointManager.UseNagleAlgorithm;
            var trueExpect100 = ServicePointManager.Expect100Continue;

            Assert.IsFalse(falseUseNagle, "False use nagle not expected");
            Assert.IsFalse(falseExpect100, "False not expected not expected");
            Assert.IsTrue(falseConnectionLimit == 26, "False connection limit not expected");

            Assert.IsTrue(trueUseNagle, "True use nagle not expected");
            Assert.IsTrue(trueExpect100, "True not expected not expected");
            Assert.IsTrue(trueConnectionLimit == 37, "True connection limit not expected");
        }
    }
}
