// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.math
{
    #region using Clauses
    using System;
    #endregion

    /// <summary>
    /// A type used to calculate common aggregates across numeric sets
    /// </summary>
    public static class Aggregates
    {
        /// <summary>
        /// Calculates a quantile / percentile for the value across the set. This is is the value in the position within the set split into 100 samples.
        /// </summary>
        /// <param name="values">The set of values to calculate the aggregate</param>
        /// <param name="quantile">The quantile to be calculated, between and 100</param>
        /// <param name="isSorted">True is an optimization to indicate that the array is sorted already</param>
        /// <returns>Null if there are no values, values is null or the qantile is not greater than 0 and less than 100.</returns>
        public static double? Quantile(double[] values, double quantile, bool isSorted = false)
        {
            double? result = null;           
            
            if(values != null && values.Length > 0 && quantile >= 0.0 && quantile <= 100.0)
            {
                if (!isSorted) Array.Sort(values);

                if (quantile == 0.0)
                {
                    result = values[0];
                }
                else if(quantile == 100.0)
                {
                    result = values[values.Length - 1];
                }
                else
                {
                    var index = Convert.ToInt32(Math.Ceiling(quantile * values.Length));

                    result = values[index - 1];
                }
            }            

            return result;
        }

        /// <summary>
        /// Calculates the sum of the values provided or null if no values exist
        /// </summary>
        /// <param name="values">The set of values to calculate the aggregate</param>
        /// <returns>The sum of the values provided or null if there is less than 1</returns>
        public static double? Sum(double[] values)
        {
            double? result = null;

            if (values != null)
            {
                var total = 0.0;

                for (var index = 0; index < values.Length; index++)
                {
                    total += values[index];
                }

                result = total;
            }

            return result;
        }

        /// <summary>
        /// Calculates the mean of the values or null if there is less than 1 value 
        /// </summary>
        /// <param name="values">The set of values to calculate the aggregate</param>
        /// <returns>The mean of the values provided or null if there is less than 1</returns>
        public static double? Mean(double[] values)
        {
            double? result = null;

            if (values != null && values.Length > 0)
            {
                var total = 0.0;

                for (var index = 0; index < values.Length; index++)
                {
                    total += values[index];
                }

                result = total / values.Length;
            }

            return result;
        }

        /// <summary>
        /// Finds the 50th percentile
        /// </summary>
        /// <param name="values">The set of values to calculate the aggregate</param>
        /// <param name="isSorted">True is an optimization to indicate that the array is sorted already</param>
        /// <returns>The middle number in the values</returns>
        public static double? Median(double[] values, bool isSorted = false)
        {
            return Quantile(values, 0.50, isSorted);
        }

        /// <summary>
        /// Calculates the standard deviation for the provided value set
        /// </summary>
        /// <param name="values">The set of values to calculate the aggregate</param>
        /// <returns>The standard deviation or null if there is less than 1 value</returns>
        public static double? StandardDeviation(double[] values)
        {
            double? result = null;

            if (values != null)
            {
                if(values.Length > 0)
                {
                    if(values.Length == 1)
                    {
                        result = 0.0;
                    }
                    else
                    {
                        var total = values[0];

                        for (var index = 1; index < values.Length; index++)
                        {
                            total += values[index];
                        }

                        var mean = total / values.Length;
                        var stdValueTotals = 0.0;

                        for (var index = 0; index < values.Length; index++)
                        {
                            stdValueTotals = Math.Pow(mean - values[index], 2);
                        }

                        result = Math.Sqrt(stdValueTotals / (values.Length - 1));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// A helper method to retrieve most of the common values associated with performance monitoring
        /// </summary>
        /// <param name="values">The values to calculate the aggregates for</param>
        /// <param name="maximum">The maximum value in the set</param>
        /// <param name="minimum">The minimum value in the set</param>
        /// <param name="mean">The average value in the set</param>
        /// <param name="sum">The total of all values in the set</param>
        /// <param name="standardDeviation">The standard deviation of the set</param>
        /// <param name="p50">The 50th percentile</param>
        /// <param name="p90">The 90th percentile</param>
        /// <param name="p95">The 95th percentile</param>
        /// <param name="p98">The 98th percentile</param>
        /// <param name="p99">The 99th percentile</param>
        public static void GetPerformanceAggregates(double[] values, out double? maximum, out double? minimum, out double? mean, out double? sum, out double? standardDeviation, out double? p50, out double? p90, out double? p95, out double? p98, out double? p99)
        {
            standardDeviation = null;
            mean = null;

            if (values != null && values.Length > 0)
            {
                if (values.Length == 1)
                {
                    standardDeviation = 0.0;
                    mean = values[0];
                    p50 = values[0];
                    p90 = values[0];
                    p95 = values[0];
                    p98 = values[0];
                    p99 = values[0];
                    maximum = values[0];
                    minimum = values[0];
                    sum = values[0];
                }
                else
                {
                    Array.Sort(values);
                    sum = values[0];

                    for (var index = 1; index < values.Length; index++)
                    {
                        sum += values[index];
                    }

                    mean = sum / values.Length;
                    var stdValueTotals = 0.0;

                    for (var index = 0; index < values.Length; index++)
                    {
                        stdValueTotals = Math.Pow(mean.Value - values[index], 2);
                    }

                    standardDeviation = Math.Sqrt(stdValueTotals / (values.Length - 1));

                    minimum = values[0];
                    maximum = values[values.Length - 1];
                    p50 = values[Convert.ToInt32(Math.Ceiling(0.50 * values.Length)) - 1];
                    p90 = values[Convert.ToInt32(Math.Ceiling(0.90 * values.Length)) - 1];
                    p95 = values[Convert.ToInt32(Math.Ceiling(0.95 * values.Length)) - 1];
                    p98 = values[Convert.ToInt32(Math.Ceiling(0.98 * values.Length)) - 1];
                    p99 = values[Convert.ToInt32(Math.Ceiling(0.99 * values.Length)) - 1];
                }
            }
            else
            {
                sum = null;                
                p50 = null;
                p90 = null;
                p95 = null;
                p98 = null;
                p99 = null;
                maximum = null;
                minimum = null;
            }

        }
    }
}
