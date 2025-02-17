﻿using System.Security.Cryptography;

namespace ARWNI2S.Security.Cryptography
{
    /// <summary>
    /// Hash helper class
    /// </summary>
    public partial class HashHelper
    {
        /// <summary>
        /// Create a data hash
        /// </summary>
        /// <param name="data">The data for calculating the hash</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <param name="trimByteCount">The number of bytes, which will be used in the hash algorithm; leave 0 to use all array</param>
        /// <returns>Data hash</returns>
        public static string CreateHash(byte[] data, string hashAlgorithm, int trimByteCount = 0)
        {
            if (string.IsNullOrEmpty(hashAlgorithm))
                throw new ArgumentNullException(nameof(hashAlgorithm));

            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlgorithm) ?? throw new ArgumentException("Unrecognized hash name");
            if (trimByteCount > 0 && data.Length > trimByteCount)
            {
                var newData = new byte[trimByteCount];
                Array.Copy(data, newData, trimByteCount);

                return Convert.ToHexString(algorithm.ComputeHash(newData));
            }

            return Convert.ToHexString(algorithm.ComputeHash(data));
        }
    }
}
