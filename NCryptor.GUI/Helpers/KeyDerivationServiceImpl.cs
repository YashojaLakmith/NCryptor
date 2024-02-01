using System;
using System.Linq;
using System.Security.Cryptography;

using NCryptor.GUI.Crypto;
using NCryptor.GUI.Options;

namespace NCryptor.GUI.Helpers
{
    internal class KeyDerivationServiceImpl : IKeyDerivationServices
    {
        private readonly ISymmetricCryptoService _cryptoService;
        private readonly CryptographicOptions _options;

        public KeyDerivationServiceImpl(ISymmetricCryptoService cryptoService, CryptographicOptions options)
        {
            _cryptoService = cryptoService;
            _options = options;
        }

        public (byte[], byte[]) DeriveKeyAndVerificationTag(byte[] password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _options.KeyDerivationIterations, HashAlgorithmName.SHA512))
            {
                byte[] key = pbkdf2.GetBytes(_cryptoService.KeySizeInBytes);
                byte[] keyPart = key.Skip(key.Length / 2).ToArray();
                byte[] tag;
                byte[] tagPart;

                using(var hash = SHA512.Create())
                {
                    tag = hash.ComputeHash(keyPart);
                    tagPart = tag.Take(_options.VerificationTagLength).ToArray();
                }

                Array.Clear(keyPart, 0, keyPart.Length);
                Array.Clear(tag, 0, tag.Length);
                
                return (key, tagPart);
            }
        }

        public byte[] GenerateRandomBytes(int length)
        {
            var bytes = new byte[length];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);

                return bytes;
            }
        }

        public byte[] GenerateRandomIV()
        {
            return GenerateRandomBytes(_cryptoService.IVSizeInBytes);
        }

        public byte[] GenerateRandomSalt()
        {
            return GenerateRandomBytes(_options.SaltLength);
        }
    }
}
