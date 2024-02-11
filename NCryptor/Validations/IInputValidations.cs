namespace NCryptor.Validations
{
    public interface IInputValidations
    {
        bool IsValidPassword(string password);
        bool IsValidOutputDirectory(string outputDirectory);
    }
}
