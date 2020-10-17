// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.tests.exceptions
{
    #region Using Clauses
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Collections.Generic;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using praxicloud.core.containers;
    using System.Threading.Tasks;
    using System.Net;
    using System.Threading;
    using System.Net.Sockets;
    using static praxicloud.core.containers.TcpProbeEndpoint;
    #endregion

    /// <summary>
    /// Tests the TCP health Probe functionality
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AvailabilityProbeTests
    {
        #region Test Methods
        /// <summary>
        /// Creates a TCP health Probe, starts it and stops it
        /// </summary>
        [TestMethod]
        public void StartStop()
        {
            var checker = new TestAvailabilityCheck();

            checker.CurrentAvailable = true;

            var probe = new TestAvailabilityProbe(IPAddress.Any, 10034, TimeSpan.FromMilliseconds(1), checker);
            var stopSuccess = false;
            var startSuccess = probe.StartAsync(CancellationToken.None).GetAwaiter().GetResult();

            if (startSuccess)
            {
                Task.Delay(2000).GetAwaiter().GetResult();
                stopSuccess = probe.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            }

            Assert.IsTrue(startSuccess, "Start was not successful");
            Assert.IsTrue(stopSuccess, "Stop was not successful");
        }

        /// <summary>
        /// Creates a TCP health Probe, starts it, starts it again and stops it
        /// </summary>
        [TestMethod]
        public void DoubleStartStop()
        {
            var checker = new TestAvailabilityCheck();

            checker.CurrentAvailable = true;

            var probe = new TestAvailabilityProbe(IPAddress.Any, 10034, TimeSpan.FromMilliseconds(1), checker);
            var stopSuccess = false;
            bool? secondStart = null;
            var startSuccess = probe.StartAsync(CancellationToken.None).GetAwaiter().GetResult();

            if (startSuccess)
            {
                secondStart = probe.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
                Task.Delay(2000).GetAwaiter().GetResult();
                stopSuccess = probe.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            }

            Assert.IsTrue(startSuccess, "Start was not successful");
            Assert.IsTrue(stopSuccess, "Stop was not successful");
            Assert.IsTrue(secondStart.HasValue, "Second start should have a value");
            Assert.IsFalse(secondStart.Value, "Second start should be false");
        }

        /// <summary>
        /// Creates a TCP health Probe, starts it, starts it again and stops it then stops it again
        /// </summary>
        [TestMethod]
        public void DoubleStartDoubleStop()
        {
            var checker = new TestAvailabilityCheck();

            checker.CurrentAvailable = true;

            var probe = new TestAvailabilityProbe(IPAddress.Any, 10034, TimeSpan.FromMilliseconds(1), checker);
            var stopSuccess = false;
            var secondStopSuccess = false;
            bool secondStart = false;
            var startSuccess = probe.StartAsync(CancellationToken.None).GetAwaiter().GetResult();

            if (startSuccess)
            {
                secondStart = probe.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
                Task.Delay(2000).GetAwaiter().GetResult();
                stopSuccess = probe.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
                secondStopSuccess = probe.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            }

            Assert.IsTrue(startSuccess, "Start was not successful");
            Assert.IsTrue(stopSuccess, "Stop was not successful");
            Assert.IsFalse(secondStart, "Second start should be false");
            Assert.IsFalse(secondStopSuccess, "Second stop should be false");
        }

        /// <summary>
        /// Creates a TCP Probe, starts it, connects the desired number of times and stops it
        /// </summary>
        /// <param name="connectionCount">The number of connections to attempt</param>
        /// <param name="interval">The delay between each attempt in milliseconds</param>
        [DataTestMethod]
        [DataRow(1, 0)]
        [DataRow(10, 10)]
        [DataRow(50, 10)]
        [DataRow(100, 10)]
        [DataRow(1000, 10)]
        public void ClientConnect(int connectionCount, int interval)
        {
            const int Port = 10034;

            var checker = new TestAvailabilityCheck();

            checker.CurrentAvailable = true;

            var probe = new TestAvailabilityProbe(IPAddress.Any, 10034, TimeSpan.FromMilliseconds(1), checker);
            var stopSuccess = false;
            var startSuccess = probe.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
            var statusList = new List<bool>();

            if (startSuccess)
            {
                var continueListening = true;
                var maximumTime = connectionCount * (interval + 10);
                var timeoutAt = DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(maximumTime + 1000));

                var collectionThread = new Thread(new ThreadStart(() =>
                {


                    while (continueListening && statusList.Count < connectionCount)
                    {
                        var success = false;

                        try
                        {
                            using (var client = new TcpClient())
                            {
                                client.Connect(IPAddress.Loopback, Port);
                                success = true;
                                client.Close();
                            }
                        }
                        catch (Exception)
                        {

                        }

                        statusList.Add(success);

                        if (statusList.Count < connectionCount)
                        {
                            Thread.Sleep(interval);
                        }
                    }
                }));

                collectionThread.Start();

                while (DateTime.UtcNow < timeoutAt && collectionThread.IsAlive)
                {
                    Task.Delay(1000).GetAwaiter().GetResult();
                }

                continueListening = false;

                stopSuccess = probe.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            }

            Assert.IsTrue(startSuccess, "Start was not successful");
            Assert.IsTrue(stopSuccess, "Stop was not successful");
            Assert.IsTrue(connectionCount == statusList.Count(), "Not enough connections were performed");
            Assert.IsFalse(statusList.Any(item => !item), "Failure to connect found");
        }

        ///// <summary>
        ///// Creates a TCP Probe, starts it, connects the desired number of times and stops it
        ///// </summary>
        ///// <param name="connectionCount">The number of connections to attempt</param>
        ///// <param name="disconnectAfter">The number of connections after which to stop listening</param>
        ///// <param name="interval">The delay between each attempt in milliseconds</param>
        //[DataTestMethod]
        //[DataRow(1, 1, 100)]
        //[DataRow(3, 1, 100)]
        //[DataRow(10, 5, 100)]
        //[DataRow(20, 10, 100)]
        //public void ClientConnectStop(int connectionCount, int disconnectAfter, int interval)
        //{
        //    const int Port = 10034;

        //    var checker = new TestHealthCheck();

        //    checker.CurrentHealthy = true;

        //    var probe = new TestHealthProbe(IPAddress.Any, Port, TimeSpan.FromMilliseconds(1), checker);
        //    var stopSuccess = false;
        //    var startSuccess = probe.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        //    var statusList = new List<bool>();

        //    if (startSuccess)
        //    {
        //        var continueListening = true;
        //        var timeoutAt = DateTime.UtcNow.Add(TimeSpan.FromMinutes(5));

        //        var collectionThread = new Thread(new ThreadStart(() =>
        //        {
        //            var endpoint = new IPEndPoint(IPAddress.Loopback, Port);

        //            while (continueListening && statusList.Count < connectionCount)
        //            {
        //                var success = false;

        //                try
        //                {
        //                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        //                    {
        //                        socket.NoDelay = true;
        //                        socket.LingerState = new LingerOption(true, 0);

        //                        var connectionTask = socket.ConnectAsync(endpoint);
        //                        var completedTask = Task.WhenAny(connectionTask, Task.Delay(50)).GetAwaiter().GetResult();

        //                        if (connectionTask.IsCompleted)
        //                        {
        //                            success = true;
        //                        }

        //                        socket.Disconnect(false);
        //                        socket.Close();
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    var abc = e.Message;
        //                }

        //                statusList.Add(success);

        //                if (statusList.Count >= disconnectAfter)
        //                {
        //                    checker.CurrentHealthy = false;
        //                }

        //                if (statusList.Count >= connectionCount)
        //                {
        //                    continueListening = false;
        //                }
        //                else
        //                {
        //                    Thread.Sleep(interval);
        //                }
        //            }
        //        }));

        //        collectionThread.Start();

        //        while (DateTime.UtcNow < timeoutAt & continueListening && collectionThread.IsAlive)
        //        {
        //            Task.Delay(1000).GetAwaiter().GetResult();
        //        }

        //        continueListening = false;
        //        collectionThread.Join();

        //        stopSuccess = probe.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
        //    }

        //    Assert.IsTrue(startSuccess, "Start was not successful");
        //    Assert.IsTrue(connectionCount == statusList.Count(), "Not enough connections were performed");

        //    var successCount = statusList.Count(item => item);
        //    Assert.IsTrue(successCount >= disconnectAfter - 1 && successCount <= disconnectAfter + 1, "Success count does not match the disconnect after");
        //}
        #endregion
    }

    #region TestAvailabilityProbe Definition
    /// <summary>
    /// A availability probe that counts invocations of specialized events
    /// </summary>
    public class TestAvailabilityProbe : AvailabilityContainerProbe
    {
        #region Variables
        /// <summary>
        /// The number of times the probe received hander is invoked
        /// </summary>
        private long _probeReceived = 0;

        /// <summary>
        /// The number of times the probe processed handler is invoked
        /// </summary>
        private long _probeProcessed = 0;

        /// <summary>
        /// The number of times the error handler is invoked
        /// </summary>
        private long _errorCount = 0;
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="port">The port to listen on</param>
        /// <param name="pollingInterval">The interval between checking for new connection requests</param>
        /// <param name="handler">A handler used to determine if the service is available</param>
        public TestAvailabilityProbe(ushort port, TimeSpan pollingInterval, IAvailabilityCheck handler) : base(port, pollingInterval, handler)
        {

        }

        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="port">The port to listen on</param>
        /// <param name="pollingInterval">The interval between checking for new connection requests</param>
        /// <param name="handler">A handler used to determine if the service is available</param>
        public TestAvailabilityProbe(IPAddress address, ushort port, TimeSpan pollingInterval, IAvailabilityCheck handler) : base(address, port, pollingInterval, handler)
        {

        }
        #endregion
        #region Properties
        /// <summary>
        /// The number of times the probe received hander is invoked
        /// </summary>
        public long ProbeReceivedCount => _probeReceived;

        /// <summary>
        /// The number of times the probe processed handler is invoked
        /// </summary>
        public long ProbeProcessedCount => _probeProcessed;

        /// <summary>
        /// The number of times the error handler is invoked
        /// </summary>
        public long ErrorCount => _errorCount;
        #endregion
        #region Methods
        /// <inheritdoc />
        protected override void ProcessingErrorHandler(Exception exception, bool terminating, bool starting)
        {
            Interlocked.Increment(ref _errorCount);
        }

        /// <inheritdoc />
        protected override void ProbeProcessed()
        {
            Interlocked.Increment(ref _probeProcessed);
        }

        /// <inheritdoc />
        protected override void ProbeReceived()
        {
            Interlocked.Increment(ref _probeReceived);
        }
        #endregion
    }

    /// <summary>
    /// A test availability check that the consumer controls the value of
    /// </summary>
    public class TestAvailabilityCheck : IAvailabilityCheck
    {
        #region Properties
        /// <summary>
        /// Set by the consumer to indicate the availability response
        /// </summary>
        public bool CurrentAvailable { get; set; }

        #endregion
        #region Methods
        /// <inheritdoc />
        public Task<bool> IsAvailableAsync()
        {
            return Task.FromResult(CurrentAvailable);
        }
        #endregion
    }
    #endregion
}
