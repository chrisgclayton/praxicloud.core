// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.exceptions
{
    /// <summary>
    /// Indicates the type of the unobserved or unhandled exception being raised
    /// </summary>
    public enum UnobservedType : byte
    {
        /// <summary>
        /// App domain unhandled exception
        /// </summary>
        AppDomain = 0,

        /// <summary>
        /// Task Scheduler unobserved exception
        /// </summary>
        TaskScheduler = 1
    }
}