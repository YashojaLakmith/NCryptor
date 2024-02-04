namespace NCryptor.Helpers
{
    internal interface IFileServices
    {
        bool CheckFileExistance(string filePath);
        void DeleteFileIfExists(string filePath);
        string CreateEncryptedFilePath(string originalPath, string outputDirectory);
        string CreateDecryptedFilePath(string originalPath, string outputDirectory);
    }
}
