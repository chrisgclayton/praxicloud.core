// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.configuration
{
    #region Using Clauses
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    #endregion

    /// <summary>
    /// Common configuration related operations
    /// </summary>
    public static class ConfigurationUtilities
    {
        #region Methods
        /// <summary>
        /// Checks to see if the path is fully qualified and if not combines with the config directory
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="defaultSubDirectory">Default directory relative to the current directory that configuration files are stored in if an absolute path is not provided</param>
        /// <returns>A fully qualified file name</returns>
        public static string ExpandConfigFileName(string fileName, string defaultSubDirectory)
        {
            var baseDirectory = string.IsNullOrWhiteSpace(defaultSubDirectory) ? Environment.CurrentDirectory : Path.Combine(Environment.CurrentDirectory, defaultSubDirectory);

            return Path.IsPathFullyQualified(fileName) ? fileName : Path.Combine(baseDirectory, fileName);
        }

        /// <summary>
        /// Create a populated configuration object based on the specified configuration elements
        /// </summary>
        /// <typeparam name="T">The type of the configuration object to be returned</typeparam>
        /// <param name="json">JSON string that is used to configure</param>
        /// <param name="fileName">The file name to be loaded or null if none</param>
        /// <param name="defaultSubDirectory">Default subdirectory the file is located in if not in the current directory or fully qualified</param>
        /// <param name="commandLineArguments">Command line arguments to be used for configuration</param>
        /// <param name="memoryValues">An in memory collection that is used to populate configuraiton data</param>
        /// <param name="keyVaultOptions">Key fault information to be used for configuration data that are prefixed if one is provided (secrets)</param>
        /// <returns>A popualted instance of the configuration object</returns>
        public static T CreateFrom<T>(string json, string fileName, string defaultSubDirectory = null, string[] commandLineArguments = null, Dictionary<string, string> memoryValues = null) where T : class
        {
            return (T)CreateFrom(typeof(T), json, fileName, defaultSubDirectory, commandLineArguments, memoryValues);
        }

        /// <summary>
        /// Create a populated configuration object based on the specified configuration elements
        /// </summary>
        /// <param name="configurationType">The type of the configuration object to be returned</param>
        /// <param name="json">JSON string that is used to configure</param>
        /// <param name="fileName">The file name to be loaded or null if none</param>
        /// <param name="defaultSubDirectory">Default subdirectory the file is located in if not in the current directory or fully qualified</param>
        /// <param name="commandLineArguments">Command line arguments to be used for configuration</param>
        /// <param name="memoryValues">An in memory collection that is used to populate configuraiton data</param>
        /// <param name="keyVaultOptions">Key fault information to be used for configuration data 
        public static object CreateFrom(Type configurationType, string json, string fileName, string defaultSubDirectory = null, string[] commandLineArguments = null, Dictionary<string, string> memoryValues = null)
        {
            var builder = new ConfigurationBuilder();

            if (memoryValues != null && memoryValues.Count > 0) builder.AddInMemoryCollection(memoryValues);
            builder.AddEnvironmentVariables();

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                fileName = ExpandConfigFileName(fileName, defaultSubDirectory);
                builder.AddJsonFile(fileName, true);
            }

            if (commandLineArguments != null && commandLineArguments.Length > 0) builder.AddCommandLine(commandLineArguments);

            if (!string.IsNullOrWhiteSpace(json))
            {
                builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)));
            }

            IConfigurationRoot root = null;

            root = builder.Build();

            var configuration = Activator.CreateInstance(configurationType);

            root.Bind(configuration);

            return configuration;
        }

        /// <summary>
        /// Looks up a value in a dictionary of string, object pairs
        /// </summary>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key to retrieve</param>
        /// <param name="defaultValue">The default value to set to if not found</param>
        /// <returns>The value retrieved or default if not found or invalid</returns>
        public static bool GetFromDictionary(Dictionary<string, string> dictionary, string key, bool defaultValue)
        {
            bool returnValue = defaultValue;

            if (dictionary.TryGetValue(key, out var value))
            {
                if (bool.TryParse(value, out var dataValue))
                {
                    returnValue = dataValue;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Looks up a value in a dictionary of string, object pairs
        /// </summary>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key to retrieve</param>
        /// <param name="defaultValue">The default value to set to if not found</param>
        /// <returns>The value retrieved or default if not found or invalid</returns>
        public static int GetFromDictionary(Dictionary<string, string> dictionary, string key, int defaultValue)
        {
            int returnValue = defaultValue;

            if (dictionary.TryGetValue(key, out var value))
            {
                if (int.TryParse(value, out var dataValue))
                {
                    returnValue = dataValue;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Looks up a value in a dictionary of string, object pairs
        /// </summary>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key to retrieve</param>
        /// <param name="defaultValue">The default value to set to if not found</param>
        /// <returns>The value retrieved or default if not found or invalid</returns>
        public static double GetFromDictionary(Dictionary<string, string> dictionary, string key, double defaultValue)
        {
            double returnValue = defaultValue;

            if (dictionary.TryGetValue(key, out var value))
            {
                if (double.TryParse(value, out var dataValue))
                {
                    returnValue = dataValue;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Looks up a value in a dictionary of string, object pairs
        /// </summary>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key to retrieve</param>
        /// <param name="defaultValue">The default value to set to if not found</param>
        /// <returns>The value retrieved or default if not found or invalid</returns>
        public static byte GetFromDictionary(Dictionary<string, string> dictionary, string key, byte defaultValue)
        {
            byte returnValue = defaultValue;

            if (dictionary.TryGetValue(key, out var value))
            {
                if (byte.TryParse(value, out var dataValue))
                {
                    returnValue = dataValue;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Looks up a value in a dictionary of string, object pairs
        /// </summary>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key to retrieve</param>
        /// <param name="defaultValue">The default value to set to if not found</param>
        /// <returns>The value retrieved or default if not found or invalid</returns>
        public static string GetFromDictionary(Dictionary<string, string> dictionary, string key, string defaultValue)
        {
            string returnValue = defaultValue;

            if (dictionary.TryGetValue(key, out var value))
            {
                returnValue = value;
            }

            return returnValue;
        }

        /// <summary>
        /// Looks up a value in a dictionary of string, object pairs
        /// </summary>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key to retrieve</param>
        /// <param name="defaultValue">The default value to set to if not found</param>
        /// <returns>The value retrieved or default if not found or invalid</returns>
        public static TimeSpan GetFromDictionary(Dictionary<string, string> dictionary, string key, TimeSpan defaultValue)
        {
            TimeSpan returnValue = defaultValue;

            if (dictionary.TryGetValue(key, out var value))
            {
                if (TimeSpan.TryParse(value, out var dataValue))
                {
                    returnValue = dataValue;
                }
            }

            return returnValue;
        }
        #endregion
    }
}
