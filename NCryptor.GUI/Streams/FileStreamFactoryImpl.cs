using System.IO;

namespace NCryptor.GUI.Streams
{
    internal class FileStreamFactoryImpl : IFileStreamFactory
    {
        public FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess access, FileShare fileShare)
        {
            return new FileStream(filePath, fileMode, access, fileShare);
        }
    }
}
