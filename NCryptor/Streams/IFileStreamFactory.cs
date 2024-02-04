namespace NCryptor.Streams
{
    public interface IFileStreamFactory
    {
        FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess access, FileShare fileShare);
        FileStream CreateReadFileStream(string filePath);
        FileStream CreateWriteFileStream(string filePath);
    }
}
