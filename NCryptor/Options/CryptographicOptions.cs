namespace NCryptor.Options
{
    internal class CryptographicOptions
    {
        public virtual int KeyDerivationIterations { get; } = 100000;
        public virtual int VerificationTagLength { get; } = 64;
        public virtual int SaltLength { get; } = 64;
    }
}
