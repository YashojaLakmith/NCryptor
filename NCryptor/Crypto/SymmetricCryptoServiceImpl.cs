using System.Security.Cryptography;

using NCryptor.Events;

namespace NCryptor.Crypto
{
    public class SymmetricCryptoServiceImpl : ISymmetricCryptoService
    {
        private readonly SymmetricAlgorithm _alg;
        private bool _disposedValue;
        private readonly IProgressReportEventService _progressReportService
            ;
        private const int BufferSize = 81920;

        public int KeyByteSize => _alg.KeySize / 8;
        public int IvByteSize => _alg.IV.Length;

        public SymmetricCryptoServiceImpl(SymmetricAlgorithm algorithm, IProgressReportEventService progressReportService)
        {
            _alg = algorithm;
            _progressReportService = progressReportService;

            _alg.GenerateIV();
        }

        public async Task DecryptAsync(Stream inputStream, Stream outputStream, byte[] key, byte[] iv, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SetupKeyAndIv(key, iv);
            using var decryptor = _alg.CreateDecryptor();
            await using var cs = new CryptoStream(outputStream, decryptor, CryptoStreamMode.Write);
            await CopyStreamsWhileReportingProgress(inputStream, cs, cancellationToken);
            await cs.FlushFinalBlockAsync(cancellationToken);
        }

        public async Task EncryptAsync(Stream inputStream, Stream outputStream, byte[] key, byte[] iv, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SetupKeyAndIv(key, iv);
            using var encryptor = _alg.CreateEncryptor();
            await using var cs = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write);
            await CopyStreamsWhileReportingProgress(inputStream, cs, cancellationToken);
            await cs.FlushFinalBlockAsync(cancellationToken);
        }

        private void SetupKeyAndIv(byte[] key, byte[] iv)
        {
            _alg.Key = key;
            _alg.IV = iv;
        }

        private async Task CopyStreamsWhileReportingProgress(Stream inputStream, Stream targetStream, CancellationToken cancellationToken = default)
        {
            var buffer = new byte[BufferSize];
            int bytesRead;
            var previousReportedPercentage = 0;

            while ((bytesRead = await inputStream.ReadAsync(buffer, 0, BufferSize, cancellationToken)) > 0)
            {
                var currentPercentage = CalculateProgress(inputStream.Position, inputStream.Length);
                ReportIfHasAProgress(ref previousReportedPercentage, currentPercentage);
                await targetStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
            }

            Array.Clear(buffer);
        }

        private static int CalculateProgress(long currentPosition, long length)
            => (int) Math.Round((double) currentPosition / length* 100);

        private void ReportIfHasAProgress(ref int previousPercentage, int currentPercentage)
        {
            if (currentPercentage <= previousPercentage) return;
            _progressReportService.ProgressPublished(currentPercentage);
            previousPercentage = currentPercentage;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                    
            }
            _alg.Key = null;
            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
