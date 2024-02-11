namespace NCryptor.Streams
{
    public class FileStreamFactoryImpl : IFileStreamFactory
    {
        public FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess access, FileShare fileShare)
            => new(filePath, fileMode, access, fileShare);

        public FileStream CreateReadFileStream(string filePath)
            => new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        public FileStream CreateWriteFileStream(string filePath)
            => new(filePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
    }
}