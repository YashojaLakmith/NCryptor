using System;
using System.Collections.Generic;
using System.Text;

namespace AES
{
    public class AESKeyMaterial
    {
        public AESKeyMaterial(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
        }

        public byte[] Key { get; }
        public byte[] IV { get; }
    }
}
