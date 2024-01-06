using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NCryptor.GUI.Crypto
{
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
