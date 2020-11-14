// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.configuration
{
    #region Using Clauses
    using System;
    #endregion

    /// <summary>
    /// A set of common utilities to output proeprty inforamtion
    /// </summary>
    public static class PropertyDump
    {
        #region Delegates
        /// <summary>
        /// Signature of a type that is used to write the property name and value
        /// </summary>
        /// <param name="propertyName">The name of the property to write</param>
        /// <param name="value">The value of the property to write</param>
        /// <param name="type">The type of the property</param>
        public delegate void WriteProperty(string propertyName, object value, Type type);
        #endregion
        #region Methods
        /// <summary>
        /// Writes the properties of an instance to a delegate if not decorated with the DoNoTOutput attribute
        /// </summary>
        /// <param name="item">The item to output</param>
        /// <param name="writer">The method to invoke for writing a property value</param>        
        public static void WriteConfiguration(object item, WriteProperty writer)
        {
            foreach (var property in item.GetType().GetProperties())
            {
                if (Attribute.GetCustomAttribute(property, typeof(DoNotOutputAttribute)) == null)
                {
                    if (property.GetMethod != null)
                    {
                        writer(property.Name, property.GetMethod.Invoke(item, null), property.PropertyType);
                    }
                }
            }
        }
        #endregion
    }
}
