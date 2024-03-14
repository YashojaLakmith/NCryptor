namespace NCryptor.Helpers
{
    /// <summary>
    /// Defines File IO methods.
    /// </summary>
    public interface IFileServices
    {
        /// <returns><c>true</c> if file exists. otherwise <c>false</c>.</returns>
        bool CheckFileExistance(string filePath);
        void DeleteFileIfExists(string filePath);

        /// <summary>
        /// Creates the file path for the output encrypted file using the original file path and output directory path.
        /// </summary>
        string CreateEncryptedFilePath(string originalPath, string outputDirectory);

        /// <summary>
        /// Creates the file path for the output decrypted file using the original file path and output directory path.
        /// </summary>
        string CreateDecryptedFilePath(string originalPath, string outputDirectory);
    }
}
