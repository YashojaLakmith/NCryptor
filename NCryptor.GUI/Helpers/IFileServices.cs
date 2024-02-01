namespace NCryptor.GUI.Helpers
{
    internal interface IFileServices
    {
        bool CheckFileExistance(string filePath);
        void DeleteFile(string filePath);
        string ChangeOutputFileNameIfExists(string filePath);
        string CreateEncryptedFilePath(string originalPath, string outputDirectory);
        string CreateDecryptedFilePath(string originalPath, string outputDirectory);
    }
}
