namespace NCryptor.Options;

/// <summary>
/// Base class for sharing the key derivation options throughout the application.
/// </summary>
public class KeyDerivationOptions
{
    public virtual int KeyDerivationIterations { get; } = 100000;
    public virtual int VerificationTagLength { get; } = 64;
    public virtual int SaltLength { get; } = 64;
}
