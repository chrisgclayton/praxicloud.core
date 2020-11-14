// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.exceptions
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using praxicloud.core.exceptions.extensions;
    using System.Data;
    using System.Threading;
    using System.Runtime.InteropServices;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Tests the domain and task unhandled exception items
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExtensionTests
    {
        /// <summary>
        /// Confirm the flattening of an aggregate root exception
        /// </summary>
        [TestMethod]
        public void AggregateTest()
        {
            var innerExceptions = new Exception[] { new ApplicationException("exception2"), new ApplicationException("exception3") };
            var root = new AggregateException("exception1", innerExceptions);
            var flattened = root.FlattenToString();

            Assert.IsTrue(string.Equals(flattened, $"exception1 (exception2) (exception3){Environment.NewLine}{Environment.NewLine}exception2{Environment.NewLine}exception3", StringComparison.Ordinal), "Flattened not as expected");
        }

        /// <summary>
        /// Confirm the flattening of an exception root exception
        /// </summary>
        [TestMethod]
        public void Aggregate2Test()
        {
            var innerExceptions = new Exception[] { new ApplicationException("exception2"), new ApplicationException("exception3"), new AggregateException("exception4", new Exception[] { new ApplicationException("exception5"), new ApplicationException("exception6") }) };
            var root = new AggregateException("exception1", innerExceptions);
            var flattened = root.FlattenToString();

            Assert.IsTrue(string.Equals(flattened, $"exception1 (exception2) (exception3) (exception4 (exception5) (exception6)){Environment.NewLine}{Environment.NewLine}exception2{Environment.NewLine}exception3{Environment.NewLine}exception4 (exception5) (exception6){Environment.NewLine}{Environment.NewLine}exception5{Environment.NewLine}exception6", StringComparison.Ordinal), "Flattened not as expected");
        }

        /// <summary>
        /// Confirm the flattening of an aggregate root exception
        /// </summary>
        [TestMethod]
        public void ExceptionTest()
        {
            var innerExceptions = new Exception[] { new ApplicationException("exception3"), new ApplicationException("exception4") };
            var innerException = new AggregateException("exception2", innerExceptions);
            var root = new ApplicationException("exception1", innerException);
            var flattened = root.FlattenToString();

            Assert.IsTrue(string.Equals(flattened, $"exception1{Environment.NewLine}exception2 (exception3) (exception4){Environment.NewLine}{Environment.NewLine}exception3{Environment.NewLine}exception4", StringComparison.Ordinal), "Flattened not as expected");
        }

        /// <summary>
        /// Confirm the flattening of an exception root exception
        /// </summary>
        [TestMethod]
        public void Exception2Test()
        {
            var root = new ApplicationException("exception1", new ApplicationException("exception2"));
            var flattened = root.FlattenToString();

            Assert.IsTrue(string.Equals(flattened, $"exception1{Environment.NewLine}exception2", StringComparison.Ordinal), "Flattened not as expected");
        }

        /// <summary>
        /// Confirm the flattening of an aggregate root exception
        /// </summary>
        [TestMethod]
        public void FatalityTest()
        {
            var nonfatal1 = new ApplicationException("exception1");
            var nonfatal2 = new InsufficientMemoryException("Oops");
            var fatal1 = new OutOfMemoryException("Oops");
            var fatal2 = new DataException();            
            var fatal3 = new AccessViolationException("Oops");
            var fatal4 = new SEHException("Oops");
            var fatal5 = new TypeInitializationException("fulltypename", fatal1);
            var fatal6 = new TargetInvocationException(fatal1);
            var fatal7 = new AggregateException("Oops", new Exception[] { fatal1, fatal2 });
            var fatal8 = new AggregateException("Oops", fatal7);

            Assert.IsFalse(((Exception)null).IsFatal(), "Null should be non fatal");
            Assert.IsFalse(nonfatal1.IsFatal(), "Non fatal 1 expected");
            Assert.IsFalse(nonfatal2.IsFatal(), "Non fatal 2 expected");
            Assert.IsTrue(fatal1.IsFatal(), "Fatal 1 expected");
            Assert.IsTrue(fatal2.IsFatal(), "Fatal 2 expected");
            Assert.IsTrue(fatal3.IsFatal(), "Fatal 3 expected");
            Assert.IsTrue(fatal4.IsFatal(), "Fatal 4 expected");
            Assert.IsTrue(fatal5.IsFatal(), "Fatal 5 expected");
            Assert.IsTrue(fatal6.IsFatal(), "Fatal 6 expected");
            Assert.IsTrue(fatal7.IsFatal(), "Fatal 7 expected");
            Assert.IsTrue(fatal8.IsFatal(), "Fatal 8 expected");

           
        }

    }
}



/*

        TypeInitializationException
 */