namespace NCryptor.Options
{
    public interface ICryptographicOptions
    {
        /// <summary>
        /// Key size for the cryptographic algorithm used, in bytes.
        /// </summary>
        int KeyByteSize { get; }

        /// <summary>
        /// Initialization Vector size for the cryptographic algorithm used, in bytes.
        /// </summary>
        int IvByteSize { get; }
    }
}
