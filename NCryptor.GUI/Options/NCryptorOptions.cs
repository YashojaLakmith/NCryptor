namespace NCryptor.GUI.Options
{
    internal class NCryptorOptions
    {
        public int KeyDerivationIterations { get; } = 100000;
        public int VerificationTagLength { get; } = 64;
    }
}
