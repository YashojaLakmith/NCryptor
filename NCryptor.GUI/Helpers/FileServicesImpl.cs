using System;
using System.IO;

using NCryptor.GUI.Options;

namespace NCryptor.GUI.Helpers
{
    internal class FileServicesImpl : IFileServices
    {
        private readonly FileSystemOptions _options;

        public FileServicesImpl(FileSystemOptions fileOptions)
        {
            _options = fileOptions;
        }

        public string ChangeOutputFileNameIfExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return filePath;
            }

            var ext = Path.GetExtension(filePath);
            var file = Path.GetFileNameWithoutExtension(filePath);

            file += $" ({DateTime.Now})";
            return Path.ChangeExtension(file, ext);
        }

        public bool CheckFileExistance(string filePath)
        {
            return File.Exists(filePath);
        }

        public string CreateDecryptedFilePath(string originalPath, string outputDirectory)
        {
            var name = Path.GetFileNameWithoutExtension(Path.GetFileName(originalPath));
            var fullPath = Path.Combine(outputDirectory, name);
            fullPath = ChangeOutputFileNameIfExists(fullPath);

            return fullPath;
        }

        public string CreateEncryptedFilePath(string originalPath, string outputDirectory)
        {
            var name = Path.GetFileName(originalPath);
            name += _options.Extension;
            name = ChangeOutputFileNameIfExists(name);

            return Path.Combine(outputDirectory, name);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }
    }
}
