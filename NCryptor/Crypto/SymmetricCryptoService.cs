using System.Security.Cryptography;
using NCryptor.Events.EventArguments;

namespace NCryptor.Crypto
{
    internal class SymmetricCryptoService : ISymmetricCryptoService
    {
        private readonly SymmetricAlgorithm _alg;
        private bool disposedValue;
        private const int BUFFER_SIZE = 81920;

        public int KeySizeInBytes => _alg.KeySize / 8;
        public int IVSizeInBytes => _alg.IV.Length;

        public event EventHandler<ProgressPercentageReportedEventArgs> ProgressPercentageReported;

        public SymmetricCryptoService(SymmetricAlgorithm algorithm)
        {
            _alg = algorithm;
            _alg.GenerateIV();
        }

        public async Task DecryptAsync(Stream inputStream, Stream outputStream, byte[] key, byte[] iv, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _alg.Key = key;
            _alg.IV = iv;

            using var decryptor = _alg.CreateDecryptor();
            using var cs = new CryptoStream(outputStream, decryptor, CryptoStreamMode.Write);
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesRead;
            int previousReportedProgress = 0;

            while ((bytesRead = await inputStream.ReadAsync(buffer, 0, BUFFER_SIZE, cancellationToken)) > 0)
            {
                int progress = CalculateProgress(inputStream.Position, inputStream.Length);
                ReportProgressIfHigherThanPreviousReported(ref previousReportedProgress, progress);

                await cs.WriteAsync(buffer, 0, bytesRead, cancellationToken);
            }

            await cs.FlushFinalBlockAsync(cancellationToken);
            Array.Clear(buffer, 0, BUFFER_SIZE);
        }

        public async Task EncryptAsync(Stream inputStream, Stream outputStream, byte[] key, byte[] iv, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _alg.Key = key;
            _alg.IV = iv;

            using var encryptor = _alg.CreateEncryptor();
            using var cs = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write);
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesRead;
            int previousReportedProgress = 0;

            while ((bytesRead = await inputStream.ReadAsync(buffer, 0, BUFFER_SIZE, cancellationToken)) > 0)
            {
                int progress = CalculateProgress(inputStream.Position, inputStream.Length);
                ReportProgressIfHigherThanPreviousReported(ref previousReportedProgress, progress);

                await cs.WriteAsync(buffer, 0, bytesRead, cancellationToken);
            }

            await cs.FlushFinalBlockAsync(cancellationToken);
            Array.Clear(buffer, 0, BUFFER_SIZE);
        }

        public virtual void ReportProgressPercentage(ProgressPercentageReportedEventArgs e)
        {
            ProgressPercentageReported?.Invoke(this, e);
        }

        private static int CalculateProgress(long currentPosition, long length)
        {
            return (int)Math.Round((double)currentPosition / length * 100);
        }

        private void ReportProgressIfHigherThanPreviousReported(ref int previousValue, int newValue)
        {
            if (previousValue < newValue)
            {
                ReportProgressPercentage(new ProgressPercentageReportedEventArgs(newValue));
                previousValue = newValue;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _alg.Key = null;
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SymmetricCryptoService()
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
