using NCryptor.Options;

namespace NCryptor.Helpers
{
    internal class FileServicesImpl : IFileServices
    {
        private readonly FileSystemOptions _options;

        public FileServicesImpl(FileSystemOptions fileOptions)
        {
            _options = fileOptions;
        }

        public bool CheckFileExistance(string filePath)
        {
            return File.Exists(filePath);
        }

        public string CreateDecryptedFilePath(string originalPath, string outputDirectory)
        {
            var name = Path.GetFileNameWithoutExtension(originalPath);
            var fullPath = Path.Combine(outputDirectory, name);

            if (!CheckFileExistance(fullPath)) return fullPath;

            return RandomlyChangeFileName(fullPath);
        }

        public string CreateEncryptedFilePath(string originalPath, string outputDirectory)
        {
            var ext = Path.GetExtension(originalPath);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(originalPath);
            var nameWithExt = Path.GetFileName(originalPath);
            var tempName = Path.Combine(outputDirectory, nameWithExt);

            if(!CheckFileExistance($"{tempName}{_options.Extension}")) return $"{tempName}{_options.Extension}";

            tempName = RandomlyChangeFileName(tempName);
            return $"{tempName}{_options.Extension}";
        }

        public void DeleteFileIfExists(string filePath)
        {
            if (CheckFileExistance(filePath)) File.Delete(filePath);
        }

        private string RandomlyChangeFileName(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            var directory = Path.GetDirectoryName(filePath);

            var fullPath = $"{nameWithoutExt} ({DateTime.Now: yyyy-MM-dd--HH-mm-ss})";
            if (!string.IsNullOrEmpty(ext)) fullPath = $"{fullPath}{ext}";

            return Path.Combine(directory, fullPath);
        }
    }
}
