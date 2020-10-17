// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.configuration
{
    #region Using Clauses
    using System;
    #endregion

    /// <summary>
    /// An attribute that can be decorated on properties of an object to be sure that the dump method in the service will not
    /// output it to the logs (security sensitive data etc. should be decorated with this)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DoNotOutputAttribute : Attribute
    {
    }
}