using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace NCryptor.Core
{
    public class NCryptorTask : IDisposable
    {
        private readonly ICryptoService _cryptoService;
        private readonly ILogger _logger;
        private readonly IStreamProvider _streamProvider;
        private readonly CancellationToken _cancellationToken;
        private bool disposedValue;

        public NCryptorTask(ICryptoService cryptoService, ILogger logger, IStreamProvider streamProvider, CancellationToken cancellationToken = default)
        {
            _cryptoService = cryptoService;
            _logger = logger;
            _streamProvider = streamProvider;
            _cancellationToken = cancellationToken;
        }

        /// <exception cref="OperationCanceledException"> is thrown when cancellation is triggered by <see cref="CancellationToken"/>.</exception>
        /// <exception cref="ObjectDisposedException"> is thrown when an object used in encryption operation has already been disposed.</exception>
        public Task HandleEncryptAsync()
        {
            throw new NotImplementedException();
        }

        public Task HandleDecryptAsync()
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _cryptoService.Dispose();
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
