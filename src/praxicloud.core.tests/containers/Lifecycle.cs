// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.containers
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Collections.Generic;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using praxicloud.core.containers;
    using System.Threading.Tasks;
    using System.Collections.Specialized;
    #endregion

    /// <summary>
    /// Tests the lifespan type for container operations
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]   

    public class Lifecycle
    {
        /// <summary>
        /// Tests to make sure the cancellation token is triggered
        /// </summary>
        [TestMethod]
        public void Cancellation()
        {
            var count = 0;

            ContainerLifecycle.CancellationToken.Register(() => count++);
            Task.Factory.StartNew(() =>
            {
                Task.Delay(500).GetAwaiter().GetResult();

            }).ContinueWith(t =>
            {
                ContainerLifecycle.End();
            });

            try
            {
                Task.Delay(5000, ContainerLifecycle.CancellationToken).GetAwaiter().GetResult();
            }
            catch(TaskCanceledException)
            {

            }

            Assert.IsTrue(ContainerLifecycle.CancellationToken.IsCancellationRequested, "Cancellation token was not triggered");
            Assert.IsTrue(count == 1, "Count was not expected");
        }

        /// <summary>
        /// Tests to make sure the task is completed successfully
        /// </summary>
        [TestMethod]
        public void TaskDelay()
        {
            var count = 0;

            ContainerLifecycle.CancellationToken.Register(() => count++);
            Task.Factory.StartNew(() =>
            {
                Task.Delay(500).GetAwaiter().GetResult();

            }).ContinueWith(t =>
            {
                ContainerLifecycle.End();
            });

            var taskCompleted = Task.WhenAny(ContainerLifecycle.Task, Task.Delay(5000, ContainerLifecycle.CancellationToken)).GetAwaiter().GetResult();
            
            Assert.IsTrue(ContainerLifecycle.Task.IsCompleted, "Cancellation token was not triggered");
            Assert.IsTrue(taskCompleted.Id == ContainerLifecycle.Task.Id, "Task id completed as expected");
        }
    }
}
