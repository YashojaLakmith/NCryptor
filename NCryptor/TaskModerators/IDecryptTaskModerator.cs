namespace NCryptor.TaskModerators
{
    /// <summary>
    /// Defines methods for moderating the decryption operation.
    /// </summary>
    public interface IDecryptTaskModerator
    {
        /// <summary>
        /// Asynchronously moderates file decryption.
        /// </summary>
        Task ModerateFileDecryptionAsync();
    }
}
