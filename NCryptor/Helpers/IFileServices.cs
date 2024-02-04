namespace NCryptor.Helpers
{
    public interface IFileServices
    {
        bool CheckFileExistance(string filePath);
        void DeleteFileIfExists(string filePath);
        string CreateEncryptedFilePath(string originalPath, string outputDirectory);
        string CreateDecryptedFilePath(string originalPath, string outputDirectory);
    }
}
