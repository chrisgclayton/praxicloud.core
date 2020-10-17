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
    #endregion

    /// <summary>
    /// Tests the TCP Probe functionality
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TcpProbeEndpointTests
    {
        #region Test Methods
        /// <summary>
        /// Creates a TCP Probe, starts it and stops it
        /// </summary>
        [TestMethod]
        public void StartStop()
        {
            var probe = new TcpProbeEndpoint(IPAddress.Any, 10034, null, null);
            var stopSuccess = false;
            var startSuccess = probe.StartAsync(CancellationToken.None).GetAwaiter().GetResult();

            if(startSuccess)
            {
                Task.Delay(2000).GetAwaiter().GetResult();
                stopSuccess = probe.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            }

            Assert.IsTrue(startSuccess, "Start was not successful");
            Assert.IsTrue(stopSuccess, "Stop was not successful");
        }

        /// <summary>
        /// Creates a TCP Probe, starts it, starts it again and stops it
        /// </summary>
        [TestMethod]
        public void DoubleStartStop()
        {
            var probe = new TcpProbeEndpoint(IPAddress.Any, 10035, null, null);
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
        /// Creates a TCP Probe, starts it, starts it again and stops it then stops it again
        /// </summary>
        [TestMethod]
        public void DoubleStartDoubleStop()
        {
            var probe = new TcpProbeEndpoint(IPAddress.Any, 10036, null, null);
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

            var probe = new TcpProbeEndpoint(IPAddress.Any, Port, null, null);
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
                        catch(Exception)
                        {

                        }

                        statusList.Add(success);

                        if(statusList.Count < connectionCount)
                        {
                            Thread.Sleep(interval);
                        }
                    }
                }));

                collectionThread.Start();

                while(DateTime.UtcNow < timeoutAt && collectionThread.IsAlive)
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

        //    var probe = new TcpProbeEndpoint(IPAddress.Any, Port, null, null);
        //    var stopSuccess = false;
        //    var startSuccess = probe.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        //    var statusList = new List<bool>();

        //    if (startSuccess)
        //    {
        //        var continueListening = true;
        //        var maximumTime = connectionCount * (interval + 10);
        //        var timeoutAt = DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(maximumTime + 1000));

        //        var collectionThread = new Thread(new ThreadStart(() =>
        //        {
        //            var endpoint = new IPEndPoint(IPAddress.Loopback, Port);

        //            while (continueListening && statusList.Count < connectionCount)
        //            {
        //                var success = false;

        //                try
        //                {
        //                    using(var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        //                    {
        //                        socket.NoDelay = true;
        //                        socket.LingerState = new LingerOption(true, 0);
                                
        //                        var connectionTask = socket.ConnectAsync(endpoint);
        //                        var completedTask = Task.WhenAny(connectionTask, Task.Delay(50)).GetAwaiter().GetResult();

        //                        if(connectionTask.IsCompleted)
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

        //                if(statusList.Count >= disconnectAfter)
        //                {
        //                    probe.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
        //                }

        //                if (statusList.Count < connectionCount)
        //                {
        //                    Thread.Sleep(interval);
        //                }
        //            }
        //        }));

        //        collectionThread.Start();

        //        while (DateTime.UtcNow < timeoutAt && collectionThread.IsAlive)
        //        {
        //            Task.Delay(1000).GetAwaiter().GetResult();
        //        }

        //        continueListening = false;

        //        stopSuccess = probe.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
        //    }

        //    Assert.IsTrue(startSuccess, "Start was not successful");
        //    Assert.IsTrue(connectionCount == statusList.Count(), "Not enough connections were performed");
        //    Assert.IsTrue(statusList.Count(item => item) == disconnectAfter, "Success count does not match the disconnect after");
        //}
        #endregion
    }
}
