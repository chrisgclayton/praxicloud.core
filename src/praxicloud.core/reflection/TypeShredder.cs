// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.reflection
{
    #region Using Clauses
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using praxicloud.core.security;
    #endregion

    /// <summary>
    /// Reflection based class that creates delegates (compiled expressions) for property setters and getters.
    /// </summary>
    public sealed class TypeShredder
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of the type shredder.
        /// </summary>
        /// <param name="itemType">The type being shredded.</param>
        /// <param name="ignoreList">An optional list of property names to ignore.</param>
        public TypeShredder(Type itemType, List<string> ignoreList = null)
        {
            Guard.NotNull(nameof(itemType), itemType);

            TypeRepresented = itemType;
            Name = itemType.FullName;
            ExpressionList = GetTypePropertyExpressions(itemType, ignoreList);
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Retrieves a list of property expressions based on the type passed in.
        /// </summary>
        /// <param name="itemType">The type that contains the properties to be shredded.</param>
        /// <param name="ignoreList">An optional list of property names to ignore.</param>
        /// <returns>A list of property setters and getters.</returns>
        private static List<PropertyExpressions> GetTypePropertyExpressions(Type itemType, List<string> ignoreList)
        {
            List<PropertyInfo> properties;

            if (ignoreList == null || !ignoreList.Any())
            {
                properties = itemType.GetRuntimeProperties().ToList();
            }
            else
            {
                properties = itemType.GetRuntimeProperties().Where(property => !ignoreList.Any(element => string.Equals(property.Name, element, StringComparison.Ordinal))).ToList();
            }

            return properties.Select(propertyInfo => new PropertyExpressions
            {
                GetExpression = propertyInfo.CanRead ? PropertyExpressions.GetCompiledGetterExpression(propertyInfo) : null,
                SetExpression = propertyInfo.CanWrite ? PropertyExpressions.GetCompiledSetterExpression(propertyInfo) : null,
                PropertyName = propertyInfo.Name,
                PropertyType = propertyInfo.PropertyType
            }).ToList();
        }

        #endregion
        #region Public Properties
        /// <summary>
        /// The type that was shredded.
        /// </summary>
        public Type TypeRepresented { get; }

        /// <summary>
        /// The name of the type that was shredded.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A list of expressions based on the properties that were shredded.
        /// </summary>
        public List<PropertyExpressions> ExpressionList { get; }
        #endregion
    }
}
