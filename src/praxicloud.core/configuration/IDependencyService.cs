// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.configuration
{
    #region Using Clauses
    using Microsoft.Extensions.DependencyInjection;
    #endregion

    /// <summary>
    /// An instance that exposes the dependency injection service for instantiated types to receive from the constructor 
    /// </summary>
    public interface IDependencyService
    {
        #region Properties
        /// <summary>
        /// The dependency injection service provider
        /// </summary>
        ServiceProvider Services { get; }
        #endregion
    }
}
