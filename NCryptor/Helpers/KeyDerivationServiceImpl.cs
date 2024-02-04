using System.Security.Cryptography;

using NCryptor.Crypto;
using NCryptor.Options;

namespace NCryptor.Helpers
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
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, _options.KeyDerivationIterations, HashAlgorithmName.SHA512, _cryptoService.KeySizeInBytes);
            byte[] keyPart = key.Skip(key.Length / 2).ToArray();
            byte[] tag;
            byte[] tagPart;

            using (var hash = SHA512.Create())
            {
                tag = hash.ComputeHash(keyPart);
                tagPart = tag.Take(_options.VerificationTagLength).ToArray();
            }

            Array.Clear(keyPart, 0, keyPart.Length);
            Array.Clear(tag, 0, tag.Length);

            return (key, tagPart);
        }

        public byte[] GenerateRandomBytes(int length)
        {
            return RandomNumberGenerator.GetBytes(length);
        }

        public byte[] GenerateRandomIV()
        {
            return RandomNumberGenerator.GetBytes(_cryptoService.IVSizeInBytes);
        }

        public byte[] GenerateRandomSalt()
        {
            return RandomNumberGenerator.GetBytes(_options.SaltLength);
        }
    }
}
