﻿using System;
using System.Linq;
using System.Security.Cryptography;

namespace NCryptor.GUI.Crypto
{
    /// <summary>
    /// Contains static methods for deriving keys and verification tags.
    /// </summary>
    internal class KeyDerivation
    {
        private KeyDerivation() { }

        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        internal static byte[] DeriveKeyPbkdf2(byte[] password, byte[] salt, int size, int iterations)
        {
            if(password is null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if(salt is null)
            {
                throw new ArgumentNullException(nameof(salt));
            }

            if(size < 32)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            if(iterations < 25000)
            {
                throw new ArgumentOutOfRangeException(nameof(iterations));
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512))
            {
                return pbkdf2.GetBytes(size);
            }
        }

        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        internal static (byte[], byte[]) GetKeyAndVerificationTag(byte[] password, byte[] salt, int keySize, int tagSize, int iterations)
        {
            if(tagSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tagSize));
            }

            var km = DeriveKeyPbkdf2(password, salt, keySize + tagSize, iterations);
            return (km.Take(keySize).ToArray(), km.Skip(keySize).ToArray());
        }
    }
}
