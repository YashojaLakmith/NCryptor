using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace NCryptor.Core
{
    public class NCryptorTask : IDisposable
    {
        private readonly ICryptoService _cryptoService;
        private readonly IKeyMaterial _keyMaterial;
        private readonly ILogger _logger;
        private readonly IStreamProvider _streamProvider;
        private readonly CancellationToken _cancellationToken;
        private bool disposedValue;

        public NCryptorTask(ICryptoService cryptoService, IKeyMaterial keyMaterial, ILogger logger, IStreamProvider streamProvider, CancellationToken cancellationToken = default)
        {
            _cryptoService = cryptoService;
            _keyMaterial = keyMaterial;
            _logger = logger;
            _streamProvider = streamProvider;
            _cancellationToken = cancellationToken;
        }

        public virtual async Task HandleEncryptAsync()
        {
            //try
            //{
            //    await _cryptoService.EncryptAsync(_cancellationToken);
            //}
            //catch (OperationCanceledException ex)
            //{
            //    _logger.Log(ex);
            //    throw;
            //}
            //catch(ArgumentNullException ex)
            //{
            //    _logger.Log(ex);
            //}
            //catch(CryptographicException ex)
            //{
            //    _logger.Log(ex);
            //}
            //catch(Exception ex)
            //{
            //    _logger.Log(ex);
            //}
        }

        public virtual async Task HandleDecryptAsync()
        {
            //try
            //{
            //    await _cryptoService.DecryptAsync(_cancellationToken);
            //}
            //catch(OperationCanceledException ex)
            //{
            //    _logger.Log(ex);
            //    throw;
            //}
            //catch (ArgumentNullException ex)
            //{
            //    _logger.Log(ex);
            //}
            //catch (CryptographicException ex)
            //{
            //    _logger.Log(ex);
            //}
            //catch(Exception ex)
            //{
            //    _logger.Log(ex);
            //}
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _cryptoService.Dispose();
                    _keyMaterial.Dispose();
                    _streamProvider.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~NCryptorTask()
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
