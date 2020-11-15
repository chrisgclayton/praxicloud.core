// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.configuration
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using praxicloud.core.configuration;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    #endregion

    /// <summary>
    /// Tests to validate configuration utility functionality
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ConfigurationUtilityTests
    {
        #region Dictionary Retrieval
        /// <summary>
        /// Retrieves boolean values from the dictionary
        /// </summary>
        [TestMethod]
        public void GetBool()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "Item1", "true" },
                { "Item2", "false" },
                { "Item3", "true" },
                { "Item4", "false" },
                { "Item5", "true" },
                { "Item6", "false" },
                { "Item7", "true" },
                { "Item8", "false" }
            };

            var value1 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item3", false);
            var value2 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item4", false);
            var value3 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item9", false);

            Assert.IsTrue(value1, "Item3 is not the expected value");
            Assert.IsFalse(value2, "Item4 is not the expected value");
            Assert.IsFalse(value3, "Item9 is not the expected value");
        }

        /// <summary>
        /// Retrieves integer values from the dictionary
        /// </summary>
        [TestMethod]
        public void GetInt()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "Item1", "1" },
                { "Item2", "2" },
                { "Item3", "3" },
                { "Item4", "4" },
                { "Item5", "5" },
                { "Item6", "6" },
                { "Item7", "7" },
                { "Item8", "8" }
            };

            var value1 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item3", 100);
            var value2 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item4", 100);
            var value3 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item9", 100);

            Assert.IsTrue(value1 == 3, "Item3 is not the expected value");
            Assert.IsTrue(value2 == 4, "Item4 is not the expected value");
            Assert.IsTrue(value3 == 100, "Item0 is not the expected value");
        }

        /// <summary>
        /// Retrieves double values from the dictionary
        /// </summary>
        [TestMethod]
        public void GetDouble()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "Item1", "1.1" },
                { "Item2", "2.2" },
                { "Item3", "3.3" },
                { "Item4", "4.4" },
                { "Item5", "5.5" },
                { "Item6", "6.6" },
                { "Item7", "7.7" },
                { "Item8", "8.8" }
            };

            var value1 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item3", 100.123);
            var value2 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item4", 100.123);
            var value3 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item9", 100.123);

            Assert.IsTrue(value1 == 3.3, "Item3 is not the expected value");
            Assert.IsTrue(value2 == 4.4, "Item4 is not the expected value");
            Assert.IsTrue(value3 == 100.123, "Item9 is not the expected value");
        }

        /// <summary>
        /// Retrieves string values from the dictionary
        /// </summary>
        [TestMethod]
        public void GetString()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "Item1", "1.1" },
                { "Item2", "2.2" },
                { "Item3", "3.3" },
                { "Item4", "4.4" },
                { "Item5", "5.5" },
                { "Item6", "6.6" },
                { "Item7", "7.7" },
                { "Item8", "8.8" }
            };

            var value1 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item3", "100.123");
            var value2 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item4", "100.123");
            var value3 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item9", "100.123");

            Assert.IsTrue(value1 == "3.3", "Item3 is not the expected value");
            Assert.IsTrue(value2 == "4.4", "Item4 is not the expected value");
            Assert.IsTrue(value3 == "100.123", "Item9 is not the expected value");
        }

        /// <summary>
        /// Retrieves byte values from the dictionary
        /// </summary>
        [TestMethod]
        public void GetByte()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "Item1", "1" },
                { "Item2", "2" },
                { "Item3", "3" },
                { "Item4", "4" },
                { "Item5", "5" },
                { "Item6", "6" },
                { "Item7", "7" },
                { "Item8", "8" }
            };

            var value1 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item3", (byte)100);
            var value2 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item4", (byte)100);
            var value3 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item9", (byte)100);

            Assert.IsTrue(value1 == 3, "Item3 is not the expected value");
            Assert.IsTrue(value2 == 4, "Item4 is not the expected value");
            Assert.IsTrue(value3 == 100, "Item9 is not the expected value");
        }

        /// <summary>
        /// Retrieves time span values from the dictionary
        /// </summary>
        [TestMethod]
        public void GetTimeSpan()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "Item1", TimeSpan.FromMinutes(1).ToString() },
                { "Item2", TimeSpan.FromMinutes(2).ToString() },
                { "Item3", TimeSpan.FromMinutes(3).ToString() },
                { "Item4", TimeSpan.FromMinutes(4).ToString() },
                { "Item5", TimeSpan.FromMinutes(5).ToString() },
                { "Item6", TimeSpan.FromMinutes(6).ToString() },
                { "Item7", TimeSpan.FromMinutes(7).ToString() },
                { "Item8", TimeSpan.FromMinutes(8).ToString() }
            };

            var value1 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item3", TimeSpan.FromHours(1));
            var value2 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item4", TimeSpan.FromHours(1));
            var value3 = ConfigurationUtilities.GetFromDictionary(dictionary, "Item9", TimeSpan.FromHours(1));

            Assert.IsTrue(value1 == TimeSpan.FromMinutes(3), "Item3 is not the expected value");
            Assert.IsTrue(value2 == TimeSpan.FromMinutes(4), "Item4 is not the expected value");
            Assert.IsTrue(value3 == TimeSpan.FromHours(1), "Item9 is not the expected value");
        }
        #endregion
        #region File Expansion
        /// <summary>
        /// Validates the expansion of the file name
        /// </summary>
        [DataTestMethod]
        [DataRow(true, "tools", "file.txt")]
        [DataRow(true, "tools", "file.txt", "test")]
        [DataRow(true, "tools", "file.txt", "test", "out")]
        [DataRow(false, "tools", "file.txt")]
        [DataRow(false, null, "file.txt")]
        public void ExpandFileName(bool offRoot, string directory, string fileName, params string[] pathElements)
        {
            var constructedDirectoryPathElements = new List<string>();

            if (offRoot) constructedDirectoryPathElements.Add(Path.GetPathRoot(Environment.CurrentDirectory));
            if ((pathElements?.Count() ?? 0) > 0) constructedDirectoryPathElements.AddRange(pathElements);

            var constructedDirectoryPath = Path.Combine(constructedDirectoryPathElements.ToArray());
            if (offRoot) Path.Combine(Path.GetPathRoot(Environment.CurrentDirectory), directory);


            var expectedQualifiedName = offRoot ? Path.Combine(constructedDirectoryPath, fileName) : Path.Combine(Environment.CurrentDirectory, constructedDirectoryPath, fileName);


            var expandedPath = ConfigurationUtilities.ExpandConfigFileName(fileName, constructedDirectoryPath);

            Assert.IsTrue(string.Equals(expectedQualifiedName, expandedPath, StringComparison.Ordinal), "Expanded directory path was incorrect");
        }

        /// <summary>
        /// Validates the expansion of the file name
        /// </summary>
        [TestMethod]
        public void ExpandFullyQualified()
        {
            var fileName = Path.Combine(Path.GetPathRoot(Environment.CurrentDirectory), "test", "demo.txt");
            var expandedPath = ConfigurationUtilities.ExpandConfigFileName(fileName, null);

            Assert.IsTrue(string.Equals(fileName, expandedPath, StringComparison.Ordinal), "Expanded directory path was incorrect");
        }
        #endregion
        #region Create From Non Generic
        /// <summary>
        /// A test that validates what happens when no parameters are provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleNoValues()
        {
            var configuration = ConfigurationUtilities.CreateFrom(typeof(DemoConfiguration), null, null, null, null, null);
            var typedConfiguration = configuration as DemoConfiguration;

            Assert.IsNotNull(configuration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == default, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == default, "DateTimeValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == default, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == default, "EnumerationValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == default, "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.StringValue == default, "StringValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when only JSON configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleJsonValues()
        {
            var json = GetSimpleJson();
            var configuration = ConfigurationUtilities.CreateFrom(typeof(DemoConfiguration), json, null, null, null, null);
            var typedConfiguration = configuration as DemoConfiguration;

            Assert.IsNotNull(configuration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 23, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.FromMinutes(123), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueJson", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringJson", StringComparison.Ordinal), "ObjectValue::StringValue not expected");                       
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 12:00:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when only Memory configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleMemoryValues()
        {
            var memory = GetSimpleDictionary();
            var configuration = ConfigurationUtilities.CreateFrom(typeof(DemoConfiguration), null, null, null, null, memory);
            var typedConfiguration = configuration as DemoConfiguration;

            Assert.IsNotNull(configuration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == false, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 42, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("01:02:03.4567"), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Simple, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueMemory", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringMemory", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 13:00:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when only Command Line configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleCommandLineValues()
        {
            var commandLine = GetSimpleCommandLine();
            var configuration = ConfigurationUtilities.CreateFrom(typeof(DemoConfiguration), null, null, null, commandLine, null);
            var typedConfiguration = configuration as DemoConfiguration;

            Assert.IsNotNull(configuration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 53, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("01:03:04.4567"), "TimeSpanValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueCommand", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringCommand", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 14:00:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when only root file configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleRootFileValues()
        {            
            var configuration = ConfigurationUtilities.CreateFrom(typeof(DemoConfiguration), null, "SimpleConfigurationRoot.json", null, null, null);
            var typedConfiguration = configuration as DemoConfiguration;

            Assert.IsNotNull(configuration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 88, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("17:17:17.1717"), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringFile2", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringFile2", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-10-17 17:05:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when only subdirectory file configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleSubdirectoryFileValues()
        {
            var configuration = ConfigurationUtilities.CreateFrom(typeof(DemoConfiguration), null, "SimpleConfiguration.json", "configuration", null, null);
            var typedConfiguration = configuration as DemoConfiguration;

            Assert.IsNotNull(configuration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == false, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 99, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Simple, "EnumerationValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("16:16:16.1616"), "TimeSpanValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringFile1", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringFile1", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-05 17:05:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when Memory and file based configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateMemoryAndFileValues()
        {
            var memory = GetSimpleDictionary();
            var configuration = ConfigurationUtilities.CreateFrom(typeof(DemoConfiguration), null, "SimpleConfigurationRoot.json", null, null, memory);
            var typedConfiguration = configuration as DemoConfiguration;

            Assert.IsNotNull(configuration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 88, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("17:17:17.1717"), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringFile2", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringFile2", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-10-17 17:05:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when Memory, file and command line based configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateMemoryAndFileAndCommandLineValues()
        {
            var memory = GetSimpleDictionary();
            var commandLine = GetSimpleCommandLine();
            var configuration = ConfigurationUtilities.CreateFrom(typeof(DemoConfiguration), null, "SimpleConfigurationRoot.json", null, commandLine, memory);
            var typedConfiguration = configuration as DemoConfiguration;

            Assert.IsNotNull(configuration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 53, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("01:03:04.4567"), "TimeSpanValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueCommand", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringCommand", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 14:00:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when Memory, file, command line, and JSON based configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateMemoryAndFileAndCommandLineAndJsonValues()
        {
            var memory = GetSimpleDictionary();
            var commandLine = GetSimpleCommandLine();
            var json = GetSimpleJson();
            var configuration = ConfigurationUtilities.CreateFrom(typeof(DemoConfiguration), json, "SimpleConfigurationRoot.json", null, commandLine, memory);
            var typedConfiguration = configuration as DemoConfiguration;

            Assert.IsNotNull(configuration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 23, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.FromMinutes(123), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueJson", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringJson", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 12:00:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when Memory, file, command line, and JSON based configuration with mixed values is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateMemoryAndFileAndCommandLineAndJsonMixedValues()
        {
            var memory = GetSparseDictionary();
            var commandLine = GetSparseCommandLine();
            var json = GetSparseJson();
            var configuration = ConfigurationUtilities.CreateFrom(typeof(DemoConfiguration), json, "SparseConfiguration.json", "configuration", commandLine, memory);
            var typedConfiguration = configuration as DemoConfiguration;

            Assert.IsNotNull(configuration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 72, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("16:16:16.1616"), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Simple, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueMemory", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringMemory", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 14:00:00.0000"), "DateTimeValue not expected");
        }
        #endregion
        #region Create From Generic
        /// <summary>
        /// A test that validates what happens when no parameters are provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleNoValuesGeneric()
        {
            var typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(null, null, null, null, null);

            Assert.IsNotNull(typedConfiguration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == default, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == default, "DateTimeValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == default, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == default, "EnumerationValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == default, "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.StringValue == default, "StringValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when only JSON configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleJsonValuesGeneric()
        {
            var json = GetSimpleJson();
            var typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(json, null, null, null, null);

            Assert.IsNotNull(typedConfiguration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 23, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.FromMinutes(123), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueJson", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringJson", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 12:00:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when only Memory configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleMemoryValuesGeneric()
        {
            var memory = GetSimpleDictionary();
            var typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(null, null, null, null, memory);

            Assert.IsNotNull(typedConfiguration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == false, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 42, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("01:02:03.4567"), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Simple, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueMemory", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringMemory", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 13:00:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when only Command Line configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleCommandLineValuesGeneric()
        {
            var commandLine = GetSimpleCommandLine();
            var typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(null, null, null, commandLine, null);

            Assert.IsNotNull(typedConfiguration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 53, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("01:03:04.4567"), "TimeSpanValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueCommand", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringCommand", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 14:00:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when only root file configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleRootFileValuesGeneric()
        {
            var typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(null, "SimpleConfigurationRoot.json", null, null, null);

            Assert.IsNotNull(typedConfiguration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 88, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("17:17:17.1717"), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringFile2", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringFile2", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-10-17 17:05:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when only subdirectory file configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateSimpleSubdirectoryFileValuesGeneric()
        {
            var typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(null, "SimpleConfiguration.json", "configuration", null, null);

            Assert.IsNotNull(typedConfiguration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == false, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 99, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Simple, "EnumerationValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("16:16:16.1616"), "TimeSpanValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringFile1", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringFile1", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-05 17:05:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when Memory and file based configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateMemoryAndFileValuesGeneric()
        {
            var memory = GetSimpleDictionary();
            var typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(null, "SimpleConfigurationRoot.json", null, null, memory);

            Assert.IsNotNull(typedConfiguration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 88, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("17:17:17.1717"), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringFile2", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringFile2", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-10-17 17:05:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when Memory, file and command line based configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateMemoryAndFileAndCommandLineValuesGeneric()
        {
            var memory = GetSimpleDictionary();
            var commandLine = GetSimpleCommandLine();
            var typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(null, "SimpleConfigurationRoot.json", null, commandLine, memory);

            Assert.IsNotNull(typedConfiguration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 53, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("01:03:04.4567"), "TimeSpanValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueCommand", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringCommand", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 14:00:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when Memory, file, command line, and JSON based configuration is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateMemoryAndFileAndCommandLineAndJsonValuesGeneric()
        {
            var memory = GetSimpleDictionary();
            var commandLine = GetSimpleCommandLine();
            var json = GetSimpleJson();
            var typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(json, "SimpleConfigurationRoot.json", null, commandLine, memory);

            Assert.IsNotNull(typedConfiguration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 23, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.FromMinutes(123), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Complex, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueJson", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringJson", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 12:00:00.0000"), "DateTimeValue not expected");
        }

        /// <summary>
        /// A test that validates what happens when Memory, file, command line, and JSON based configuration with mixed values is provided to create the configuration from
        /// </summary>
        [TestMethod]
        public void CreateMemoryAndFileAndCommandLineAndJsonMixedValuesGeneric()
        {
            var memory = GetSparseDictionary();
            var commandLine = GetSparseCommandLine();
            var json = GetSparseJson();
            var typedConfiguration = ConfigurationUtilities.CreateFrom<DemoConfiguration>(json, "SparseConfiguration.json", "configuration", commandLine, memory);

            Assert.IsNotNull(typedConfiguration, "Configuration unexpected value");
            Assert.IsNotNull(typedConfiguration, "Invalid type returned");
            Assert.IsNotNull(typedConfiguration.ObjectValue == default, "ObjectValue not expected");
            Assert.IsTrue(typedConfiguration.BooleanValue == true, "BooleanValue not expected");
            Assert.IsTrue(typedConfiguration.IntegerValue == 72, "IntegerValue not expected");
            Assert.IsTrue(typedConfiguration.TimeSpanValue == TimeSpan.Parse("16:16:16.1616"), "TimeSpanValue not expected");
            Assert.IsTrue(typedConfiguration.EnumerationValue == DemoType.Simple, "EnumerationValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.StringValue, "StringValueMemory", StringComparison.Ordinal), "StringValue not expected");
            Assert.IsTrue(string.Equals(typedConfiguration.ObjectValue.StringValue, "SubstringMemory", StringComparison.Ordinal), "ObjectValue::StringValue not expected");
            Assert.IsTrue(typedConfiguration.DateTimeValue == DateTime.Parse("2020-09-01 14:00:00.0000"), "DateTimeValue not expected");
        }
        #endregion
        #region Shared Methods
        /// <summary>
        /// Retrieves a simple version of the JSON string for configuring the container
        /// </summary>
        /// <returns>The JSON string</returns>
        private string GetSimpleJson()
        {
            var jsonValues = new DemoConfiguration()
            {
                BooleanValue = true,
                DateTimeValue = DateTime.Parse("2020-09-01 12:00:00.0000"),
                EnumerationValue = DemoType.Complex,
                IntegerValue = 23,
                ObjectValue = new SubConfiguration { StringValue = "SubstringJson" },
                StringValue = "StringValueJson",
                TimeSpanValue = TimeSpan.FromMinutes(123)
            };

            return JsonConvert.SerializeObject(jsonValues);
        }

        /// <summary>
        /// Retrieves a simple version of the in memory dictionary for configuring the container
        /// </summary>
        /// <returns>The in memory dictionary string</returns>
        private Dictionary<string, string> GetSimpleDictionary()
        {
            return new Dictionary<string, string>
            {
                { "BooleanValue", "false" },
                { "DateTimeValue", "2020-09-01 13:00:00.0000" },
                { "EnumerationValue", "1" },
                { "IntegerValue", "42" },
                { "StringValue", "StringValueMemory" },
                { "TimeSpanValue", "01:02:03.4567" },
                { "ObjectValue:StringValue", "SubstringMemory" }
            };
        }

        /// <summary>
        /// Retrieves a simple version of the command line arguments for configuring the container
        /// </summary>
        /// <returns>The command line arguments</returns>
        private string[] GetSimpleCommandLine()
        {
            return new string[]
            {
                "--BooleanValue=true",
                "--DateTimeValue=2020-09-01 14:00:00.0000",
                "--EnumerationValue=2",
                "--IntegerValue=53",
                "--StringValue=StringValueCommand",
                "--TimeSpanValue=01:03:04.4567",
                "--ObjectValue:StringValue=SubstringCommand"
            };
        }

        /// <summary>
        /// Retrieves a sparse version of the JSON string for configuring the container
        /// </summary>
        /// <returns>The JSON string</returns>
        private string GetSparseJson()
        {
            return "{ \"BooleanValue\": true, \"IntegerValue\": 72 }";
        }

        /// <summary>
        /// Retrieves a sparse version of the in memory dictionary for configuring the container
        /// </summary>
        /// <returns>The in memory dictionary string</returns>
        private Dictionary<string, string> GetSparseDictionary()
        {
            return new Dictionary<string, string>
            {
                { "StringValue", "StringValueMemory" },
                { "ObjectValue:StringValue", "SubstringMemory" }
            };
        }

        /// <summary>
        /// Retrieves a sparse version of the command line arguments for configuring the container
        /// </summary>
        /// <returns>The command line arguments</returns>
        private string[] GetSparseCommandLine()
        {
            return new string[]
            {
                "--DateTimeValue=2020-09-01 14:00:00.0000"
            };
        }
        #endregion
        #region Configuration Type
        /// <summary>
        /// A configuration test object
        /// </summary>
        [ExcludeFromCodeCoverage]
        public sealed class DemoConfiguration
        {
            /// <summary>
            /// A value with boolean data type
            /// </summary>
            public bool BooleanValue { get; set; }

            /// <summary>
            /// A value with integer data type
            /// </summary>
            public int IntegerValue { get; set; }

            /// <summary>
            /// A value with a string data type
            /// </summary>
            public string StringValue { get; set; }

            /// <summary>
            /// A value with an enumeration data type
            /// </summary>
            public DemoType EnumerationValue { get; set; }

            /// <summary>
            /// A value with a date time data type
            /// </summary>
            public DateTime DateTimeValue { get; set; }

            /// <summary>
            /// A value with a time span data type
            /// </summary>
            public TimeSpan TimeSpanValue { get; set; }

            /// <summary>
            /// A value with an object data type
            /// </summary>
            public SubConfiguration ObjectValue { get; set; }
        }

        /// <summary>
        /// Demo type definition
        /// </summary>
        public enum DemoType
        {
            Simple = 1,
            Complex = 2
        }

        /// <summary>
        /// A configuration object used as a sub object
        /// </summary>
        [ExcludeFromCodeCoverage]
        public sealed class SubConfiguration
        {
            /// <summary>
            /// A value with a string data type
            /// </summary>
            public string StringValue { get; set; }
        }
        #endregion
    }
}

