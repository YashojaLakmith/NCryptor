namespace NCryptor.Validations;

/// <summary>
/// Defines methods for validating the user input.
/// </summary>
public interface IInputValidations
{
    /// <summary>
    /// Validates the given password.
    /// </summary>
    /// <returns><c>true</c> if valid. Otherwise <c>false</c>.</returns>
    bool IsValidPassword(string password);

    /// <summary>
    /// Validates the given output directory for existence and necessary permissions.
    /// </summary>
    /// <returns><c>true</c> if valid. Otherwise <c>false</c>.</returns>
    bool IsValidOutputDirectory(string outputDirectory);
}
