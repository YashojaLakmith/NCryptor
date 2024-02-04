using System.IO;

namespace NCryptor.GUI.Streams
{
    internal interface IFileStreamFactory
    {
        FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess access, FileShare fileShare);
        FileStream CreateReadFileStream(string filePath);
        FileStream CreateWriteFileStream(string filePath);
    }
}
