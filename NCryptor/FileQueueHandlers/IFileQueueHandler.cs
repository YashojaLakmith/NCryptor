using NCryptor.Events;

namespace NCryptor.FileQueueHandlers
{
    /// <summary>
    /// Provides methods for moderating the processing of file list to be encrypted or decrypted.
    /// </summary>
    public interface IFileQueueHandler : IFileQueueEvents, IDisposable
    {
        /// <summary>
        /// Asynchronously moderates the encryption of files.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the operation.</returns>
        Task EncryptTheFilesAsync(IEnumerable<string> fileList, string outputDirectory, byte[] key, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously moderates the decryption of files.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the operation.</returns>
        Task DecryptTheFilesAsync(IEnumerable<string> fileList, string outputDirectory, byte[] key, CancellationToken cancellationToken);
    }
}
