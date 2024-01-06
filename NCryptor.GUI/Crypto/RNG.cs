using System;
using System.Security.Cryptography;

namespace NCryptor.GUI.Crypto
{
    internal class RNG
    {
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
