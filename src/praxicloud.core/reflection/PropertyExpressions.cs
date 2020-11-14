// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.reflection
{
    #region Using Clauses
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using praxicloud.core.security;
    #endregion

    /// <summary>
    /// A container used to keep property related expressions and data together.
    /// </summary>
    public sealed class PropertyExpressions
    {
        #region Public Properties
        /// <summary>
        /// The name of the proprety that the contained values represents.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The type of the property.  
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// The property getter as a compiled lambda expression.
        /// </summary>
        public Delegate GetExpression { get; set; }

        /// <summary>
        /// The property setter as a compiled lambda expression.
        /// </summary>
        public Delegate SetExpression { get; set; }
        #endregion
        #region Public Methods
        /// <summary>
        /// Simplifies the retrieval of the value from the associated getter.
        /// </summary>
        /// <typeparam name="T">The type of the object that contains the getter.</typeparam>
        /// <param name="containingObject">The object that contains the property.</param>
        /// <returns>The value returned from the containingOjbect's getter.</returns>
        public object GetValue<T>(T containingObject) where T : class
        {
            Guard.NotNull(nameof(containingObject), containingObject);

            return ((Func<T, object>)GetExpression)(containingObject);
        }

        /// <summary>
        /// Simplifies the setting of the value using the associated setter.
        /// </summary>
        /// <typeparam name="T">The type of the object that contains the setter.</typeparam>
        /// <param name="containingObject">The object that contains the property.</param>
        /// <param name="propertyValue">The value to be assigned to the property.</param>
        public void SetValue<T>(T containingObject, object propertyValue) where T : class
        {
            Guard.NotNull(nameof(containingObject), containingObject);

            var setter = (Action<T, object>)SetExpression;

            setter(containingObject, propertyValue);
        }
        #endregion
        #region Public Static Methods
        /// <summary>
        /// Creates a compiled setter expression based on the property information.
        /// </summary>
        /// <param name="propertyInfo">The property information that the expression is to be based on.</param>
        /// <returns>A delegate of the setter or null if it was not valid.</returns>
        public static Delegate GetCompiledSetterExpression(PropertyInfo propertyInfo)
        {
            Guard.NotNull(nameof(propertyInfo), propertyInfo);

            Delegate setterDelegate = null;
            var declaringType = propertyInfo.DeclaringType;

            if (declaringType != null)
            {
                var parameter = Expression.Parameter(declaringType, "i");                   // Create a parameter of type for the object to be passed in
                var argument = Expression.Parameter(typeof(object), "a");                   // Create an object to represent the value being passed in.  This is an object and converted
                // to simplify calling it
                var typeConversion = Expression.Convert(argument, propertyInfo.PropertyType);   // Convert the object passed in to the strongly typed property type.
                var setterCall = Expression.Call(parameter, propertyInfo.SetMethod, typeConversion);   // Set the arguments to include the cast

                setterDelegate = Expression.Lambda(setterCall, parameter, argument).Compile();              // Compile the expression
            }

            return setterDelegate;
        }

        /// <summary>
        /// Creates a compiled getter expression based on the property information
        /// </summary>
        /// <param name="propertyInfo">The property information that the expression is to be based on.</param>
        /// <returns>A delegate of the getter or null if it was not valid.</returns>
        public static Delegate GetCompiledGetterExpression(PropertyInfo propertyInfo)
        {
            Guard.NotNull(nameof(propertyInfo), propertyInfo);

            Delegate getterDelegate = null;
            var declaringType = propertyInfo.DeclaringType;

            if (declaringType != null)
            {
                var parameter = Expression.Parameter(declaringType, "i");   // Create a parameter of type for the object to be passed in
                var property = Expression.Property(parameter, propertyInfo);   // Create a property accessor
                var convert = Expression.TypeAs(property, typeof(object));      // Convert the value to an object

                getterDelegate = Expression.Lambda(convert, parameter).Compile();           // Compile the expression
            }

            return getterDelegate;
        }
        #endregion
    }
}
