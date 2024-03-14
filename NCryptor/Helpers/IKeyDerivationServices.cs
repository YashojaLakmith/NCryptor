namespace NCryptor.Helpers
{
    /// <summary>
    /// Defines methods for key derivation functionalities.
    /// </summary>
    public interface IKeyDerivationServices
    {
        /// <summary>
        /// Generates a byte array of given length using a cryptographically strong random number generator.
        /// </summary>
        byte[] GenerateRandomBytes(int length);

        /// <summary>
        /// Generates a random initialization vector using a cryptographically strong random number generator.
        /// </summary>
        byte[] GenerateRandomIv();

        /// <summary>
        /// Generates a random salt using a cryptographically strong random number generator.
        /// </summary>
        byte[] GenerateRandomSalt();

        /// <summary>
        /// Derives a key and a unique verification tag using given password and salt.
        /// </summary>
        (byte[], byte[]) DeriveKeyAndVerificationTag(byte[] password, byte[] salt);
    }
}
