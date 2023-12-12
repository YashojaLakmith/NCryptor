using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using NCryptor.Core;

namespace AES
{
    public class AES_256_CBC : ICryptoService
    {
        private bool disposedValue;
        private readonly IStreamProvider _streams;
        private readonly Aes _aesAlg;

        /// <exception cref="ArgumentNullException"> is thrown when the <see cref="IKeyMaterial.Key"/> or <see cref="IKeyMaterial.IV"/> or <see cref="IStreamProvider.InputStream"/> or <see cref="IStreamProvider.OutputStream"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"> is thrown when <see cref="IKeyMaterial.Key"/> or <see cref="IKeyMaterial.IV"/> lengths are invalid.</exception>
        public AES_256_CBC(IStreamProvider streams, IAESKeyMaterial keyMaterial)
        {
            VerifyKeyandIV(keyMaterial.Key, keyMaterial.IV);

            _streams = streams;
            _aesAlg = Aes.Create();
            _aesAlg.IV = keyMaterial.IV;
            _aesAlg.Key = keyMaterial.Key;
            _aesAlg.KeySize = 256;
            _aesAlg.Mode = CipherMode.CBC;
            _aesAlg.Padding = PaddingMode.PKCS7;
        }

        public async Task DecryptAsync(CancellationToken cancellationToken = default)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(_aesAlg));
            }

            cancellationToken.ThrowIfCancellationRequested();

            using (var decryptor = _aesAlg.CreateDecryptor())
            {
                using(var cs = new CryptoStream(_streams.OutputStream, decryptor, CryptoStreamMode.Write))
                {
                    await _streams.InputStream.CopyToAsync(cs, 81920, cancellationToken);
                }
            }
        }

        public async Task EncryptAsync(CancellationToken cancellationToken = default)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(_aesAlg));
            }

            cancellationToken.ThrowIfCancellationRequested();

            using(var encryptor = _aesAlg.CreateEncryptor())
            {
                using(var cs = new CryptoStream(_streams.OutputStream, encryptor, CryptoStreamMode.Write))
                {
                    await _streams.InputStream.CopyToAsync(cs, 81920, cancellationToken);
                }
            }
        }

        private void VerifyKeyandIV(byte[] key, byte[] iv)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (iv is null)
            {
                throw new ArgumentNullException(nameof(iv));
            }

            if (key.Length != 32)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            if (iv.Length != 16)
            {
                throw new ArgumentOutOfRangeException(nameof(iv));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _aesAlg.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        //~AES_256_CBC()
        //{
        //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //    Dispose(disposing: false);
        //}

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
