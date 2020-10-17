// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests
{
    #region Using Clauses
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using praxicloud.core.security;
    #endregion

    /// <summary>
    /// A set of tests validating the guard methods
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class GuardTests
    {
        #region NotNull
        /// <summary>
        /// Tests the not null success path
        /// </summary>
        [TestMethod]
        public void NotNullSuccessTest()
        {
            object value1 = new object();

            Guard.NotNull("value1", value1);
        }

        /// <summary>
        /// Tests the not null failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void NotNullFailureTest()
        {
            object value1 = null;

            Guard.NotNull("value1", value1);
        }
        #endregion
        #region NotDefault
        /// <summary>
        /// Tests the not null success path
        /// </summary>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(true)]
        [DataRow("test value")]
        public void NotDefaultSuccessTest<T>(T value) where T : class
        {
            Guard.NotDefault("value", value);
        }

        /// <summary>
        /// Tests the not null success path
        /// </summary>
        [TestMethod]
        public void NotDefaultSuccessTest()
        {
            object value1 = new object();
            int value2 = 4;
            bool value3 = true;
            string value4 = "abc";
            TimeSpan value5 = TimeSpan.FromSeconds(1);

            Guard.NotDefault("value1", value1);
            Guard.NotDefault("value2", value2);
            Guard.NotDefault("value3", value3);
            Guard.NotDefault("value4", value4);
            Guard.NotDefault("value5", value5);
        }

        /// <summary>
        /// Tests the not null failure path of object type
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void NotDefaultFailureTest()
        {
            object value = null;

            Guard.NotDefault("value", value);
        }
        #endregion
        #region NotNullOrEmpty
        /// <summary>
        /// Tests the not null or empty success path
        /// </summary>
        [DataTestMethod]
        [DataRow(" ")]
        [DataRow("  ")]
        [DataRow("                    ")]
        [DataRow("a")]
        [DataRow("b")]
        [DataRow("1")]
        [DataRow("2")]
        [DataRow(";")]
        [DataRow("\\")]
        [DataRow("(")]
        [DataRow("{}")]
        [DataRow("Testing it out")]
        public void NotNullOrEmptySuccessTest(string value)
        {
            Guard.NotNullOrEmpty("value", value);
        }

        /// <summary>
        /// Tests the not null or empty failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void NotNullOrEmptyFailureNullTest()
        {
            string value = null;

            Guard.NotNullOrEmpty("value", value);
        }

        /// <summary>
        /// Tests the not null or empty failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void NotNullOrEmptyFailureEmptyTest()
        {
            string value = "";

            Guard.NotNullOrEmpty("value", value);
        }
        #endregion
        #region NotNullOrWhitespace
        /// <summary>
        /// Tests the not null or whitespace success path
        /// </summary>
        [DataTestMethod]
        [DataRow("a")]
        [DataRow("b")]
        [DataRow("1")]
        [DataRow("2")]
        [DataRow(";")]
        [DataRow("\\")]
        [DataRow("(")]
        [DataRow("{}")]
        [DataRow("Testing it out")]
        public void NotNullOrWhitespaceSuccessTest(string value)
        {
            Guard.NotNullOrWhitespace("value", value);
        }

        /// <summary>
        /// Tests the not null or whitespace failure path
        /// </summary>
        [DataTestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("  ")]
        [DataRow("          ")]
        [DataRow("         ")]
        [DataRow("                           ")]
        [DataRow(null)]
        [DataRow(default(string))]
        [ExpectedException(typeof(GuardException))]
        public void NotNullOrWhitespaceFailureTest(string value)
        {
            Guard.NotNullOrWhitespace("value", value);
        }
        #endregion
        #region String Length in Range
        /// <summary>
        /// Tests the not null or string length success path
        /// </summary>
        [DataTestMethod]
        [DataRow("", -1, 2)]
        [DataRow("", -1, 0)]
        [DataRow("a", -1, 2)]
        [DataRow("a", 1, 2)]
        [DataRow("a", 0, 1)]
        [DataRow("a", 0, 2)]
        [DataRow(" ", -1, 2)]
        [DataRow(" ", 1, 2)]
        [DataRow(" ", 0, 1)]
        [DataRow(" ", 0, 2)]
        [DataRow("a ", -1, 3)]
        [DataRow("a ", 1, 3)]
        [DataRow("a ", 0, 2)]
        [DataRow("a ", 0, 3)]
        [DataRow(" a", -1, 3)]
        [DataRow(" a", 1, 3)]
        [DataRow(" a", 0, 2)]
        [DataRow(" a", 0, 3)]
        [DataRow(" a ", -1, 4)]
        [DataRow(" a ", 1, 4)]
        [DataRow(" a ", 0, 4)]
        [DataRow(" a ", 0, 5)]
        [DataRow(" a ", 3, 3)]
        [DataRow(" a ", 2, 3)]
        [DataRow(" a ", 1, 3)]
        [DataRow(" a ", 0, 3)]
        public void StringLengthInRangeSuccessTest(string value, int minimum, int maximum)
        {
            Guard.StringLengthInRange("value", value, minimum, maximum);
        }

        /// <summary>
        /// Tests the not null or string length failure path
        /// </summary>
        [DataTestMethod]
        [DataRow("a", -1, 0)]
        [DataRow("a", 0, 0)]
        [DataRow("a", 2, 10)]
        [DataRow(" ", -1, 0)]
        [DataRow(" ", 0, 0)]
        [DataRow(" ", 2, 10)]
        [DataRow("", 1, 2)]
        [DataRow("", 1, 5)]
        [DataRow(null, 1, 5)]
        [ExpectedException(typeof(GuardException))]
        public void StringLengthInRangeFailureTest(string value, int minimum, int maximum)
        {
            Guard.StringLengthInRange("value", value, minimum, maximum);
        }
        #endregion
        #region Directory Exists
        /// <summary>
        /// Tests the directory exists success path
        /// </summary>
        [TestMethod]
        public void DirectoryExistsSuccessTest()
        {
            Guard.DirectoryExists("value", Environment.CurrentDirectory);
        }

        /// <summary>
        /// Tests the directory exists failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void DirectoryExistsFailureTest()
        {
            Guard.DirectoryExists("value", Path.Combine(Environment.CurrentDirectory, "thisdoesnotexisttest"));
        }
        #endregion
        #region Directory Not Exists
        /// <summary>
        /// Tests the directory not exists success path
        /// </summary>

        [TestMethod]
        public void DirectoryNotExistsSuccessTest()
        {
            Guard.DirectoryNotExists("value", Path.Combine(Environment.CurrentDirectory, "thisdoesnotexisttest"));
        }

        /// <summary>
        /// Tests the directory not exists failure path
        /// </summary>

        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void DirectoryNotExistsFailureTest()
        {
            Guard.DirectoryNotExists("value", Environment.CurrentDirectory);
        }
        #endregion
        #region File Exists
        /// <summary>
        /// Tests the file exists success path
        /// </summary>

        [TestMethod]
        public void FileExistsSuccessTest()
        {
            Guard.FileExists("value", Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Tests the file exists failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void FileExistsFailureTest()
        {
            Guard.FileExists("value", Path.Combine(Environment.CurrentDirectory, "thisdoesnotexisttest.abc"));
        }
        #endregion
        #region File Not Exists
        /// <summary>
        /// Tests the file exists success path
        /// </summary>

        [TestMethod]
        public void FileNotExistsSuccessTest()
        {
            Guard.FileNotExists("value", Path.Combine(Environment.CurrentDirectory, "thisdoesnotexisttest.abc"));
        }

        /// <summary>
        /// Tests the file not exists failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void FileNotExistsFailureTest()
        {
            Guard.FileNotExists("value", Assembly.GetExecutingAssembly().Location);
        }
        #endregion
        #region Not Less Than
        /// <summary>
        /// Tests the not less than success path
        /// </summary>
        /// <param name="millisecondsValue">The milliseconds of the value</param>
        /// <param name="millsecondsBoundary">The milliseconds of the boundary</param>
        [DataTestMethod]
        [DataRow(-1, -1)]
        [DataRow(0, -1)]
        [DataRow(1, -1)]
        [DataRow(100, -1)]
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(100, 0)]
        [DataRow(1, 1)]
        [DataRow(100, 1)]
        [DataRow(10, 10)]
        [DataRow(11, 10)]
        [DataRow(20, 10)]
        [DataRow(9999, 10)]
        [DataRow(-10, -10)]
        [DataRow(-9, -10)]
        [DataRow(0, -10)]
        [DataRow(1, -10)]
        public void NotLessThanTimeSpanSuccessTest(long millisecondsValue, long millsecondsBoundary)
        {
            var value = TimeSpan.FromMilliseconds(millisecondsValue);
            var boundary = TimeSpan.FromMilliseconds(millsecondsBoundary);

            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than failure path
        /// </summary>
        /// <param name="millisecondsValue">The milliseconds of the value</param>
        /// <param name="millsecondsBoundary">The milliseconds of the boundary</param>
        [DataTestMethod]
        [DataRow(-10, -1)]
        [DataRow(-2, -1)]
        [DataRow(-1, 0)]
        [DataRow(-1, 1)]
        [DataRow(0, 1)]
        [DataRow(-1, 10)]
        [DataRow(0, 10)]
        [DataRow(9, 10)]
        [ExpectedException(typeof(GuardException))]
        public void NotLessThanTimeSpanFailureTest(long millisecondsValue, long millsecondsBoundary)
        {
            var value = TimeSpan.FromMilliseconds(millisecondsValue);
            var boundary = TimeSpan.FromMilliseconds(millsecondsBoundary);

            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than success path
        /// </summary>
        /// <param name="millisecondsValue">The milliseconds of the value</param>
        /// <param name="millsecondsBoundary">The milliseconds of the boundary</param>
        [DataTestMethod]
        [DataRow(-1, -1)]
        [DataRow(0, -1)]
        [DataRow(1, -1)]
        [DataRow(100, -1)]
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(100, 0)]
        [DataRow(1, 1)]
        [DataRow(100, 1)]
        [DataRow(10, 10)]
        [DataRow(11, 10)]
        [DataRow(20, 10)]
        [DataRow(9999, 10)]
        [DataRow(-10, -10)]
        [DataRow(-9, -10)]
        [DataRow(0, -10)]
        [DataRow(1, -10)]
        public void NotLessThanDateTimeSuccessTest(long millisecondsValue, long millsecondsBoundary)
        {
            var value = DateTime.UnixEpoch.AddMilliseconds(millisecondsValue);
            var boundary = DateTime.UnixEpoch.AddMilliseconds(millsecondsBoundary);

            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than failure path
        /// </summary>
        /// <param name="millisecondsValue">The milliseconds of the value</param>
        /// <param name="millsecondsBoundary">The milliseconds of the boundary</param>
        [DataTestMethod]
        [DataRow(-10, -1)]
        [DataRow(-2, -1)]
        [DataRow(-1, 0)]
        [DataRow(-1, 1)]
        [DataRow(0, 1)]
        [DataRow(-1, 10)]
        [DataRow(0, 10)]
        [DataRow(9, 10)]
        [ExpectedException(typeof(GuardException))]
        public void NotLessThanDateTimeFailureTest(long millisecondsValue, long millsecondsBoundary)
        {
            var value = DateTime.UnixEpoch.AddMilliseconds(millisecondsValue);
            var boundary = DateTime.UnixEpoch.AddMilliseconds(millsecondsBoundary);

            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than success path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-1, -1)]
        [DataRow(0, -1)]
        [DataRow(1, -1)]
        [DataRow(100, -1)]
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(100, 0)]
        [DataRow(1, 1)]
        [DataRow(100, 1)]
        [DataRow(10, 10)]
        [DataRow(11, 10)]
        [DataRow(20, 10)]
        [DataRow(9999, 10)]
        [DataRow(-10, -10)]
        [DataRow(-9, -10)]
        [DataRow(0, -10)]
        [DataRow(1, -10)]
        public void NotLessThanLongSuccessTest(long value, long boundary)
        {
            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than failure path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-10, -1)]
        [DataRow(-2, -1)]
        [DataRow(-1, 0)]
        [DataRow(-1, 1)]
        [DataRow(0, 1)]
        [DataRow(-1, 10)]
        [DataRow(0, 10)]
        [DataRow(9, 10)]
        [ExpectedException(typeof(GuardException))]
        public void NotLessThanLongFailureTest(long value, long boundary)
        {
            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than success path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-1.0, -1.0)]
        [DataRow(0.0, -1.0)]
        [DataRow(1.0, -1.0)]
        [DataRow(100.0, -1.0)]
        [DataRow(0.0, 0.0)]
        [DataRow(1.0, 0.0)]
        [DataRow(100.0, 0.0)]
        [DataRow(1.0, 1.0)]
        [DataRow(100.0, 1.0)]
        [DataRow(10.0, 10.0)]
        [DataRow(11.0, 10.0)]
        [DataRow(20.0, 10.0)]
        [DataRow(9999.0, 10.0)]
        [DataRow(-10.0, -10.0)]
        [DataRow(-9.0, -10.0)]
        [DataRow(0.0, -10.0)]
        [DataRow(1.0, -10.0)]
        [DataRow(1.1, 1.0)]
        [DataRow(1.001, 1.0)]
        [DataRow(1.000001, 1.0)]
        public void NotLessThanDoubleSuccessTest(double value, double boundary)
        {
            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than failure path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-10.0, -1.0)]
        [DataRow(-2.0, -1.0)]
        [DataRow(-1.0, 0.0)]
        [DataRow(-1.0, 1.0)]
        [DataRow(0.0, 1.0)]
        [DataRow(-1.0, 10.0)]
        [DataRow(0.0, 10.0)]
        [DataRow(9.0, 10.0)]
        [DataRow(0.0, 10.0)]
        [DataRow(9.9, 10.0)]
        [DataRow(9.99, 10.0)]
        [DataRow(9.99999, 10.0)]
        [ExpectedException(typeof(GuardException))]
        public void NotLessThanDoubleFailureTest(double value, double boundary)
        {
            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than success path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(0UL, 0UL)]
        [DataRow(1UL, 0UL)]
        [DataRow(100UL, 0UL)]
        [DataRow(1UL, 1UL)]
        [DataRow(100UL, 1UL)]
        [DataRow(10UL, 10UL)]
        [DataRow(11UL, 10UL)]
        [DataRow(20UL, 10UL)]
        [DataRow(9999UL, 10UL)]
        public void NotLessThanUnsignedLongSuccessTest(ulong value, ulong boundary)
        {
            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than failure path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(0UL, 1UL)]
        [DataRow(0UL, 10UL)]
        [DataRow(9UL, 10UL)]
        [ExpectedException(typeof(GuardException))]
        public void NotLessThanUnsignedLongFailureTest(ulong value, ulong boundary)
        {
            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than success path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-1.0F, -1.0F)]
        [DataRow(0.0F, -1.0F)]
        [DataRow(1.0F, -1.0F)]
        [DataRow(100.0F, -1.0F)]
        [DataRow(0.0F, 0.0F)]
        [DataRow(1.0F, 0.0F)]
        [DataRow(100.0F, 0.0F)]
        [DataRow(1.0F, 1.0F)]
        [DataRow(100.0F, 1.0F)]
        [DataRow(10.0F, 10.0F)]
        [DataRow(11.0F, 10.0F)]
        [DataRow(20.0F, 10.0F)]
        [DataRow(9999.0F, 10.0F)]
        [DataRow(-10.0F, -10.0F)]
        [DataRow(-9.0F, -10.0F)]
        [DataRow(0.0F, -10.0F)]
        [DataRow(1.0F, -10.0F)]
        [DataRow(1.1F, 1.0F)]
        [DataRow(1.001F, 1.0F)]
        [DataRow(1.000001F, 1.0F)]
        public void NotLessThanFloatSuccessTest(float value, float boundary)
        {
            Guard.NotLessThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not less than failure path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-10.0F, -1.0F)]
        [DataRow(-2.0F, -1.0F)]
        [DataRow(-1.0F, 0.0F)]
        [DataRow(-1.0F, 1.0F)]
        [DataRow(0.0F, 1.0F)]
        [DataRow(-1.0F, 10.0F)]
        [DataRow(0.0F, 10.0F)]
        [DataRow(9.0F, 10.0F)]
        [DataRow(0.0F, 10.0F)]
        [DataRow(9.9F, 10.0F)]
        [DataRow(9.99F, 10.0F)]
        [DataRow(9.99999F, 10.0F)]
        [ExpectedException(typeof(GuardException))]
        public void NotLessThanFloatFailureTest(float value, float boundary)
        {
            Guard.NotLessThan("value", value, boundary);
        }
        #endregion
        #region Not More Than
        /// <summary>
        /// Tests the not more than success path
        /// </summary>
        /// <param name="millisecondsValue">The milliseconds of the value</param>
        /// <param name="millsecondsBoundary">The milliseconds of the boundary</param>
        [DataTestMethod]
        [DataRow(-1, -1)]
        [DataRow(-2, -1)]
        [DataRow(-3, -1)]
        [DataRow(-100, -1)]
        [DataRow(0, 0)]
        [DataRow(-1, 0)]
        [DataRow(-100, 0)]
        [DataRow(-1, -1)]
        [DataRow(-100, -1)]
        [DataRow(10, 10)]
        [DataRow(9, 10)]
        [DataRow(10, 20)]
        [DataRow(1, 10)]
        [DataRow(10, 10)]
        [DataRow(9, 10)]
        [DataRow(0, 10)]
        [DataRow(1, 10)]
        public void NotMoreThanTimeSpanSuccessTest(long millisecondsValue, long millsecondsBoundary)
        {
            var value = TimeSpan.FromMilliseconds(millisecondsValue);
            var boundary = TimeSpan.FromMilliseconds(millsecondsBoundary);

            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than failure path
        /// </summary>
        /// <param name="millisecondsValue">The milliseconds of the value</param>
        /// <param name="millsecondsBoundary">The milliseconds of the boundary</param>
        [DataTestMethod]
        [DataRow(10, 1)]
        [DataRow(2, -1)]
        [DataRow(1, 0)]
        [DataRow(1, -1)]
        [DataRow(0, -1)]
        [DataRow(1, -10)]
        [DataRow(0, -10)]
        [DataRow(-9, -10)]
        [ExpectedException(typeof(GuardException))]
        public void NotMoreThanTimeSpanFailureTest(long millisecondsValue, long millsecondsBoundary)
        {
            var value = TimeSpan.FromMilliseconds(millisecondsValue);
            var boundary = TimeSpan.FromMilliseconds(millsecondsBoundary);

            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than success path
        /// </summary>
        /// <param name="millisecondsValue">The milliseconds of the value</param>
        /// <param name="millsecondsBoundary">The milliseconds of the boundary</param>
        [DataTestMethod]
        [DataRow(-1, -1)]
        [DataRow(-2, -1)]
        [DataRow(-3, -1)]
        [DataRow(-100, -1)]
        [DataRow(0, 0)]
        [DataRow(-1, 0)]
        [DataRow(-100, 0)]
        [DataRow(-1, -1)]
        [DataRow(-100, -1)]
        [DataRow(10, 10)]
        [DataRow(9, 10)]
        [DataRow(10, 20)]
        [DataRow(1, 10)]
        [DataRow(10, 10)]
        [DataRow(9, 10)]
        [DataRow(0, 10)]
        [DataRow(1, 10)]
        public void NotMoreThanDateTimeSuccessTest(long millisecondsValue, long millsecondsBoundary)
        {
            var value = DateTime.UnixEpoch.AddMilliseconds(millisecondsValue);
            var boundary = DateTime.UnixEpoch.AddMilliseconds(millsecondsBoundary);

            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than failure path
        /// </summary>
        /// <param name="millisecondsValue">The milliseconds of the value</param>
        /// <param name="millsecondsBoundary">The milliseconds of the boundary</param>
        [DataTestMethod]
        [DataRow(10, 1)]
        [DataRow(2, -1)]
        [DataRow(1, 0)]
        [DataRow(1, -1)]
        [DataRow(0, -1)]
        [DataRow(1, -10)]
        [DataRow(0, -10)]
        [DataRow(-9, -10)]
        [ExpectedException(typeof(GuardException))]
        public void NotMoreThanDateTimeFailureTest(long millisecondsValue, long millsecondsBoundary)
        {
            var value = DateTime.UnixEpoch.AddMilliseconds(millisecondsValue);
            var boundary = DateTime.UnixEpoch.AddMilliseconds(millsecondsBoundary);

            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than success path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-1, -1)]
        [DataRow(-2, -1)]
        [DataRow(-3, -1)]
        [DataRow(-100, -1)]
        [DataRow(0, 0)]
        [DataRow(-1, 0)]
        [DataRow(-100, 0)]
        [DataRow(1, 1)]
        [DataRow(-100, 1)]
        [DataRow(10, 10)]
        [DataRow(9, 10)]
        [DataRow(0, 10)]
        [DataRow(-999, 10)]
        [DataRow(-10, -10)]
        [DataRow(-90, -10)]
        [DataRow(-100, -10)]
        [DataRow(-11, -10)]
        public void NotMoreThanLongSuccessTest(long value, long boundary)
        {
            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than failure path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(10, -1)]
        [DataRow(2, -1)]
        [DataRow(1, 0)]
        [DataRow(2, 1)]
        [DataRow(10, 1)]
        [DataRow(-1, -10)]
        [DataRow(0, -10)]
        [DataRow(-9, -10)]
        [ExpectedException(typeof(GuardException))]
        public void NotMoreThanLongFailureTest(long value, long boundary)
        {
            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than success path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-1.0, -1.0)]
        [DataRow(-2.0, -1.0)]
        [DataRow(-2.2, -1.0)]
        [DataRow(-100.0, -1.0)]
        [DataRow(0.0, 0.0)]
        [DataRow(-1.0, 0.0)]
        [DataRow(-100.0, 0.0)]
        [DataRow(1.0, 1.0)]
        [DataRow(0.5, 1.0)]
        [DataRow(-100.0, 1.0)]
        [DataRow(10.0, 10.0)]
        [DataRow(9.0, 10.0)]
        [DataRow(0.0, 10.0)]
        [DataRow(9.50, 10.0)]
        [DataRow(-10.0, -10.0)]
        [DataRow(-10.5, -10.0)]
        [DataRow(-11.0, -10.0)]
        [DataRow(0.9, 1.0)]
        public void NotMoreThanDoubleSuccessTest(double value, double boundary)
        {
            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than failure path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-0.5, -1.0)]
        [DataRow(0.0, -1.0)]
        [DataRow(1.0, 0.0)]
        [DataRow(10.0, 1.0)]
        [DataRow(10.5, 10.0)]
        [DataRow(10.9, 10.0)]
        [ExpectedException(typeof(GuardException))]
        public void NotMoreThanDoubleFailureTest(double value, double boundary)
        {
            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than success path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(0UL, 0UL)]
        [DataRow(1UL, 2UL)]
        [DataRow(100UL, 101UL)]
        [DataRow(1UL, 1UL)]
        [DataRow(100UL, 200UL)]
        [DataRow(10UL, 10UL)]
        [DataRow(11UL, 12UL)]
        [DataRow(20UL, 30UL)]
        [DataRow(9999UL, 100000UL)]
        public void NotMoreThanUnsignedLongSuccessTest(ulong value, ulong boundary)
        {
            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than failure path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(2UL, 1UL)]
        [DataRow(20UL, 10UL)]
        [DataRow(100UL, 10UL)]
        [ExpectedException(typeof(GuardException))]
        public void NotMoreThanUnsignedLongFailureTest(ulong value, ulong boundary)
        {
            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than success path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-1.0F, -1.0F)]
        [DataRow(-2.0F, -1.0F)]
        [DataRow(-2.2F, -1.0F)]
        [DataRow(-100.0F, -1.0F)]
        [DataRow(0.0F, 0.0F)]
        [DataRow(-1.0F, 0.0F)]
        [DataRow(-100.0F, 0.0F)]
        [DataRow(1.0F, 1.0F)]
        [DataRow(0.5F, 1.0F)]
        [DataRow(-100.0F, 1.0F)]
        [DataRow(10.0F, 10.0F)]
        [DataRow(9.0F, 10.0F)]
        [DataRow(0.0F, 10.0F)]
        [DataRow(9.50F, 10.0F)]
        [DataRow(-10.0F, -10.0F)]
        [DataRow(-10.5F, -10.0F)]
        [DataRow(-11.0F, -10.0F)]
        [DataRow(0.9F, 1.0F)]
        public void NotMoreThanFloatSuccessTest(float value, float boundary)
        {
            Guard.NotMoreThan("value", value, boundary);
        }

        /// <summary>
        /// Tests the not more than failure path
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="boundary">The boundary</param>
        [DataTestMethod]
        [DataRow(-0.5F, -1.0F)]
        [DataRow(0.0F, -1.0F)]
        [DataRow(1.0F, 0.0F)]
        [DataRow(10.0F, 1.0F)]
        [DataRow(10.5F, 10.0F)]
        [DataRow(10.9F, 10.0F)]
        [ExpectedException(typeof(GuardException))]
        public void NotMoreThanFloatFailureTest(float value, float boundary)
        {
            Guard.NotMoreThan("value", value, boundary);
        }
        #endregion
        #region Index out of Range
        /// <summary>
        /// Tests the index in range success path
        /// </summary>
        /// <param name="count">The number of elements in the array or null for a null value</param>
        /// <param name="index">The index to check if in range</param>
        [DataTestMethod]
        [DataRow(0, -1)]
        [DataRow(0, -10)]
        [DataRow(1, -1)]
        [DataRow(1, -2)]
        [DataRow(1, 0)]
        [DataRow(2, -1)]
        [DataRow(2, 0)]
        [DataRow(2, 1)]
        [DataRow(3, -1)]
        [DataRow(3, 0)]
        [DataRow(3, 1)]
        [DataRow(3, 2)]
        [DataRow(1000, -1)]
        [DataRow(1000, 0)]
        [DataRow(1000, 1)]
        [DataRow(1000, 10)]
        [DataRow(1000, 232)]
        [DataRow(1000, 999)]
        public void IndexOutOfRangeSuccessTest(int? count, int index)
        {
            bool[] elements = count.HasValue ? new bool[count.Value] : null;

            Guard.IndexNotOutOfRange("value", elements, index);
        }

        /// <summary>
        /// Tests the index in range failure path
        /// </summary>
        /// <param name="count">The number of elements in the array or null for a null value</param>
        /// <param name="index">The index to check if in range</param>
        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(0, 1)]
        [DataRow(1, 1)]
        [DataRow(1, 2)]
        [DataRow(1, 10)]
        [DataRow(2, 2)]
        [DataRow(2, 3)]
        [DataRow(2, 10)]
        [DataRow(3, 3)]
        [DataRow(3, 4)]
        [DataRow(3, 100)]
        [DataRow(3, 1000)]
        [DataRow(1000, 1000)]
        [DataRow(1000, 1001)]
        [DataRow(1000, 2000)]
        [DataRow(null, 0)]
        [DataRow(null, 1)]
        [DataRow(null, 1000)]
        [ExpectedException(typeof(GuardException))]
        public void IndexOutOfRangeFailureTest(int? count, int index)
        {
            bool[] elements = count.HasValue ? new bool[count.Value] : null;

            Guard.IndexNotOutOfRange("value", elements, index);
        }
        #endregion
        #region Segment out of Range
        /// <summary>
        /// Tests the segment out of range success path
        /// </summary>
        /// <param name="elementCount">The number of elements in the array or null for a null value</param>
        /// <param name="offset">The index to check if in range</param>
        /// <param name="count">The number of elements in the segment</param>
        [DataTestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(1, 0, 0)]
        [DataRow(1, 0, 1)]
        [DataRow(2, 0, 0)]
        [DataRow(2, 0, 1)]
        [DataRow(2, 0, 2)]
        [DataRow(10, 0, 0)]
        [DataRow(10, 0, 1)]
        [DataRow(10, 0, 10)]
        [DataRow(1, 1, 0)]
        [DataRow(2, 1, 0)]
        [DataRow(2, 1, 1)]
        [DataRow(10, 1, 0)]
        [DataRow(10, 1, 1)]
        [DataRow(10, 1, 9)]
        [DataRow(2, 2, 0)]
        [DataRow(10, 2, 0)]
        [DataRow(10, 2, 1)]
        [DataRow(10, 2, 8)]
        public void SegmentOutOfRangeSuccessTest(int? elementCount, int offset, int count)
        {
            bool[] elements = elementCount.HasValue ? new bool[elementCount.Value] : null;

            Guard.SegmentNotOutOfRange("value", elements, offset, count);
        }

        /// <summary>
        /// Tests the segment out of range success path
        /// </summary>
        /// <param name="elementCount">The number of elements in the array or null for a null value</param>
        /// <param name="offset">The index to check if in range</param>
        /// <param name="count">The number of elements in the segment</param>
        [DataTestMethod]
        [DataRow(0, -1, 1)]
        [DataRow(0, 0, 1)]
        [DataRow(1, 0, 2)]
        [DataRow(1, 0, 10)]
        [DataRow(2, 1, 2)]
        [DataRow(2, 1, 3)]
        [DataRow(2, 2, 1)]
        [DataRow(10, 10, 1)]
        [DataRow(10, 10, 2)]
        [DataRow(10, 10, 10)]
        [DataRow(null, 0, 1)]
        [DataRow(null, 1, 0)]
        [DataRow(null, 1, 1)]
        [ExpectedException(typeof(GuardException))]
        public void SegmentOutOfRangeFailureTest(int? elementCount, int offset, int count)
        {
            bool[] elements = elementCount.HasValue ? new bool[elementCount.Value] : null;

            Guard.SegmentNotOutOfRange("value", elements, offset, count);
        }
        #endregion
        #region Element Count In Range
        /// <summary>
        /// Tests the element count in range success path
        /// </summary>
        /// <param name="elementCount">The number of elements in the array or null for a null value</param>
        /// <param name="minimum">The minimum number of elements allowed, inclusive</param>
        /// <param name="maximum">The maximum number of elements allowed, inclusive</param>
        [DataTestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 0, 1)]
        [DataRow(0, 0, 10)]
        [DataRow(1, 0, 1)]
        [DataRow(1, 0, 2)]
        [DataRow(1, 0, 10)]
        [DataRow(10, 0, 10)]
        [DataRow(10, 0, 11)]
        [DataRow(10, 0, 100)]
        [DataRow(1, 1, 1)]
        [DataRow(1, 1, 2)]
        [DataRow(1, 1, 10)]
        [DataRow(10, 1, 10)]
        [DataRow(10, 1, 11)]
        [DataRow(10, 1, 100)]
        [DataRow(5, 5, 5)]
        [DataRow(5, 5, 6)]
        [DataRow(5, 5, 10)]
        [DataRow(10, 5, 10)]
        [DataRow(10, 5, 11)]
        [DataRow(10, 5, 100)]
        public void ElementCountInRangeSuccessTest(int? elementCount, int minimum, int maximum)
        {
            bool[] elements = elementCount.HasValue ? new bool[elementCount.Value] : null;

            Guard.ElementCountInRange("value", elements, minimum, maximum);
        }

        /// <summary>
        /// Tests the element count in range failure path
        /// </summary>
        /// <param name="elementCount">The number of elements in the array or null for a null value</param>
        /// <param name="minimum">The minimum number of elements allowed, inclusive</param>
        /// <param name="maximum">The maximum number of elements allowed, inclusive</param>
        [DataTestMethod]
        [DataRow(0, 1, 1)]
        [DataRow(0, 1, 2)]
        [DataRow(0, 1, 10)]
        [DataRow(1, 2, 2)]
        [DataRow(1, 2, 3)]
        [DataRow(1, 2, 10)]
        [DataRow(10, 11, 11)]
        [DataRow(10, 11, 12)]
        [DataRow(10, 11, 100)]
        [DataRow(1, 2, 2)]
        [DataRow(1, 2, 3)]
        [DataRow(1, 2, 10)]
        [DataRow(10, 11, 11)]
        [DataRow(10, 11, 12)]
        [DataRow(10, 11, 100)]
        [DataRow(5, 6, 6)]
        [DataRow(5, 6, 7)]
        [DataRow(5, 6, 10)]
        [DataRow(10, 0, 0)]
        [DataRow(10, 0, 1)]
        [DataRow(10, 3, 4)]
        [ExpectedException(typeof(GuardException))]
        public void ElementCountInRangeFailureTest(int? elementCount, int minimum, int maximum)
        {
            bool[] elements = elementCount.HasValue ? new bool[elementCount.Value] : null;

            Guard.ElementCountInRange("value", elements, minimum, maximum);
        }
        #endregion
        #region Element Count Atleast
        /// <summary>
        /// Tests the element count atleast success path
        /// </summary>
        /// <param name="elementCount">The number of elements in the array or null for a null value</param>
        /// <param name="minimum">The minimum number of elements allowed, inclusive</param>
        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(1, 1)]
        [DataRow(10, 0)]
        [DataRow(10, 1)]
        [DataRow(10, 10)]
        [DataRow(10, 1)]
        [DataRow(10, 2)]
        [DataRow(10, 3)]
        [DataRow(5, 0)]
        [DataRow(5, 2)]
        [DataRow(5, 5)]
        [DataRow(10, 5)]
        [DataRow(10, 9)]
        [DataRow(10, 10)]
        public void ElementCountAtLeastSuccessTest(int? elementCount, int minimum)
        {
            bool[] elements = elementCount.HasValue ? new bool[elementCount.Value] : null;

            Guard.ElementCountAtLeast("value", elements, minimum);
        }

        /// <summary>
        /// Tests the element count atleast failure path
        /// </summary>
        /// <param name="elementCount">The number of elements in the array or null for a null value</param>
        /// <param name="minimum">The minimum number of elements allowed, inclusive</param>
        [DataTestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 2)]
        [DataRow(10, 11)]
        [DataRow(5, 6)]
        [ExpectedException(typeof(GuardException))]
        public void ElementCountAtLeastFailureTest(int? elementCount, int minimum)
        {
            bool[] elements = elementCount.HasValue ? new bool[elementCount.Value] : null;

            Guard.ElementCountAtLeast("value", elements, minimum);
        }
        #endregion
        #region Collection Not Empty
        /// <summary>
        /// Tests the collection not empty success path
        /// </summary>
        /// <param name="elementCount">The number of elements to add to the collection or null to set it to null</param>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(10)]
        [DataRow(100)]
        public void CollectionNotEmptySuccessTest(int elementCount)
        {
            ICollection<int> collection = new List<int>();

            for (var index = 0; index < elementCount; index++)
            {
                ((List<int>)collection).Add(index);
            }

            Guard.CollectionNotEmpty("value", collection);
        }

        /// <summary>
        /// Tests the collection not empty failure path
        /// </summary>
        /// <param name="isNull">True if the collection is null</param>
        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        [ExpectedException(typeof(GuardException))]
        public void CollectionNotEmptyFailureTest(bool isNull)
        {
            ICollection<int> collection = isNull ? null : new List<int>();

            Guard.CollectionNotEmpty("value", collection);
        }
        #endregion
        #region IsEqual
        /// <summary>
        /// Tests the is equal success path
        /// </summary>
        /// <param name="item1">The first item to compare</param>
        /// <param name="item2">The second item to compare</param>
        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(1000, 1000)]
        public void IsEqualSuccessTest(int item1, int item2)
        {
            Guard.IsEqual("value", item1, item2);
        }

        /// <summary>
        /// Tests the is equal success path
        /// </summary>
        /// <param name="item1">The first item to compare</param>
        /// <param name="item2">The second item to compare</param>
        [DataTestMethod]
        [DataRow(0L, 0L)]
        [DataRow(1L, 1L)]
        [DataRow(2L, 2L)]
        [DataRow(1000L, 1000L)]
        public void IsEqualSuccessTest(long item1, long item2)
        {
            Guard.IsEqual("value", item1, item2);
        }

        /// <summary>
        /// Tests the is equal success path
        /// </summary>
        /// <param name="item1">The first item to compare</param>
        /// <param name="item2">The second item to compare</param>
        [DataTestMethod]
        [DataRow(1.0F, 1.0F)]
        [DataRow(2.5F, 2.5F)]
        [DataRow(-1.0F, -1.0F)]
        [DataRow(-2.5F, -2.5F)]
        [DataRow(-1000.0F, -1000.0F)]
        public void IsEqualSuccessTest(float item1, float item2)
        {
            Guard.IsEqual("value", item1, item2);
        }

        /// <summary>
        /// Tests the is equal success path
        /// </summary>
        /// <param name="item1">The first item to compare</param>
        /// <param name="item2">The second item to compare</param>
        [DataTestMethod]
        [DataRow("Hello", "Hello")]
        [DataRow("Hello it works", "Hello it works")]
        [DataRow(";):'", ";):'")]
        public void IsEqualSuccessTest(string item1, string item2)
        {
            Guard.IsEqual("value", item1, item2);
        }

        /// <summary>
        /// Tests the is equal failure path
        /// </summary>
        /// <param name="item1">The first item to compare</param>
        /// <param name="item2">The second item to compare</param>
        [DataTestMethod]
        [DataRow(0, 1)]
        [DataRow(1, -1)]
        [DataRow(2, 3)]
        [DataRow(1000, 95000)]
        [ExpectedException(typeof(GuardException))]
        public void IsEqualFailureTest(int item1, int item2)
        {
            Guard.IsEqual("value", item1, item2);
        }

        /// <summary>
        /// Tests the is equal failure path
        /// </summary>
        /// <param name="item1">The first item to compare</param>
        /// <param name="item2">The second item to compare</param>
        [DataTestMethod]
        [DataRow(0L, 1L)]
        [DataRow(1L, -1L)]
        [DataRow(2L, 3L)]
        [DataRow(1000L, 93L)]
        public void IsEqualFailureTest(long item1, long item2)
        {
            Guard.IsEqual("value", item1, item2);
        }

        /// <summary>
        /// Tests the is equal failure path
        /// </summary>
        /// <param name="item1">The first item to compare</param>
        /// <param name="item2">The second item to compare</param>
        [DataTestMethod]
        [DataRow(1.0F, -1.0F)]
        [DataRow(2.5F, 5.5F)]
        [DataRow(-1.0F, -3.0F)]
        [DataRow(-2.5F, -2.9F)]
        [DataRow(-1000.0F, -1500.0F)]
        public void IsEqualFailureTest(float item1, float item2)
        {
            Guard.IsEqual("value", item1, item2);
        }

        /// <summary>
        /// Tests the is equal failure path
        /// </summary>
        /// <param name="item1">The first item to compare</param>
        /// <param name="item2">The second item to compare</param>
        [DataTestMethod]
        [DataRow("Hello", "HellO")]
        [DataRow("Hello string works", "Hello it works")]
        [DataRow(";):'", ";:'")]
        public void IsEqualFailureTest(string item1, string item2)
        {
            Guard.IsEqual("value", item1, item2);
        }
        #endregion
        #region Matches Regex
        /// <summary>
        /// Tests the matches regex success path
        /// </summary>
        /// <param name="pattern">The regex pattern</param>
        /// <param name="value">The value to validate</param>
        [DataTestMethod]
        [DataRow(".*value.*", "Testing value match")]
        [DataRow("^Testing.*$", "Testing value match")]
        [DataRow("^Testing.*match$", "Testing value match")]
        public void MatchesRegexSuccessTest(string pattern, string value)
        {
            Guard.MatchesRegex("value", value, pattern);
        }

        /// <summary>
        /// Tests the matches regex failure path
        /// </summary>
        /// <param name="pattern">The regex pattern</param>
        /// <param name="value">The value to validate</param>
        [DataTestMethod]
        [DataRow("^value.*", "Testing value match")]
        [DataRow("^.*match.+$", "Testing value match")]
        [DataRow("^Testing.(1)match$", "Testing value match")]
        [ExpectedException(typeof(GuardException))]
        public void MatchesRegexFailureTest(string pattern, string value)
        {
            Guard.MatchesRegex("value", value, pattern);
        }
        #endregion
        #region Satifies Condition
        /// <summary>
        /// Tests the satisfies condition success path
        /// </summary>
        /// <param name="pattern">The regex pattern</param>
        /// <param name="value">The value to validate</param>
        [DataTestMethod]
        [DataRow(".*value.*", "This value matches", "this also is also a matching value", "values seem to all match")]
        [DataRow("^Testing.*$", "Testing this", "Testing that")]
        [DataRow("^Testing.*match$", "Testing this value is a match", "Testing that value is a match", "Testing the other value is a match")]
        public void SatisfyConditionSuccessTest(string pattern, params string[] values)
        {
            Guard.SatisfyCondition("value", values, item => Regex.IsMatch(item, pattern));
        }

        /// <summary>
        /// Tests the satisfies condition failure path
        /// </summary>
        /// <param name="pattern">The regex pattern</param>
        /// <param name="value">The value to validate</param>
        [DataTestMethod]
        [DataRow(".*value.*", "This value matches", "this one does not match", "values never seem to match")]
        [DataRow("^Testing.*$", "This is a Testing value", "Testing that value")]
        [DataRow("^Testing.*match$", "Testing this value is a match", "testing that value is a match", "Testing the other value is a match")]
        [ExpectedException(typeof(GuardException))]
        public void SatisfyConditionFailureTest(string pattern, params string[] values)
        {
            Guard.SatisfyCondition("value", values, item => Regex.IsMatch(item, pattern));
        }

        /// <summary>
        /// Tests the satisfies condition failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void SatisfyConditionNullFailureTest()
        {
            Guard.SatisfyCondition("value", (string[])null, item => true);
        }
        #endregion
        #region In Range
        /// <summary>
        /// Tests the in range success path
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <param name="minimum">The minimum inclusive value allowed</param>
        /// <param name="maximum">The maximum inclusive value allowed</param>
        [DataTestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, -1, 0)]
        [DataRow(0, 0, 1)]
        [DataRow(0, -100, 100)]
        [DataRow(1, 1, 1)]
        [DataRow(1, 0, 1)]
        [DataRow(1, 1, 2)]
        [DataRow(1, 0, 2)]
        [DataRow(1, -100, 100)]
        [DataRow(-1, -1, -1)]
        [DataRow(-1, -2, -1)]
        [DataRow(-1, -1, 0)]
        [DataRow(-1, -2, -0)]
        [DataRow(-1, -100, 100)]
        public void InRangeSuccessTest(int value, int minimum, int maximum)
        {
            Guard.InRange("value", value, minimum, maximum);
        }

        /// <summary>
        /// Tests the in range success path
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <param name="minimum">The minimum inclusive value allowed</param>
        /// <param name="maximum">The maximum inclusive value allowed</param>
        [DataTestMethod]
        [DataRow(0.0, 0.0, 0.0)]
        [DataRow(0.0, -1.0, 0.0)]
        [DataRow(0.0, 0.0, 1.0)]
        [DataRow(0.0, -100.0, 100.0)]
        [DataRow(1.0, 1.0, 1.0)]
        [DataRow(1.0, 0.0, 1.0)]
        [DataRow(1.0, 1.0, 2.0)]
        [DataRow(1.0, 0.0, 2.0)]
        [DataRow(1.0, -100.0, 100.0)]
        [DataRow(-1.0, -1.0, -1.0)]
        [DataRow(-1.0, -2.0, -1.0)]
        [DataRow(-1.0, -1.0, 0.0)]
        [DataRow(-1.0, -2.0, -0.0)]
        [DataRow(-1.0, -100.0, 100.0)]
        [DataRow(1.5, 1.3, 1.7)]
        [DataRow(1.9, 0.4, 2.0)]
        [DataRow(1.9, 1.9, 2.0)]
        public void InRangeSuccessTest(double value, double minimum, double maximum)
        {
            Guard.InRange("value", value, minimum, maximum);
        }

        /// <summary>
        /// Tests the in range success path
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <param name="minimum">The minimum inclusive value allowed</param>
        /// <param name="maximum">The maximum inclusive value allowed</param>
        [DataTestMethod]
        [DataRow(0.0F, 0.0F, 0.0F)]
        [DataRow(0.0F, -1.0F, 0.0F)]
        [DataRow(0.0F, 0.0F, 1.0F)]
        [DataRow(0.0F, -100.0F, 100.0F)]
        [DataRow(1.0F, 1.0F, 1.0F)]
        [DataRow(1.0F, 0.0F, 1.0F)]
        [DataRow(1.0F, 1.0F, 2.0F)]
        [DataRow(1.0F, 0.0F, 2.0F)]
        [DataRow(1.0F, -100.0F, 100.0F)]
        [DataRow(-1.0F, -1.0F, -1.0F)]
        [DataRow(-1.0F, -2.0F, -1.0F)]
        [DataRow(-1.0F, -1.0F, 0.0F)]
        [DataRow(-1.0F, -2.0F, -0.0F)]
        [DataRow(-1.0F, -100.0F, 100.0F)]
        [DataRow(1.5F, 1.3F, 1.7F)]
        [DataRow(1.9F, 0.4F, 2.0F)]
        [DataRow(1.9F, 1.9F, 2.0F)]
        public void InRangeSuccessTest(float value, float minimum, float maximum)
        {
            Guard.InRange("value", value, minimum, maximum);
        }

        /// <summary>
        /// Tests the in range success path
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <param name="minimum">The minimum inclusive value allowed</param>
        /// <param name="maximum">The maximum inclusive value allowed</param>
        [DataTestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, -1, 0)]
        [DataRow(0, 0, 1)]
        [DataRow(0, -100, 100)]
        [DataRow(1, 1, 1)]
        [DataRow(1, 0, 1)]
        [DataRow(1, 1, 2)]
        [DataRow(1, 0, 2)]
        [DataRow(1, -100, 100)]
        [DataRow(-1, -1, -1)]
        [DataRow(-1, -2, -1)]
        [DataRow(-1, -1, 0)]
        [DataRow(-1, -2, -0)]
        [DataRow(-1, -100, 100)]
        public void InRangeTimeSpanSuccessTest(int value, int minimum, int maximum)
        {
            var valueTimeSpan = TimeSpan.FromMinutes(value);
            var minimumTimeSpan = TimeSpan.FromMinutes(minimum);
            var maximumTimeSpan = TimeSpan.FromMinutes(maximum);

            Guard.InRange("value", valueTimeSpan, minimumTimeSpan, maximumTimeSpan);
        }

        /// <summary>
        /// Tests the in range failure path
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <param name="minimum">The minimum inclusive value allowed</param>
        /// <param name="maximum">The maximum inclusive value allowed</param>
        [DataTestMethod]
        [DataRow(0, 1, 2)]
        [DataRow(0, 1, 10)]
        [DataRow(0, 1, 100)]
        [DataRow(1, 2, 2)]
        [DataRow(1, 2, 10)]
        [DataRow(1, 2, 200)]
        [DataRow(1, -1, 0)]
        [DataRow(1, -100, 0)]
        [ExpectedException(typeof(GuardException))]
        public void InRangeFailureTest(int value, int minimum, int maximum)
        {
            Guard.InRange("value", value, minimum, maximum);
        }

        /// <summary>
        /// Tests the in range failure path
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <param name="minimum">The minimum inclusive value allowed</param>
        /// <param name="maximum">The maximum inclusive value allowed</param>
        [DataTestMethod]
        [DataRow(0.0, 1.0, 2.0)]
        [DataRow(0.0, 1.0, 10.0)]
        [DataRow(0.0, 1.0, 100.0)]
        [DataRow(1.0, 2.0, 2.0)]
        [DataRow(1.0, 2.0, 10.0)]
        [DataRow(1.0, 2.0, 200.0)]
        [DataRow(1.0, -1.0, 0.0)]
        [DataRow(1.0, -100.0, 0.0)]
        [DataRow(1.0, 1.2, 200.0)]
        [DataRow(1.0, -1.0, 0.9)]
        [ExpectedException(typeof(GuardException))]
        public void InRangeFailureTest(double value, double minimum, double maximum)
        {
            Guard.InRange("value", value, minimum, maximum);
        }

        /// <summary>
        /// Tests the in range success path
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <param name="minimum">The minimum inclusive value allowed</param>
        /// <param name="maximum">The maximum inclusive value allowed</param>
        [DataTestMethod]
        [DataRow(0, 1, 2)]
        [DataRow(0, 1, 10)]
        [DataRow(0, 1, 100)]
        [DataRow(1, 2, 2)]
        [DataRow(1, 2, 10)]
        [DataRow(1, 2, 200)]
        [DataRow(1, -1, 0)]
        [DataRow(1, -100, 0)]
        [ExpectedException(typeof(GuardException))]
        public void InRangeTimeSpanFailureTest(int value, int minimum, int maximum)
        {
            var valueTimeSpan = TimeSpan.FromMinutes(value);
            var minimumTimeSpan = TimeSpan.FromMinutes(minimum);
            var maximumTimeSpan = TimeSpan.FromMinutes(maximum);

            Guard.InRange("value", valueTimeSpan, minimumTimeSpan, maximumTimeSpan);
        }
        #endregion
        #region In Collection
        /// <summary>
        /// Tests the in collection success path
        /// </summary>
        /// <param name="search">The value to find in the collection</param>
        /// <param name="values">The collection to search in</param>
        [DataTestMethod]
        [DataRow("item1", "item1", "item2", "item3")]
        [DataRow("item2", "item1", "item2", "item3")]
        [DataRow("item3", "item1", "item2", "item3")]
        [DataRow("item2", "item2", "item2", "item3")]
        [DataRow("item1", "item1")]
        public void InCollectionSuccessTest(string search, params string[] values)
        {
            Guard.InCollection("value", search, values.ToList());
        }

        /// <summary>
        /// Tests the in collection success path
        /// </summary>
        /// <param name="search">The value to find in the collection</param>
        /// <param name="values">The collection to search in</param>
        [DataTestMethod]
        [DataRow(1, 1, 2, 3)]
        [DataRow(2, 1, 2, 3)]
        [DataRow(3, 1, 2, 3)]
        [DataRow(2, 2, 2, 3)]
        [DataRow(1, 1)]
        public void InCollectionSuccessTest(int search, params int[] values)
        {
            Guard.InCollection("value", search, values.ToList());
        }

        /// <summary>
        /// Tests the in collection failure path
        /// </summary>
        /// <param name="search">The value to find in the collection</param>
        /// <param name="values">The collection to search in</param>
        [DataTestMethod]
        [DataRow("item1", "item4", "item2", "item3")]
        [DataRow("item2", "item1", "item5", "item3")]
        [DataRow("item3", "item1", "item2", "item6")]
        [DataRow("item2", "item7", "item7", "item3")]
        [DataRow("item1", "item2")]
        [ExpectedException(typeof(GuardException))]
        public void InCollectionFailureTest(string search, params string[] values)
        {
            Guard.InCollection("value", search, values.ToList());
        }

        /// <summary>
        /// Tests the in collection success path
        /// </summary>
        /// <param name="search">The value to find in the collection</param>
        /// <param name="values">The collection to search in</param>
        [DataTestMethod]
        [DataRow(1, 4, 2, 3)]
        [DataRow(2, 1, 5, 3)]
        [DataRow(3, 1, 2, 7)]
        [DataRow(2, 8, 9, 3)]
        [ExpectedException(typeof(GuardException))]
        public void InCollectionFailureTest(int search, params int[] values)
        {
            Guard.InCollection("value", search, values.ToList());
        }
        #endregion
        #region In Array
        /// <summary>
        /// Tests the in array success path
        /// </summary>
        /// <param name="search">The value to find in the collection</param>
        /// <param name="values">The collection to search in</param>
        [DataTestMethod]
        [DataRow("item1", "item1", "item2", "item3")]
        [DataRow("item2", "item1", "item2", "item3")]
        [DataRow("item3", "item1", "item2", "item3")]
        [DataRow("item2", "item2", "item2", "item3")]
        [DataRow("item1", "item1")]
        public void InArraySuccessTest(string search, params string[] values)
        {
            Guard.InArray("value", search, values);
        }

        /// <summary>
        /// Tests the in array success path
        /// </summary>
        /// <param name="search">The value to find in the collection</param>
        /// <param name="values">The collection to search in</param>
        [DataTestMethod]
        [DataRow(1, 1, 2, 3)]
        [DataRow(2, 1, 2, 3)]
        [DataRow(3, 1, 2, 3)]
        [DataRow(2, 2, 2, 3)]
        [DataRow(1, 1)]
        public void InArraySuccessTest(int search, params int[] values)
        {
            Guard.InArray("value", search, values);
        }

        /// <summary>
        /// Tests the in array failure path
        /// </summary>
        /// <param name="search">The value to find in the collection</param>
        /// <param name="values">The collection to search in</param>
        [DataTestMethod]
        [DataRow("item1", "item4", "item2", "item3")]
        [DataRow("item2", "item1", "item5", "item3")]
        [DataRow("item3", "item1", "item2", "item6")]
        [DataRow("item2", "item7", "item7", "item3")]
        [DataRow("item1", "item2")]
        [ExpectedException(typeof(GuardException))]
        public void InArrayFailureTest(string search, params string[] values)
        {
            Guard.InArray("value", search, values);
        }

        /// <summary>
        /// Tests the in array success path
        /// </summary>
        /// <param name="search">The value to find in the collection</param>
        /// <param name="values">The collection to search in</param>
        [DataTestMethod]
        [DataRow(1, 4, 2, 3)]
        [DataRow(2, 1, 5, 3)]
        [DataRow(3, 1, 2, 7)]
        [DataRow(2, 8, 9, 3)]
        [ExpectedException(typeof(GuardException))]
        public void InArrayFailureTest(int search, params int[] values)
        {
            Guard.InArray("value", search, values);
        }
        #endregion
        #region Enum Value is Defined
        /// <summary>
        /// Tests the enum value defined success path
        /// </summary>
        [TestMethod]
        public void EnumValueIsDefinedSuccessTest()
        {
            Guard.EnumValueIsDefined<Enum2, int>("value", 0);
            Guard.EnumValueIsDefined<Enum3, int>("value", 0);
            Guard.EnumValueIsDefined<Enum3, int>("value", 1);
            Guard.EnumValueIsDefined<Enum4, int>("value", 0);
            Guard.EnumValueIsDefined<Enum4, int>("value", 1);
            Guard.EnumValueIsDefined<Enum4, int>("value", 2);
            Guard.EnumValueIsDefined<Enum5, int>("value", 1);
            Guard.EnumValueIsDefined<Enum6, int>("value", 1);
            Guard.EnumValueIsDefined<Enum6, int>("value", 2);
            Guard.EnumValueIsDefined<Enum7, int>("value", 1);
            Guard.EnumValueIsDefined<Enum7, int>("value", 2);
            Guard.EnumValueIsDefined<Enum7, int>("value", 3);
            Guard.EnumValueIsDefined<Enum8, byte>("value", 1);
            Guard.EnumValueIsDefined<Enum8, byte>("value", 2);
            Guard.EnumValueIsDefined<Enum8, byte>("value", 3);
        }

        /// <summary>
        /// Tests the enum value defined failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void EnumValueIsDefined1FailureTest()
        {
            Guard.EnumValueIsDefined<Enum1, int>("value", 0);
        }


        /// <summary>
        /// Tests the enum value defined failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void EnumValueIsDefined2FailureTest()
        {
            Guard.EnumValueIsDefined<Enum2, int>("value", 1);
        }


        /// <summary>
        /// Tests the enum value defined failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void EnumValueIsDefined3FailureTest()
        {
            Guard.EnumValueIsDefined<Enum3, int>("value", 2);
        }


        /// <summary>
        /// Tests the enum value defined failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void EnumValueIsDefined4FailureTest()
        {
            Guard.EnumValueIsDefined<Enum4, int>("value", 3);
        }


        /// <summary>
        /// Tests the enum value defined failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void EnumValueIsDefined5FailureTest()
        {
            Guard.EnumValueIsDefined<Enum5, int>("value", 0);
        }

        /// <summary>
        /// Tests the enum value defined failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void EnumValueIsDefined6FailureTest()
        {
            Guard.EnumValueIsDefined<Enum8, byte>("value", 0);
        }

        /// <summary>
        /// Tests the enum value defined failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void EnumValueIsDefined7FailureTest()
        {
            Guard.EnumValueIsDefined<Enum8, byte>("value", 4);
        }
        #endregion
        #region Is Assignable To
        /// <summary>
        /// Tests the is assignable to success path
        /// </summary>
        [TestMethod]
        public void IsAssignableToSuccessTest()
        {
            Guard.IsAssignableTo<object>("value", new object());
            Guard.IsAssignableTo<object>("value", new List<string>());
            Guard.IsAssignableTo<IEnumerable<string>>("value", new List<string>());

            Guard.IsAssignableTo<Mammal>("value", new Mammal());
            Guard.IsAssignableTo<Mammal>("value", new Homininea());
            Guard.IsAssignableTo<Mammal>("value", new HomoSapien());
            Guard.IsAssignableTo<Mammal>("value", new Gorilla());
            Guard.IsAssignableTo<Homininea>("value", new Homininea());
            Guard.IsAssignableTo<Homininea>("value", new HomoSapien());
            Guard.IsAssignableTo<Homininea>("value", new Gorilla());
        }

        /// <summary>
        /// Tests the is assignable to failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void IsAssignableTo1FailureTest()
        {
            Guard.IsAssignableTo<HomoSapien>("value", new Mammal());
        }

        /// <summary>
        /// Tests the is assignable to failure path
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GuardException))]
        public void IsAssignableTo2FailureTest()
        {
            Guard.IsAssignableTo<HomoSapien>("value", new Gorilla());
        }
        #endregion
        #region Enumeration Declarations
        /// <summary>
        /// An empty enum with no values
        /// </summary>
        public enum Enum1
        {

        }

        /// <summary>
        /// An enumeration with no value assigned and default data type
        /// </summary>
        public enum Enum2
        {
            value1
        }

        /// <summary>
        /// An enumeration with no value assigned and default data types
        /// </summary>
        public enum Enum3
        {
            value1,
            value2
        }

        /// <summary>
        /// An enumeration with no value assigned and default data types
        /// </summary>
        public enum Enum4
        {
            value1,
            value2,
            value3
        }

        /// <summary>
        /// An enumeration with value assigned and default data types
        /// </summary>
        public enum Enum5
        {
            value1 = 1
        }

        /// <summary>
        /// An enumeration with value assigned and default data types
        /// </summary>
        public enum Enum6
        {
            value1 = 1,
            value2 = 2
        }

        /// <summary>
        /// An enumeration with value assigned and default data types
        /// </summary>
        public enum Enum7
        {
            value1 = 1,
            value2 = 2,
            value3 = 3
        }

        /// <summary>
        /// An enumeration with value assigned and byte data types
        /// </summary>
        public enum Enum8 : byte
        {
            value1 = 1,
            value2 = 2,
            value3 = 3
        }
        #endregion
        #region Assignable Test Types
        /// <summary>
        /// Base type
        /// </summary>
        [ExcludeFromCodeCoverage]
        public class Mammal 
        {
        }

        /// <summary>
        /// Common ancestor
        /// </summary>
        [ExcludeFromCodeCoverage]
        public class Homininea : Mammal
        {

        }

        /// <summary>
        /// Man
        /// </summary>
        [ExcludeFromCodeCoverage]
        public class HomoSapien : Homininea
        {

        }

        /// <summary>
        /// Gorilla
        /// </summary>
        [ExcludeFromCodeCoverage]
        public class Gorilla : Homininea
        {

        }
        #endregion
    }
}
