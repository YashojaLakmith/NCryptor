namespace NCryptor.Options
{
    /// <summary>
    /// Base class for sharing file system options throughout the application.
    /// </summary>
    public class FileSystemOptions
    {
        public virtual string FileExtension { get; } = @".NCRYPT";
    }
}
