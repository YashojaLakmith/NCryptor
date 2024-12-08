namespace NCryptor.TaskModerators;

/// <summary>
/// Defines methods for file encryption.
/// </summary>
public interface IEncryptTaskModerator
{
    /// <summary>
    /// Asynchronously moderates file encryption.
    /// </summary>
    /// <returns></returns>
    Task ModerateFileEncryptionAsync();
}
