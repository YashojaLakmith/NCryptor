using System.Text.RegularExpressions;

namespace NCryptor.Validations;

public partial class InputValidationsImpl : IInputValidations
{
    public bool IsValidOutputDirectory(string outputDirectory)
    {
        try
        {
            string tempFile = TryCreatingRandomFilePath(outputDirectory);
            TryCreateRandomFile(tempFile);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    private static string TryCreatingRandomFilePath(string outputDirectory)
    {
        string randomFileName = $@"{Guid.NewGuid()}.tmp";
        return Path.Combine(outputDirectory, randomFileName);
    }

    private static void TryCreateRandomFile(string randomFilePath)
    {
        try
        {
            File.Create(randomFilePath);
        }
        finally
        {
            if (File.Exists(randomFilePath))
            {
                File.Delete(randomFilePath);
            }
        }
    }

    public bool IsValidPassword(string password)
    {
        return PasswordValidationRegex().IsMatch(password);
    }

    [GeneratedRegex("^[a-zA-Z0-9!@#$%^&]{6,14}$")]
    private static partial Regex PasswordValidationRegex();
}
