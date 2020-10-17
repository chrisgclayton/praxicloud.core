// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.containers
{
    #region Using Clauses
    using praxicloud.core.security;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// A container probe that response to TCP requests when available 
    /// </summary>
    public class AvailabilityContainerProbe : ContainerProbe, IAvailabilityContainerProbe
    {
        #region Variables
        /// <summary>
        /// Checks the availability status
        /// </summary>
        private readonly IAvailabilityCheck _handler;
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="address">The IP Address to listen on</param>
        /// <param name="port">The port number to listen on all IPv4 endpoints</param>
        /// <param name="handler">The validation method</param>
        /// <param name="pollingInterval">The interval between polling checks</param>
        public AvailabilityContainerProbe(IPAddress address, ushort port, TimeSpan pollingInterval, IAvailabilityCheck handler) : base(address, port, pollingInterval)
        {
            Guard.NotNull(nameof(handler), handler);

            _handler = handler;
        }

        /// <summary>
        /// Initializes a new instance of the type
        /// </summary>
        /// <param name="port">The port number to listen on all IPv4 endpoints</param>
        /// <param name="handler">The validation method</param>
        /// <param name="pollingInterval">The interval between polling checks</param>
        public AvailabilityContainerProbe(ushort port, TimeSpan pollingInterval, IAvailabilityCheck handler) : this(IPAddress.Any, port, pollingInterval, handler)
        {
        }
        #endregion
        #region Methods
        /// <inheritdoc />
        public sealed override Task<bool> ValidationHandlerAsync()
        {
            return _handler.IsAvailableAsync();
        }
        #endregion
    }
}
