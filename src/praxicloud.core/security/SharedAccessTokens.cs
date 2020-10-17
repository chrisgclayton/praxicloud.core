// Copyright (c) Chris Clayton. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace praxicloud.core.security
{
    #region Using Clauses
    using System;
    using System.Globalization;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    #endregion

    /// <summary>
    /// A simple shared access signature helper type
    /// </summary>
    public static class SharedAccessTokens
    {
        #region Constants
        /// <summary>
        /// The formatting of the shared access signature string
        /// </summary>
        private const string SharedAccessSignatureFormat = "SharedAccessSignature sr={0}&sig={1}&se={2}";

        /// <summary>
        /// The delimeter between the shared access slignature and expiry details
        /// </summary>
        private const string ExpiryDelimeter = "\n";

        /// <summary>
        /// The key name if provided
        /// </summary>
        private const string PolicyDelimeter = "&skn=";
        #endregion
        #region Methods
        /// <summary>
        /// Create a SAS token for the URI based on the specified policy
        /// </summary>
        /// <param name="resourceUri">The URI of the resource to access</param>
        /// <param name="key">The shared access signature to encode the string with</param>
        /// <param name="policyName">The name of the policy to create the shared access policy with</param>
        /// <param name="expiryInSeconds">The duration the token is valid for</param>
        /// <returns>A string representation of the shared access signature</returns>
        public static string GenerateSasToken(string resourceUri, string key, string policyName, int expiryInSeconds)
        {
            var secondsFromEpoch = (int)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
            string expiry = Convert.ToString(secondsFromEpoch + expiryInSeconds);
            string stringToSign = WebUtility.UrlEncode(resourceUri) + ExpiryDelimeter + expiry;

            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key));
            string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            string token = string.Format(CultureInfo.InvariantCulture, SharedAccessSignatureFormat, WebUtility.UrlEncode(resourceUri), WebUtility.UrlEncode(signature), expiry);

            if (!string.IsNullOrEmpty(policyName))
            {
                token += $"{PolicyDelimeter}{policyName}";
            }

            return token;
        }

        public static bool DecomposeSasToken(string accessToken, out string resourceUri, out string policyName, out DateTime expiresAt, out string stringToValidate, out byte[] signature)
        {
            const string Prefix = "SharedAccessSignature ";

            var resourceUriFound = false;
            var policyNameFound = false;
            var expiresAtFound = false;
            var signatureFound = false;

            var additionalElements = false;
            var token = accessToken.Trim();
            string policyNameText = null;
            DateTime expiryTime = DateTime.MinValue;
            string resourceText = null;
            byte[] sourceSignature = null;
            string resourceToValidate;
            int expirationToValidate = 0;

            if (token.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring(Prefix.Length).Trim();

                var tokenElements = token.Split("&", StringSplitOptions.RemoveEmptyEntries);

                foreach (var element in tokenElements)
                {
                    if (element.StartsWith("sr=", StringComparison.OrdinalIgnoreCase))
                    {
                        resourceUriFound = true;
                        resourceToValidate = element.Substring("sr=".Length).Trim();
                        resourceText = WebUtility.UrlDecode(resourceToValidate);
                    }
                    else if (element.StartsWith("sig=", StringComparison.OrdinalIgnoreCase))
                    {
                        signatureFound = true;
                        sourceSignature = Convert.FromBase64String(WebUtility.UrlDecode(element.Substring("sig=".Length).Trim()));
                    }
                    else if (element.StartsWith("se=", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(element.Substring("se=".Length).Trim(), out expirationToValidate))
                        {
                            expiresAtFound = true;
                            expiryTime = DateTime.UnixEpoch.AddSeconds(expirationToValidate);
                        }
                    }
                    else if (element.StartsWith("skn=", StringComparison.OrdinalIgnoreCase))
                    {
                        policyNameFound = true;
                        policyNameText = element.Substring("skn=".Length).Trim();
                    }
                    else
                    {
                        additionalElements = true;
                    }
                }
            }

            resourceUri = resourceText;
            policyName = policyNameText;
            expiresAt = expiryTime;
            signature = sourceSignature;

            stringToValidate = WebUtility.UrlEncode(resourceUri) + ExpiryDelimeter + expirationToValidate;


            return resourceUriFound && policyNameFound && expiresAtFound && signatureFound && !additionalElements;
        }

        public static bool IsSignatureValid(byte[] signature, string key, string stringToValidate)
        {
            var valid = false;

            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(stringToValidate) && (signature?.Length ?? 0) > 0)
            {
                HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key));
                byte[] computedSignature = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToValidate));

                if (computedSignature.Length == (signature.Length))
                {
                    var differenceFound = false;

                    for (var index = 0; index < signature.Length && !differenceFound; index++)
                    {
                        differenceFound = computedSignature[index] != signature[index];
                    }

                    valid = !differenceFound;
                }
            }
            return valid;
        }

        #endregion
    }
}


