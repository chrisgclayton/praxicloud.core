// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.exceptions.extensions
{
    #region Using Clauses
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    #endregion

    /// <summary>
    /// Extension class used for flattening exceptions
    /// </summary>
    public static class ExceptionExtensions
    {
        #region Public Methods
        /// <summary>
        /// Flattens the exceptions error message including all inner exceptions.
        /// </summary>
        /// <param name="exception">The exception to flatten.</param>
        /// <returns>A string representing the exceptions error message concatenated with the inner exception messages.</returns>
        public static string FlattenToString(this Exception exception)
        {
            var flatException = string.Empty;

            if (exception != null)
            {
                var aggregateException = exception as AggregateException;

                flatException = aggregateException == null ? FlattenException(exception) : FlattenAggregateException(aggregateException);
            }

            return flatException;
        }

        /// <summary>
        /// Determines if any items in the list are typically fatal
        /// </summary>
        /// <param name="exception">The exception that is being analyzed</param>
        /// <returns>True if the exception is fatal</returns>
        public static bool IsFatal(this Exception exception)
        {
            var fatalFound = false;

            while (exception != null && !fatalFound)
            {
                if (exception is DataException || (exception is OutOfMemoryException && !(exception is InsufficientMemoryException)) ||
                    exception is AccessViolationException || exception is SEHException)
                {
                    fatalFound = true;
                }
                else if (exception is TypeInitializationException || exception is TargetInvocationException)
                {
                    exception = exception.InnerException;
                }
                else if (exception is AggregateException)
                {
                    ReadOnlyCollection<Exception> innerExceptions = ((AggregateException)exception).InnerExceptions;

                    foreach (Exception innerException in innerExceptions)
                    {
                        if (IsFatal(innerException))
                        {
                            fatalFound = true;
                        }
                    }

                    exception = null;
                }
                else
                {
                    exception = null;
                }
            }

            return fatalFound;
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Flattens a standard exception object.
        /// </summary>
        /// <param name="exception">The exception to be flattened.</param>
        /// <returns>A string that concatenates all inner exception messages as well.</returns>
        private static string FlattenException(Exception exception)
        {
            var builder = new StringBuilder(exception.Message);

            if (exception.InnerException != null)
            {
                var aggregateException = exception.InnerException as AggregateException;

                builder.AppendLine();
                builder.Append(aggregateException == null
                    ? FlattenException(exception.InnerException)
                    : FlattenAggregateException(aggregateException));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Flattens an aggregate exception object.
        /// </summary>
        /// <param name="exception">An aggregate exception object to be flattened.</param>
        /// <returns>A string that concatenates all inner exception messages as well.</returns>
        private static string FlattenAggregateException(AggregateException exception)
        {
            var builder = new StringBuilder();

            if (exception.InnerExceptions != null)
            {
                builder.AppendLine(exception.Message);

                foreach (Exception innerException in exception.InnerExceptions)
                {
                    var innerAggregateException = innerException as AggregateException;

                    builder.AppendLine();

                    if (innerAggregateException != null)
                    {
                        builder.Append(FlattenAggregateException(innerAggregateException));
                    }
                    else
                    {
                        builder.Append(FlattenException(innerException));
                    }
                }
            }

            return builder.ToString();
        }
        #endregion
    }
}
