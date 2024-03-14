using System.Security.Cryptography;

using NCryptor.Crypto;
using NCryptor.Options;

namespace NCryptor.Helpers
{
    public class KeyDerivationServiceImpl : IKeyDerivationServices
    {
        private readonly KeyDerivationOptions _keyDerivationOptions;
        private readonly ICryptographicOptions _cryptographicOptions;

        public KeyDerivationServiceImpl(ICryptographicOptions cryptographicOptions, KeyDerivationOptions keyDerivationOptions)
        {
            _cryptographicOptions = cryptographicOptions;
            _keyDerivationOptions = keyDerivationOptions;
        }

        public (byte[], byte[]) DeriveKeyAndVerificationTag(byte[] password, byte[] salt)
        {
            var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, _keyDerivationOptions.KeyDerivationIterations, HashAlgorithmName.SHA512, _cryptographicOptions.ByteLengthOfKey);
            var keyPart = key.Skip(key.Length / 2).ToArray();

            var tag = SHA512.HashData(keyPart);
            var tagPart = tag.Take(_keyDerivationOptions.VerificationTagLength).ToArray();

            Array.Clear(keyPart);
            Array.Clear(tag);

            return (key, tagPart);
        }

        public byte[] GenerateRandomBytes(int length)
            => RandomNumberGenerator.GetBytes(length);

        public byte[] GenerateRandomIv()
            => RandomNumberGenerator.GetBytes(_cryptographicOptions.ByteLengthOfIV);

        public byte[] GenerateRandomSalt()
            => RandomNumberGenerator.GetBytes(_keyDerivationOptions.SaltLength);
    }
}
