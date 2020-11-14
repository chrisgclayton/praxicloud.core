// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.containers
{
    #region Using Clauses
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// A container probe that response to probes indicating whether a service is healthy or available depending on probe type
    /// </summary>
    public interface IContainerProbe
    {
        #region Properties
        /// <summary>
        /// A task that can be monitored to determine when the probe completes
        /// </summary>
        Task Task { get; }
        #endregion
        #region Methods
        /// <summary>
        /// Starts th probe
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        /// <returns>True if started successfully</returns>
        Task<bool> StartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Stops th probe
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for abort requests</param>
        /// <returns>True if stopped successfully</returns>
        Task<bool> StopAsync(CancellationToken cancellationToken);
        #endregion

    }
}
