using System;
using System.Security.Cryptography;

namespace NCryptor.GUI.Crypto
{
    /// <summary>
    /// Contains methods for generating cryptographically strong random numbers.
    /// </summary>
    internal class RNG
    {
        /// <exception cref="ArgumentOutOfRangeException"/>
        internal static byte[] GenRandomBytes(int size)
        {
            if(size <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var bytes = new byte[size];

            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return bytes;
        }
    }
}
