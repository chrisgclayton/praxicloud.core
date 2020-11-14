// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.security
{
    #region Using Clauses
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    #endregion

    /// <summary>
    /// Contains common secure string handling methods.
    /// </summary>
    public static class SecureStringUtilities
    {
        #region Public Methods
        /// <summary>
        /// Creates a secure string based on the contents of the provided string.
        /// </summary>
        /// <param name="value">The string holding the contents of the secure string.</param>
        /// <returns>A populated secure string that contains the value specified in the string parameter.</returns>
        public static SecureString GetSecureString(this string value)
        {
            Guard.NotNull(nameof(value), value);

            var results = new SecureString();

            foreach (var character in value)
            {
                results.AppendChar(character);
            }

            results.MakeReadOnly();

            return results;
        }

        /// <summary>
        /// Creates a CLR string based on the provided secure string value.
        /// </summary>
        /// <param name="value">The secure string that contains the value to be placed in the CLR string.</param>
        /// <returns></returns>
        public static string SecureStringToString(this SecureString value)
        {
            Guard.NotNull(nameof(value), value);

            string plainValue = null;

            if (value != null)
            {
                // Replace with a safe handle in the future when the attribute is present
                var handle = IntPtr.Zero;

                try
                {
                    handle = SecureStringMarshal.SecureStringToGlobalAllocUnicode(value);
                    plainValue = Marshal.PtrToStringUni(handle);
                }
                finally
                {
                    if (handle != IntPtr.Zero) Marshal.ZeroFreeGlobalAllocUnicode(handle);
                }
            }

            return plainValue;
        }
        #endregion
    }
}
