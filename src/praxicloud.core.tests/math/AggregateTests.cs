// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.math
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics.CodeAnalysis;
    using MathNet.Numerics.Statistics;
    using System.Linq;
    using praxicloud.core.math;
    using System;
    using System.Diagnostics;
    using System.Collections;
    #endregion

    /// <summary>
    /// A set of tests that validate the ability to calculate aggregates accurately and performant
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AggregateTests
    {
        /// <summary>
        /// Makes sure that the value of the sum is equal to the value calculated off of the array directly
        /// </summary>
        /// <param name="values">The values to calculate the sum for</param>
        [DataRow(new double[] { 1.0, 2.0, 3.0, 4.0 })]
        [DataRow(new double[] { 5.12, -3.35, 1.23424234, 6.12 })]
        [DataTestMethod]
        public void TestSumAccuracy(double[] values)
        {
            Assert.IsTrue(values.Sum() == Aggregates.Sum(values), $"The built in sum does not equal the aggregates sum { values.Sum() } != { Aggregates.Sum(values) }");
        }

        ///// <summary>
        ///// Tests the performance of the sum compared to the built in
        ///// </summary>
        //[TestMethod]
        //public void TestSumPerformance()
        //{
        //    var generator = new Random();
        //    var values = new double[32000];

        //    for(var index = 0; index < values.Length; index++)
        //    {
        //        values[index] = generator.NextDouble();
        //    }

        //    var aggregateWatch = Stopwatch.StartNew();
        //    var aggregatesValue = Aggregates.Sum(values);
        //    aggregateWatch.Stop();

        //    var builtInWatch = Stopwatch.StartNew();
        //    var builtInValue = values.Sum();
        //    builtInWatch.Stop();

        //    var builtInWithAllowable = builtInWatch.ElapsedTicks * 1.1;

        //    Assert.IsTrue(aggregateWatch.ElapsedTicks <= builtInWithAllowable, $"The aggregates sum was more than 10% longer than the built in { aggregateWatch.ElapsedTicks } is not < { builtInWithAllowable }.");
        //}

        /// <summary>
        /// Makes sure that the value of the mean is equal to the value calculated by the mathnet library, rounded to 6 decimals
        /// </summary>
        /// <param name="values">The values to calculate the mean for</param>
        [DataRow(new double[] { 1.0, 2.0, 3.0, 4.0 })]
        [DataRow(new double[] { 5.12, -3.35, 1.23424234, 6.12 })]
        [DataTestMethod]
        public void TestMeanAccuracy(double[] values)
        {
            var mathnetValue = Statistics.Mean(values);
            var aggregatesValue = Aggregates.Mean(values).Value;

            mathnetValue = Math.Round(mathnetValue, 6);
            aggregatesValue = Math.Round(aggregatesValue, 6);

            Assert.IsNotNull(aggregatesValue, "Aggregates value was not expected to be null");
            Assert.IsTrue(mathnetValue == aggregatesValue, $"The mathnet mean does not equal the aggregates sum { mathnetValue } != { aggregatesValue }");
        }

        ///// <summary>
        ///// Tests the performance of the mean compared to the MathNet 
        ///// </summary>
        //[TestMethod]
        //public void TestMeanPerformance()
        //{
        //    var generator = new Random();
        //    var values = new double[32000];

        //    for (var index = 0; index < values.Length; index++)
        //    {
        //        values[index] = generator.NextDouble();
        //    }

        //    var aggregateWatch = Stopwatch.StartNew();
        //    var aggregatesValue = Aggregates.Mean(values).Value;
        //    aggregateWatch.Stop();

        //    var builtInWatch = Stopwatch.StartNew();
        //    var builtInValue = Statistics.Mean(values);
        //    builtInWatch.Stop();

        //    var builtInWithAllowable = builtInWatch.ElapsedTicks * 1.1;

        //    Assert.IsTrue(aggregateWatch.ElapsedTicks <= builtInWithAllowable, $"The aggregates mean was more than 10% longer than the built in { aggregateWatch.ElapsedTicks } is not < { builtInWithAllowable }.");
        //}

        /// <summary>
        /// Tests the performance of the quantiles compared to the MathNet 
        /// </summary>
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(10)]
        [DataRow(25)]
        [DataRow(50)]
        [DataRow(75)]
        [DataRow(90)]
        [DataRow(95)]
        [DataRow(98)]
        [DataRow(99)]
        [DataRow(100)]
        [DataTestMethod]
        public void TestQuantile(int quantile)
        {
            var generator = new Random();
            var values = new double[100];
            var decimalQuantile = quantile / 100.0;

            for (var index = 0; index < values.Length; index++)
            {
                values[index] = Math.Round(generator.NextDouble(), 4);
            }

            var aggregateWatch = Stopwatch.StartNew();
            var aggregatesValue = Aggregates.Quantile(values, decimalQuantile, false).Value;
            aggregateWatch.Stop();

           // Assert.IsTrue(aggregateWatch.ElapsedTicks <= 4000, $"The aggregates mean was more than 10% longer than the built in { aggregateWatch.ElapsedTicks } is not < 4000 ticks.");

            int expectedIndex = quantile - 1;

            if (quantile == 0) expectedIndex = 0;
            if (quantile == 100) expectedIndex = values.Length - 1;

            Assert.IsTrue(aggregatesValue == values[expectedIndex], "The quantile was not the expected value");
        }

        /// <summary>
        /// Tests the performance of the quantiles compared to the MathNet 
        /// </summary>
        [TestMethod]
        public void TestMedian()
        {
            var generator = new Random();
            var values = new double[100];

            for (var index = 0; index < values.Length; index++)
            {
                values[index] = Math.Round(generator.NextDouble(), 4);
            }

            var aggregateWatch = Stopwatch.StartNew();
            var aggregatesValue = Aggregates.Median(values, false).Value;
            aggregateWatch.Stop();

            ArrayList.Adapter(values).Sort();

//            Assert.IsTrue(aggregateWatch.ElapsedTicks <= 5000, $"The aggregates mean was more than 10% longer than the built in { aggregateWatch.ElapsedTicks } is not < 4000 ticks.");
            Assert.IsTrue(aggregatesValue == values[49], "The median was not the expected value");
        }


        [DataTestMethod]
        public void TestPerformanceAggregates()
        {
            var generator = new Random();
            var values = new double[100];

            for (var index = 0; index < values.Length; index++)
            {
                values[index] = Math.Round(generator.NextDouble(), 4);
            }

            var aggregateWatch = Stopwatch.StartNew();
            Aggregates.GetPerformanceAggregates(values, out var maximum, out var minimum, out var mean, out var sum, out var standardDeviation, out var p50, out var p90, out var p95, out var p98, out var p99);
            aggregateWatch.Stop();

            Assert.IsTrue(maximum == values.Max(), $"Maximum value does not equate {maximum} != {values.Max()}");
            Assert.IsTrue(minimum == values.Min(), $"Minimum value does not equate {minimum} != {values.Min()}");
            Assert.IsTrue(sum == values.Sum(), $"Sum value does not equate {sum} != {values.Sum()}");
            Assert.IsTrue(mean == values.Average(), $"Mean value does not equate {mean} != {values.Average()}");

        }

    }
}
