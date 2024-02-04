using System.Security.Cryptography;

using NCryptor.Events;

namespace NCryptor.Crypto
{
    /// <summary>
    /// Provides methods for asynchronously encrypting and decrypting streams.
    /// </summary>
    internal interface ISymmetricCryptoService : IProgressReportable, IDisposable
    {
        /// <summary>
        /// Key size for the cryptographic algorithm used, in bytes.
        /// </summary>
        int KeySizeInBytes { get; }

        /// <summary>
        /// Initialization Vector size for the cryptographic algorithm used, in bytes.
        /// </summary>
        int IVSizeInBytes { get; }

        /// <summary>
        /// Asynchronously encrypts the input stream starting from the current position and writes the cipher to the output stream starting from the current position.
        /// </summary>
        /// <returns>A <see cref="Task"/> repesenting the operation.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="OperationCanceledException"/>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="CryptographicException"/>
        Task EncryptAsync(Stream inputStream, Stream outputStream, byte[] key, byte[] iv, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously decrypts the input stream starting from the current position and writes the plaintext to the output stream starting from the current position.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the operation.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="OperationCanceledException"/>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="CryptographicException"/>
        Task DecryptAsync(Stream inputStream, Stream outputStream, byte[] key, byte[] iv, CancellationToken cancellationToken = default);
    }
}
