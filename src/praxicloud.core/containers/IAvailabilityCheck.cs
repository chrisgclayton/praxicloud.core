// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.containers
{
    #region Using Clauses
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// An interface defining an instance responsible for reporting on service availability
    /// </summary>
    public interface IAvailabilityCheck
    {
        #region Properties
        /// <summary>
        /// Determines if the service is available
        /// </summary>
        /// <returns>Returns true if available</returns>
        Task<bool> IsAvailableAsync();
        #endregion
    }
}
