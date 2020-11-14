// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.security
{
    #region Using Clauses
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Properties;
    using System.Text;
    #endregion

    /// <summary>
    /// A set of methods used to validate publicly input parameters.
    /// </summary>
    public static class Guard
    {
        #region Public Guard Methods
        /// <summary>
        /// Tests to make sure the supplied parameter value is not null.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        public static void NotNull<T>(string parameterName, T parameterValue) where T : class
        {
            if (parameterValue == null)
            {
                throw new GuardException(parameterName, string.Format(Resources.NotNull, parameterName));
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter is not the default value for the type.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        public static void NotDefault<T>(string parameterName, T parameterValue)
        {
            if (Equals(parameterValue, default(T)))
            {
                throw new GuardException(parameterName, string.Format(Resources.NotDefault, parameterName));
            }
        }

        /// <summary>
        /// Tests to make sure the supplied string parameter is not null or empty.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        public static void NotNullOrEmpty(string parameterName, string parameterValue)
        {
            NotNull(parameterName, parameterValue);

            if (string.IsNullOrEmpty(parameterValue))
            {
                throw new GuardException(string.Format(Resources.NotNullOrEmpty, parameterName), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied string parameter is not null or all whitespace.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        public static void NotNullOrWhitespace(string parameterName, string parameterValue)
        {
            NotNull(parameterName, parameterValue);

            if (string.IsNullOrWhiteSpace(parameterValue))
            {
                throw new GuardException(string.Format(Resources.NotNullOrWhitespace, parameterName), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied string parameter is in the desired length range inclusively
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="minimum">The minimum length of the string inclusively</param>
        /// <param name="maximum">The maximum length of the string inclusively</param>
        public static void StringLengthInRange(string parameterName, string parameterValue, int minimum, int maximum)
        {
            var stringLenth = parameterValue?.Length ?? 0;

            if (stringLenth < minimum || stringLenth > maximum)
            {
                throw new GuardException(string.Format(Resources.StringLengthInRange, parameterName, minimum, maximum), parameterName);
            }
        }

        /// <summary>
        /// Tests to see if the directory provided in the parameter value exists.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        public static void DirectoryExists(string parameterName, string parameterValue)
        {
            NotNullOrWhitespace(parameterName, parameterValue);

            if (!Directory.Exists(parameterValue))
            {
                throw new GuardException(string.Format(Resources.DirectoryExists, parameterName), parameterName);
            }
        }

        /// <summary>
        /// Tests to see if the directory provided in the parameter value exists.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        public static void DirectoryNotExists(string parameterName, string parameterValue)
        {
            NotNullOrWhitespace(parameterName, parameterValue);

            if (Directory.Exists(parameterValue))
            {
                throw new GuardException(string.Format(Resources.DirectoryNotExists, parameterName), parameterName);
            }
        }


        /// <summary>
        /// Tests to see if the file provided in the parameter value exists.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        public static void FileExists(string parameterName, string parameterValue)
        {
            NotNullOrWhitespace(parameterName, parameterValue);

            if (!File.Exists(parameterValue))
            {
                throw new GuardException(string.Format(Resources.FileExists, parameterName), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the file provided in the parameter does not exist.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        public static void FileNotExists(string parameterName, string parameterValue)
        {
            NotNullOrWhitespace(parameterName, parameterValue);

            if (File.Exists(parameterValue))
            {
                throw new GuardException(string.Format(Resources.FileNotExists, parameterName), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="minimumValue">The minimum allowable value of the parameter.</param>
        public static void NotLessThan(string parameterName, TimeSpan parameterValue, TimeSpan minimumValue)
        {
            if (parameterValue < minimumValue)
            {
                throw new GuardException(string.Format(Resources.NotLessThan, parameterName, minimumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="minimumValue">The minimum allowable value of the parameter.</param>
        public static void NotLessThan(string parameterName, DateTime parameterValue, DateTime minimumValue)
        {
            if (parameterValue < minimumValue)
            {
                throw new GuardException(string.Format(Resources.NotLessThan, parameterName, minimumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="minimumValue">The minimum allowable value of the parameter.</param>
        public static void NotLessThan(string parameterName, double parameterValue, double minimumValue)
        {
            if (parameterValue < minimumValue)
            {
                throw new GuardException(string.Format(Resources.NotLessThan, parameterName, minimumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="minimumValue">The minimum allowable value of the parameter.</param>
        public static void NotLessThan(string parameterName, long parameterValue, long minimumValue)
        {
            if (parameterValue < minimumValue)
            {
                throw new GuardException(string.Format(Resources.NotLessThan, parameterName, minimumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="maximumValue">The maximum allowable value of the parameter.</param>
        public static void NotMoreThan(string parameterName, TimeSpan parameterValue, TimeSpan maximumValue)
        {
            if (parameterValue > maximumValue)
            {
                throw new GuardException(string.Format(Resources.NotMoreThan, parameterName, maximumValue), parameterName);
            }
        }


        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="maximumValue">The minimum allowable value of the parameter.</param>
        public static void NotMoreThan(string parameterName, DateTime parameterValue, DateTime maximumValue)
        {
            if (parameterValue > maximumValue)
            {
                throw new GuardException(string.Format(Resources.NotMoreThan, parameterName, maximumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="maximumValue">The maximum allowable value of the parameter.</param>
        public static void NotMoreThan(string parameterName, double parameterValue, double maximumValue)
        {
            if (parameterValue > maximumValue)
            {
                throw new GuardException(string.Format(Resources.NotMoreThan, parameterName, maximumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="maximumValue">The maximum allowable value of the parameter.</param>
        public static void NotMoreThan(string parameterName, long parameterValue, long maximumValue)
        {
            if (parameterValue > maximumValue)
            {
                throw new GuardException(string.Format(Resources.NotMoreThan, parameterName, maximumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="maximumValue">The maximum allowable value of the parameter.</param>
        public static void NotMoreThan(string parameterName, ulong parameterValue, ulong maximumValue)
        {
            if (parameterValue > maximumValue)
            {
                throw new GuardException(string.Format(Resources.NotMoreThan, parameterName, maximumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="maximumValue">The maximum allowable value of the parameter.</param>
        public static void NotMoreThan(string parameterName, float parameterValue, float maximumValue)
        {
            if (parameterValue > maximumValue)
            {
                throw new GuardException(string.Format(Resources.NotMoreThan, parameterName, maximumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="minimumValue">The minimum allowable value of the parameter.</param>
        public static void NotLessThan(string parameterName, ulong parameterValue, ulong minimumValue)
        {
            if (parameterValue < minimumValue)
            {
                throw new GuardException(string.Format(Resources.NotLessThan, parameterName, minimumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure the supplied parameter value is not less than the minimum value.  If it is a Guard Exception will be raised.
        /// </summary>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="minimumValue">The minimum allowable value of the parameter.</param>
        public static void NotLessThan(string parameterName, float parameterValue, float minimumValue)
        {
            if (parameterValue < minimumValue)
            {
                throw new GuardException(string.Format(Resources.NotLessThan, parameterName, minimumValue), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure that a parameter representing a buffer position offset is not outside of the allowable bounds the specified enumerable.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="items">The enumeration of items being validated againts.</param>
        /// <param name="offset">The offset that must exist within the bounds.</param>
        public static void IndexNotOutOfRange<T>(string parameterName, IEnumerable<T> items, int offset)
        {
            if (items == null || items.Count() <= offset)
            {
                throw new GuardException(string.Format(Resources.IndexOutOfRange, parameterName), parameterName);
            }
        }

        /// <summary>
        /// Tests to make sure that the beginning and end of the segment provided will not be outside of the enumerables bounds. 
        /// </summary>
        /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="items">The enumeration of items being validated against.</param>
        /// <param name="offset">The offset that the segment starts at.</param>
        /// <param name="count">The number of elements in the segment.</param>
        public static void SegmentNotOutOfRange<T>(string parameterName, IEnumerable<T> items, int offset, int count)
        {
            var itemCount = items?.Count() ?? -1;

            if (offset < 0 || itemCount < offset + count)
            {
                throw new GuardException(string.Format(Resources.SegmentOutOfRange, parameterName), parameterName);
            }
        }

        /// <summary>
        /// Raises a GuardException if the value is not assignable to the desired type.
        /// </summary>
        /// <typeparam name="T">The type that the value must be assignable from.</typeparam>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        public static void IsAssignableTo<T>(string parameterName, object parameterValue) where T : class
        {
            if (!(parameterValue is T))
            {
                throw new GuardException(string.Format(Resources.AssignableFrom, parameterName, typeof(T).Name), parameterName);
            }
        }

        /// <summary>
        /// Raises a GuardException if the value of the paraemter has more or less elements than identified.
        /// </summary>
        /// <typeparam name="T">The type of the array being validated.</typeparam>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="minimumNumberOfElements">The minimum number of elements the array must contain.</param>
        /// <param name="maximumNumberOfElements">The maximum number of elements the array must contain.</param>
        public static void ElementCountInRange<T>(string parameterName, T[] parameterValue, int minimumNumberOfElements, int maximumNumberOfElements)
        {
            if (parameterValue.Length < minimumNumberOfElements || parameterValue.Length > maximumNumberOfElements)
            {
                throw new GuardException(string.Format(Resources.ElementCountInRange, parameterName, minimumNumberOfElements, maximumNumberOfElements, parameterValue.Length), parameterName);
            }
        }

        /// <summary>
        /// Raises a GuardException if the value of the paraemter has less elements than identified.
        /// </summary>
        /// <typeparam name="T">The type of the array being validated.</typeparam>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <param name="parameterValue">The value of the parameter being validated.</param>
        /// <param name="minimumNumberOfElements">The minimum number of elements the array must contain.</param>
        public static void ElementCountAtLeast<T>(string parameterName, T[] parameterValue, int minimumNumberOfElements)
        {
            if (parameterValue.Length < minimumNumberOfElements)
            {
                throw new GuardException(string.Format(Resources.ElementCountAtLeast, parameterName, minimumNumberOfElements, parameterValue.Length), parameterName);
            }
        }

        /// <summary>
        /// Checks whether or not the specified collection is empty.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValues">The values of the argument.</param>
        /// <param name="parameterName">The name of the parameter for diagnostic purposes.</param>
        public static void CollectionNotEmpty<T>(string parameterName, ICollection<T> argumentValues)
        {           
            if (argumentValues == null || argumentValues.Count == 0)
            {
                throw new GuardException(String.Format(CultureInfo.CurrentCulture, Resources.CollectionCannotBeEmpty, parameterName), parameterName);
            }
        }

        /// <summary>
        /// Checks an argument to ensure its value is expected value
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="expectedValue">The expected value of the argument.</param>
        /// <param name="parameterName">The name of the parameter for diagnostic purposes.</param>
        public static void IsEqual<T>(string parameterName, T argumentValue, T expectedValue)
        {
            if (Comparer<T>.Default.Compare(argumentValue, expectedValue) != 0)
            {
                throw new GuardException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidArgumentValue, parameterName, expectedValue), parameterName);
            }
        }

        /// <summary>
        /// Checks that string argument value matches the given regex.
        /// </summary>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="pattern">The regex pattern match.</param>
        /// <param name="parameterName">The name of the parameter for diagnostic purposes.</param>       
        public static void MatchesRegex(string parameterName, string argumentValue, string pattern)
        {
            if (!Regex.IsMatch(argumentValue, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(10)))
            {
                throw new GuardException(string.Format(CultureInfo.CurrentCulture, Resources.StringMustMatchRegex, parameterName, pattern), parameterName);
            }
        }

        /// <summary>
        /// Checks that all values of the specified argument satisfy a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValues">The values of the argument.</param>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <param name="parameterName">The name of the parameter for diagnostic purposes.</param>
        public static void SatisfyCondition<T>(string parameterName, IEnumerable<T> argumentValues, Func<T, bool> predicate)
        {
            if (argumentValues == null || !argumentValues.All(predicate))
            {
                throw new GuardException(String.Format(CultureInfo.CurrentCulture, Resources.ConditionNotSatisfied, parameterName), parameterName);
            }
        }

        /// <summary>
        /// Checks if the supplied argument falls into the given range of values.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="minValue">The minimum allowed value of the argument.</param>
        /// <param name="maxValue">The maximum allowed value of the argument.</param>
        /// <param name="parameterName">The name of the parameter for diagnostic purposes.</param>
        public static void InRange<T>(string parameterName, T argumentValue, T minValue, T maxValue) where T : IComparable<T>
        {
            if (Comparer<T>.Default.Compare(argumentValue, minValue) < 0 || Comparer<T>.Default.Compare(argumentValue, maxValue) > 0)
            {
                throw new GuardException(String.Format(CultureInfo.CurrentCulture, Resources.CannotBeOutOfRange, parameterName, minValue, maxValue), parameterName);
            }
        }

        /// <summary>
        /// Checks if the supplied argument present in the collection of possible values.
        /// </summary>
        /// <remarks>
        /// Comparison is case sensitive
        /// </remarks>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="collection">Collection of possible values</param>
        /// <param name="parameterName">The name of the parameter for diagnostic purposes.</param>
        public static void InCollection<T>(string parameterName, T argumentValue, ICollection<T> collection) where T : IComparable<T>
        {
            NotNull(parameterName, collection);

            if (!collection.Contains(argumentValue))
            {
                throw new GuardException(string.Format(CultureInfo.CurrentCulture, Resources.NotInCollection, parameterName, argumentValue), parameterName);
            }
        }

        /// <summary>
        /// Checks if the supplied argument present in the collection of possible values.
        /// </summary>
        /// <remarks>
        /// Comparison is case sensitive
        /// </remarks>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="array">Array of possible values</param>
        /// <param name="parameterName">The name of the parameter for diagnostic purposes.</param>
        public static void InArray<T>(string parameterName, T argumentValue, T[] array) where T : IComparable<T>
        {
            NotNull(parameterName, array);

            if (!array.Contains(argumentValue))
            {
                throw new GuardException(string.Format(CultureInfo.CurrentCulture, Resources.NotInArray, parameterName, argumentValue), parameterName);
            }
        }

        /// <summary>
        /// Checks an enum instance to ensure that its value is defined by the specified enum type.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <typeparam name="TValue">The value of the parameter being validated.</typeparam>
        /// <param name="enumValue">The enum value to check.</param>
        /// <param name="parameterName">The name of the parameter holding the value.</param>
        /// <remarks>
        /// This method does not currently support Flags enums.
        /// The constraint on the method should be updated to "enum" once the C# compiler supports it.
        /// </remarks>
        public static void EnumValueIsDefined<T, TValue>(string parameterName, TValue enumValue) where T : struct
        {
            if (!Enum.IsDefined(typeof(T), enumValue))
            {
                throw new GuardException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidEnumValue, parameterName, typeof(T)), parameterName);
            }
        }
        #endregion
    }
}