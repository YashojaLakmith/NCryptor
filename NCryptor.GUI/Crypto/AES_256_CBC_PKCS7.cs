using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NCryptor.GUI.Crypto
{
    /// <summary>
    /// A factory class for creating a <see cref="Aes"/> object with required specifications.
    /// </summary>
    internal class AES_256_CBC_PKCS7
    {
        private AES_256_CBC_PKCS7() { }

        internal static Aes CreateObject()
        {
            var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            return aes;
        }
    }
}
