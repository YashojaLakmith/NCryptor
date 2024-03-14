namespace NCryptor.Events
{
    /// <summary>
    /// Defines various events caused within <see cref="TaskModerators.IEncryptTaskModerator"/> and <see cref="TaskModerators.IDecryptTaskModerator"/> type instances.
    /// </summary>
    public interface ITaskModeratorEventService
    {
        /// <summary>
        /// Ocuurs when a file with the given file path could not be located.
        /// </summary>
        void FileNotFoundEvent(string path);

        /// <summary>
        /// Occurs when an exception was thrown inside the file processing loop handled inside the loop.
        /// </summary>
        void ErrorEvent(string errorMessage);

        /// <summary>
        /// Occurs when a single file was processed successfully.
        /// </summary>
        void SuccessfulSingleFileCompletionEvent();

        /// <summary>
        /// Occurs when each time a new file was started encrypting.
        /// </summary>
        void BeginOfFileEncryptionEvent(string filePath, int zeroBasedIndex, int totalFiles);

        /// <summary>
        /// Occurs when each time a new file was started decrypting.
        /// </summary>
        void BeginOfFileDecryptionEvent(string filePath, int zeroBasedIndex, int totalFiles);

        /// <summary>
        /// Occurs when the user has cancelled an ongoing file processing.
        /// </summary>
        void CancellationEvent();

        /// <summary>
        /// Occurs when all finished processing the whole queue of files.
        /// </summary>
        void CompletionDueToSuccessEvent();

        /// <summary>
        /// Occurs when an exception was thrown inside the file processing loop handled outside the loop.
        /// </summary>
        void CompletionDueToErrorEvent(string message);
    }
}
