namespace NCryptor.Options;

/// <summary>
/// Defines methods for retrieving the options for cryptographic operations.
/// </summary>
public interface ICryptographicOptions
{
    /// <summary>
    /// Key size for the cryptographic algorithm used, in bytes.
    /// </summary>
    int ByteLengthOfKey { get; }

    /// <summary>
    /// Initialization Vector size for the cryptographic algorithm used, in bytes.
    /// </summary>
    int ByteLengthOfIV { get; }
}
