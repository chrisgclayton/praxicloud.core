// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.containers
{
    #region Using Clauses
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// An interface that is responsible for reporting on service health
    /// </summary>
    public interface IHealthCheck
    {
        #region Properties
        /// <summary>
        /// Checks if the service is healthy
        /// </summary>
        /// <returns>Returns true if the service is healthy</returns>
        Task<bool> IsHealthyAsync();
        #endregion
    }
}
