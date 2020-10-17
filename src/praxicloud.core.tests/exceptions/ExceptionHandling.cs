// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.exceptions
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
    using praxicloud.core.exceptions;
    #endregion

    /// <summary>
    /// Tests the domain and task unhandled exception items
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExceptionHandling
    {
        /// <summary>
        /// Validates the use of the unhandled task
        /// </summary>
        [TestMethod]
        public void TestDomainUnhandledTask()
        {
            var taskCompletion = new TaskCompletionSource<bool>();
            UnobservedType observedType = UnobservedType.TaskScheduler;

            var handlers = new UnobservedHandlers((object sender, Exception exception, bool isTerminating, UnobservedType sourceType) => 
            {
                observedType = sourceType;
                taskCompletion.SetResult(true);

                return true; 
            });

            Task.Factory.StartNew(() =>
            {
                Task.Delay(100).GetAwaiter().GetResult();
                handlers.TestDomainUnhandled(this, false);
            });

            var completedTask = Task.WhenAny(taskCompletion.Task, Task.Delay(5000)).GetAwaiter().GetResult();

            Assert.IsTrue(completedTask.Id == taskCompletion.Task.Id, "The task id was not expected");
            Assert.IsTrue(observedType == UnobservedType.AppDomain, "Triggered type was not expected");
        }

        /// <summary>
        /// Validates the use of the unhandled task
        /// </summary>
        [TestMethod]
        public void TestTaskUnhandled()
        {
            var taskCompletion = new TaskCompletionSource<bool>();
            UnobservedType observedType = UnobservedType.AppDomain;

            var handlers = new UnobservedHandlers((object sender, Exception exception, bool isTerminating, UnobservedType sourceType) =>
            {
                observedType = sourceType;
                taskCompletion.SetResult(true);

                return true;
            });

            Task.Factory.StartNew(() =>
            {
                Task.Delay(100).GetAwaiter().GetResult();
                handlers.TestTaskUnhandled(this);
            });

            var completedTask = Task.WhenAny(taskCompletion.Task, Task.Delay(5000)).GetAwaiter().GetResult();

            Assert.IsTrue(completedTask.Id == taskCompletion.Task.Id, "The task id was not expected");
            Assert.IsTrue(observedType == UnobservedType.TaskScheduler, "Triggered type was not expected");
        }
    }
}
