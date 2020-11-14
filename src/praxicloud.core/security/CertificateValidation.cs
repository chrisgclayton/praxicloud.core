// Copyright (c) Christopher Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.security
{
    using System.Diagnostics.CodeAnalysis;
    #region Using Clauses
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    #endregion

    /// <summary>
    /// Methods to handle validation of certificates through the remote certificate callback handlers
    /// </summary>
    public static class CertificateValidation
    {
        #region Methods
        /// <summary>
        /// This certificate validation callback is a useful one for allowing self signed certificates
        /// </summary>
        /// <param name="sender">The calling object</param>
        /// <param name="certificate">The X509 certificate to be validated</param>
        /// <param name="chain">The associated certificate chain</param>
        /// <param name="sslPolicyErrors">A list of ssl policy errors</param>
        /// <returns>True if the certificate is valid, false otherwise</returns>
        public static bool CertificateValidationCallBackAllowsSelfSigned(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isValid = false;

            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                isValid = true;
            }
            else
            {
                // If there are errors in the certificate chain, look at each error to determine the cause.
                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                {
                    if (chain != null)
                    {
                        bool isChainErrorFound = false;

                        foreach (X509ChainStatus status in chain.ChainStatus)
                        {                            
                            if (!((certificate.Subject == certificate.Issuer) && (status.Status == X509ChainStatusFlags.UntrustedRoot)) && (status.Status != X509ChainStatusFlags.NoError))
                            {
                                isChainErrorFound = true;
                            }
                        }

                        isValid = !isChainErrorFound;
                    }
                    else
                    {
                        // When processing reaches this line, the only errors in the certificate chain are 
                        // untrusted root errors for self-signed certificates. These certificates are valid
                        // for default Exchange server installations, so return true.
                        isValid = true;
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// This certificate validation callback is a useful one for allowing self signed certificates
        /// </summary>
        /// <param name="sender">The calling object</param>
        /// <param name="certificate">The X509 certificate to be validated</param>
        /// <param name="chain">The associated certificate chain</param>
        /// <param name="sslPolicyErrors">A list of ssl policy errors</param>
        /// <returns>True if the certificate is valid, false otherwise</returns>
        public static bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }
        #endregion
    }
}
