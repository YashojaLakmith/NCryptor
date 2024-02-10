namespace NCryptor.TaskModerators
{
    /// <summary>
    /// Provides methods for moderating the processing of file list to be encrypted or decrypted.
    /// </summary>
    public interface ITaskModerator : IDisposable
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
