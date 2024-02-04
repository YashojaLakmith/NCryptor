using System.IO;

namespace NCryptor.GUI.Streams
{
    internal class FileStreamFactoryImpl : IFileStreamFactory
    {
        public FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess access, FileShare fileShare)
        {
            return new FileStream(filePath, fileMode, access, fileShare);
        }

        public FileStream CreateReadFileStream(string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public FileStream CreateWriteFileStream(string filePath)
        {
            return new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
        }
    }
}
