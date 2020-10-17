// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.containers
{
    #region Using Clauses
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Nito.AsyncEx;
    using praxicloud.core.security;
    #endregion

    /// <summary>
    /// The TCP endpoints that listen for probe requests
    /// </summary>
    public class TcpProbeEndpoint
    {
        #region Delegates
        /// <summary>
        /// A delegate invoked when a probe request is received
        /// </summary>
        public delegate void ProbeReceivedHandler();

        /// <summary>
        /// A delegate invoked when a probe request is processed successfully
        /// </summary>
        public delegate void ProbeProcessedHandler();
        #endregion
        #region Variables
        /// <summary>
        /// Listens for connectivity
        /// </summary>
        private TcpListener _listener;

        /// <summary>
        /// Controls access to the lifecycle methods
        /// </summary>
        private readonly AsyncLock _control = new AsyncLock();

        /// <summary>
        /// A completion source that is created when the listener starts and completes when it stops
        /// </summary>
        private TaskCompletionSource<bool> _completionSource = null;

        /// <summary>
        /// The IP Address to listen on
        /// </summary>
        private readonly IPAddress _address;

        /// <summary>
        /// The port number to listen on all IPv4 endpoints
        /// </summary>
        private readonly ushort _port;

        /// <summary>
        /// True if the processor should continue processing
        /// </summary>
        private bool _continueProcessing = false;

        /// <summary>
        /// The task representing the processing method
        /// </summary>
        private Task _processingTask = null;

        /// <summary>
        /// The handler that is invoked when a connection is received
        /// </summary>
        private readonly ProbeReceivedHandler _receivedHandler;

        /// <summary>
        /// The handler that is invoked when a connection is processed successfully
        /// </summary>
        private readonly ProbeProcessedHandler _processedHandler;

        #endregion
        #region Constructors
        /// <summary>
        /// Initialize new instance of the type
        /// </summary>
        /// <param name="port">The port number to listen on all IPv4 endpoints</param>
        /// <param name="processedHandler">The handler that is invoked when a connection is received</param>
        /// <param name="receivedHandler">The handler that is invoked when a connection is processed successfully</param>
        public TcpProbeEndpoint(ushort port, ProbeReceivedHandler receivedHandler, ProbeProcessedHandler processedHandler) : this(IPAddress.Any, port, receivedHandler, processedHandler)
        {
        }

        /// <summary>
        /// Initialize new instance of the type
        /// </summary>
        /// <param name="address">The IP Address to listen on</param>
        /// <param name="port">The port number to listen on all IPv4 endpoints</param>
        /// <param name="processedHandler">The handler that is invoked when a connection is received</param>
        /// <param name="receivedHandler">The handler that is invoked when a connection is processed successfully</param>
        public TcpProbeEndpoint(IPAddress address, ushort port, ProbeReceivedHandler receivedHandler, ProbeProcessedHandler processedHandler)
        {
            Guard.NotLessThan(nameof(port), port, 1);

            _address = address;
            _port = port;
            _receivedHandler = receivedHandler;
            _processedHandler = processedHandler;
        }
        #endregion
        #region Properties
        /// <summary>
        /// A task that can be monitored to determine when the probe completes
        /// </summary>
        public Task Task => _completionSource == null ? Task.CompletedTask : _completionSource.Task;
        #endregion
        #region Methods
        /// <summary>
        /// Starts th probe
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        /// <returns>True if started successfully</returns>
        public async Task<bool> StartAsync(CancellationToken cancellationToken)
        {
            var success = false;

            if(Task.IsCompleted)
            {
                using (await _control.LockAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (Task.IsCompleted)
                    {
                        if (_listener != null)
                        {
                            if (_processingTask == null)
                            {
                                await _processingTask.ConfigureAwait(false);
                                _processingTask = null;
                            }

                            _listener.Stop();
                        }

                        _listener = new TcpListener(_address, _port);
                        _listener.Start(100);
                        _processingTask = ProcessAsync();

                        success = true;
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Stops th probe
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        /// <returns>True if stopped successfully</returns>
        public async Task<bool> StopAsync(CancellationToken cancellationToken)
        {
            var success = false;

            if (!Task.IsCompleted)
            {
                using (await _control.LockAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (!Task.IsCompleted)
                    {
                        _continueProcessing = false;
                        if (_processingTask != null) await _processingTask.ConfigureAwait(false);

                        if (_listener != null)
                        {
                            _listener.Stop();
                            _listener = null;

                            success = true;
                        }
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// A processor to accept clients
        /// </summary>
        private async Task ProcessAsync()
        {
            var completionSource = new TaskCompletionSource<bool>(); 

            _completionSource = completionSource;
            _continueProcessing = true;

            try
            {
                while (_continueProcessing)
                {
                    while(_listener.Pending())
                    {
                        ProbeReceived();

                        using (var client = _listener.AcceptTcpClient())
                        {
                            ProbeProcessed();
                            client.Close();
                        }
                    }

                    await Task.Delay(100).ConfigureAwait(false);                    
                }
            }
            catch(Exception)
            {
                _continueProcessing = false;
            }

            _completionSource.TrySetResult(true);
        }

        /// <summary>
        /// Invoked when a probe request is received
        /// </summary>
        protected virtual void ProbeReceived()
        {
            _receivedHandler?.Invoke();
        }

        /// <summary>
        /// Invoked when a probe request is successfully processed
        /// </summary>
        protected virtual void ProbeProcessed()
        {
            _processedHandler?.Invoke();
        }
        #endregion
    }
}
