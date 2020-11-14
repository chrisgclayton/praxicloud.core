// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.configuration
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using praxicloud.core.configuration;
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    #endregion

    /// <summary>
    /// Tests the dump of the object
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class Dumps
    {
        /// <summary>
        /// Tests the object dumping of properties
        /// </summary>
        [TestMethod]
        public void DumpTest()
        {
            var outputList = new ConcurrentBag<(string propertyName, object value, Type type)>();
            var container = new ConfigurationContainer
            {
                Description = "Description of the container",
                FileName = "C:\\upload\\file.txt",
                Retries = 6,
                UploadTimeout = TimeSpan.FromMinutes(10)
            };

            PropertyDump.WriteConfiguration(container, (string propertyName, object value, Type type) => outputList.Add((propertyName, value, type)));
    
            foreach(var element in outputList)
            {
                var abc = element;
            }

            var fileName = outputList.Where(item => string.Equals(item.propertyName, "FileName", StringComparison.Ordinal)).First();
            var retries = outputList.Where(item => string.Equals(item.propertyName, "Retries", StringComparison.Ordinal)).First();
            var uploadTimeout = outputList.Where(item => string.Equals(item.propertyName, "UploadTimeout", StringComparison.Ordinal)).First();
            var description = outputList.Where(item => string.Equals(item.propertyName, "Description", StringComparison.Ordinal)).FirstOrDefault();

            Assert.IsNull(description.propertyName, "Description execpted to be null");

            Assert.IsTrue(fileName.type == typeof(string), "File name is expected to be a string");
            Assert.IsTrue(string.Equals((string)fileName.value, "C:\\upload\\file.txt", StringComparison.Ordinal), "File name is expected to be a string");

            Assert.IsTrue(retries.type == typeof(int), "Retries is expected to be an integer");
            Assert.IsTrue((int)retries.value == 6, "Retries is expected to be 6");

            Assert.IsTrue(uploadTimeout.type == typeof(TimeSpan), "Upload timeout is expected to be a Timespan");
            Assert.IsTrue((TimeSpan)uploadTimeout.value == TimeSpan.FromMinutes(10), "Upload timeout is expected to be 6 minutes");
        }

        /// <summary>
        /// A sample configuration container
        /// </summary>
        [ExcludeFromCodeCoverage]
        public class ConfigurationContainer
        {
            /// <summary>
            /// The file to upload
            /// </summary>
            public string FileName { get; set; }
                        
            /// <summary>
            /// The description of the file
            /// </summary>
            [DoNotOutput]
            public string Description { get; set; }

            /// <summary>
            /// The number of retries
            /// </summary>
            public int Retries { get; set; }

            /// <summary>
            /// The timeout
            /// </summary>
            public TimeSpan UploadTimeout { get; set; }
        }
    }
}
