namespace NCryptor.Streams;

/// <summary>
/// Defines methods for creating file streams.
/// </summary>
public interface IFileStreamFactory
{
    /// <summary>
    /// Creates a file stream with given parameters.
    /// </summary>
    FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess access, FileShare fileShare);

    /// <summary>
    /// Creates a file stream for file read.
    /// </summary>
    FileStream CreateReadFileStream(string filePath);

    /// <summary>
    /// Creates a file stream with exclusive file write access.
    /// </summary>
    FileStream CreateWriteFileStream(string filePath);
}
