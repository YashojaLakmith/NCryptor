using NCryptor.Options;

namespace NCryptor.Helpers;

public class FileServicesImpl : IFileServices
{
    private readonly FileSystemOptions _options;

    public FileServicesImpl(FileSystemOptions fileOptions)
    {
        _options = fileOptions;
    }

    public bool CheckFileExistance(string filePath)
        => File.Exists(filePath);

    public string CreateDecryptedFilePath(string originalPath, string outputDirectory)
    {
        string name = Path.GetFileNameWithoutExtension(originalPath);
        string fullPath = Path.Combine(outputDirectory, name);

        return !CheckFileExistance(fullPath) ? fullPath : RandomlyChangeFileName(fullPath);
    }

    public string CreateEncryptedFilePath(string originalPath, string outputDirectory)
    {
        string nameWithExt = Path.GetFileName(originalPath);
        string tempName = Path.Combine(outputDirectory, nameWithExt);

        if (!CheckFileExistance($"{tempName}{_options.FileExtension}"))
        {
            return $"{tempName}{_options.FileExtension}";
        }

        tempName = RandomlyChangeFileName(tempName);
        return $"{tempName}{_options.FileExtension}";
    }

    public void DeleteFileIfExists(string filePath)
    {
        if (CheckFileExistance(filePath))
        {
            File.Delete(filePath);
        }
    }

    private static string RandomlyChangeFileName(string filePath)
    {
        string ext = Path.GetExtension(filePath);
        string nameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
        string? directory = Path.GetDirectoryName(filePath);

        string fullPath = $"{nameWithoutExt} ({DateTime.Now: yyyy-MM-dd--HH-mm-ss})";
        if (!string.IsNullOrEmpty(ext))
        {
            fullPath = $"{fullPath}{ext}";
        }

        return Path.Combine(directory, fullPath);
    }
}
