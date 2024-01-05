using System;
using System.Security.Cryptography;

using AES;

namespace NCryptor.GUI
{
    internal class AES_KeyMaterial : IAESKeyMaterial, IDisposable
    {
        private bool disposedValue;

        public AES_KeyMaterial(byte[] key)
        {
            Key = key;
            IV = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(IV);
            }
        }

        public AES_KeyMaterial(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
        }

        public byte[] Key { get; }

        public byte[] IV { get; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Array.Clear(Key, 0, Key.Length);
                    Array.Clear(IV, 0, IV.Length);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AES_KeyMaterial()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
