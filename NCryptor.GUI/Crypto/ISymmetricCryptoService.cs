using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using NCryptor.GUI.Events;
using NCryptor.GUI.Streams;

namespace NCryptor.GUI.Crypto
{
    internal interface ISymmetricCryptoService : IProgressReportable, IDisposable
    {
        int KeySizeInBytes { get; }
        int IVSizeInBytes { get; }

        Task EncryptAsync(Stream inputStream, Stream outputStream, byte[] key, byte[] iv, CancellationToken cancellationToken = default);
        Task DecryptAsync(Stream inputStream, Stream outputStream, byte[] key, byte[] iv, CancellationToken cancellationToken = default);
    }
}
