// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.containers
{
    #region Using Clauses
    using praxicloud.core.security;
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Nito.AsyncEx;
    using static praxicloud.core.containers.TcpProbeEndpoint;
    #endregion

    /// <summary>
    /// A container probe that response to TCP requests when healthy or available depending on probe type
    /// </summary>
    public abstract class ContainerProbe : IContainerProbe
    {
        #region Variables
        /// <summary>
        /// The Tcp endpoint that will listen for new requests
        /// </summary>
        private TcpProbeEndpoint _endpoint;

        /// <summary>
        /// The interval between polling checks
        /// </summary>
        private readonly TimeSpan _pollInterval;

        /// <summary>
        /// Controls access to the lifecycle methods
        /// </summary>
        private readonly AsyncLock _control = new AsyncLock();

        /// <summary>
        /// A completion source that is created when the listener starts and completes when it stops
        /// </summary>
        private TaskCompletionSource<bool> _completionSource = null;

        /// <summary>
        /// True if the processor should continue processing
        /// </summary>
        private bool _continueProcessing = false;

        /// <summary>
        /// The task representing the processing method
        /// </summary>
        private Task _processingTask = null;

        /// <summary>
        /// The IP Address to listen on
        /// </summary>
        private readonly IPAddress _address;

        /// <summary>
        /// The port number to listen on all IPv4 endpoints
        /// </summary>
        private readonly ushort _port;
        #endregion
        #region Constructors
        /// <summary>
        /// Initialize new instance of the type
        /// </summary>
        /// <param name="port">The port number to listen on all IPv4 endpoints</param>
        /// <param name="pollingInterval">The interval between polling checks</param>
        public ContainerProbe(ushort port, TimeSpan pollingInterval) : this(IPAddress.Any, port, pollingInterval)
        {
        }

        /// <summary>
        /// Initialize new instance of the type
        /// </summary>
        /// <param name="address">The IP Address to listen on</param>
        /// <param name="port">The port number to listen on all IPv4 endpoints</param>
        /// <param name="pollingInterval">The interval between polling checks</param>
        public ContainerProbe(IPAddress address, ushort port, TimeSpan pollingInterval)
        {
            Guard.NotLessThan(nameof(port), port, 1);
            Guard.NotLessThan(nameof(pollingInterval), pollingInterval, TimeSpan.FromMilliseconds(1));

            _address = address;
            _port = port;
            _pollInterval = pollingInterval;
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
        /// Validates the status of the probe
        /// </summary>
        /// <returns>Success if available</returns>
        abstract public Task<bool> ValidationHandlerAsync();

        /// <summary>
        /// Starts th probe
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        /// <returns>True if started successfully</returns>
        public async Task<bool> StartAsync(CancellationToken cancellationToken)
        {
            var success = false;

            if (Task.IsCompleted)
            {
                using (await _control.LockAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (Task.IsCompleted)
                    {
                        if (_endpoint != null)
                        {
                            try
                            {
                                if (_processingTask == null)
                                {
                                    await _processingTask.ConfigureAwait(false);
                                    _processingTask = null;
                                }
                            }
                            catch(Exception)
                            {
                                // Suppress errors during cleanup
                            }

                            try
                            {
                                await _endpoint.StopAsync(cancellationToken).ConfigureAwait(false);
                            }
                            catch(Exception)
                            {
                                // Suppress errors to do with stopping the current listener
                            }

                            _endpoint = null;
                        }

                        try
                        {
                            if (_processingTask != null)
                            {
                                _continueProcessing = false;
                                await _processingTask.ConfigureAwait(false);
                                _processingTask = null;
                            }
                        }
                        catch(Exception)
                        {
                            // Suppress exceptions while letting the processor shutdown
                        }

                        _endpoint = new TcpProbeEndpoint(_address, _port, ProbeReceived, ProbeProcessed);
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

                        try
                        {
                            if (_processingTask != null)
                            {
                                await _processingTask.ConfigureAwait(false);
                                _processingTask = null;
                            }
                        }
                        catch (Exception)
                        {
                            // Suppress exceptions while allowing the processing task to complete
                        }
                    }

                    try
                    {
                        if (_endpoint != null)
                        {
                            await _endpoint.StopAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }
                    catch (Exception)
                    {
                        // Suppress exceptions with stopping the endpoint
                    }

                    _endpoint = null;
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// A method that can be overridden to handle error conditions that are raised
        /// </summary>
        /// <param name="exception">The exception that occurred</param>
        /// <param name="terminating">True if the exception is resulting in the accepting of connections being stopped</param>
        /// <param name="starting">True if it occurred while starting the listener (true starting, false stopping)</param>
        protected virtual void ProcessingErrorHandler(Exception exception, bool terminating, bool starting)
        {

        }

        /// <summary>
        /// A delegate invoked when a probe request is received
        /// </summary>
        protected virtual void ProbeReceived()
        {

        }

        /// <summary>
        /// A delegate invoked when a probe request is processed successfully
        /// </summary>
        protected virtual void ProbeProcessed()
        {

        }


        /// <summary>
        /// A processor to accept clients
        /// </summary>
        private async Task ProcessAsync()
        {
            var completionSource = new TaskCompletionSource<bool>();
            var isRunning = false;

            _completionSource = completionSource;
            _continueProcessing = true;

            try
            {
                _completionSource = new TaskCompletionSource<bool>();

                while (_continueProcessing)
                {
                    var handlerSuccess = await ValidationHandlerAsync().ConfigureAwait(false);

                    // Check for state here
                    if (!isRunning && handlerSuccess)
                    {
                        try
                        {
                            if (await _endpoint.StartAsync(CancellationToken.None).ConfigureAwait(false))
                            {
                                isRunning = true;
                            }
                        }
                        catch(Exception e)
                        {
                            ProcessingErrorHandler(e, false, true);
                        }

                    }
                    else if(isRunning && !handlerSuccess)
                    {
                        try
                        {
                            if (await _endpoint.StopAsync(CancellationToken.None).ConfigureAwait(false))
                            {
                                isRunning = false;
                            }
                        }
                        catch(Exception e)
                        {
                            ProcessingErrorHandler(e, false, false);
                        }
                    }                                       
                    
                    var sleepUntil = DateTime.UtcNow.Add(_pollInterval);

                    while(DateTime.UtcNow < sleepUntil && _continueProcessing)
                    {
                        await Task.Delay(100).ConfigureAwait(false);
                    }                    
                }

                _completionSource.TrySetResult(true);
            }
            catch (Exception e)
            {
                _continueProcessing = false;
                ProcessingErrorHandler(e, true, false);
            }

            _completionSource.TrySetResult(true);
        }
        #endregion
    }
}
