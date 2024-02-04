namespace NCryptor.GUI.Helpers
{
    internal interface IKeyDerivationServices
    {
        byte[] GenerateRandomBytes(int length);
        byte[] GenerateRandomIV();
        byte[] GenerateRandomSalt();
        (byte[], byte[]) DeriveKeyAndVerificationTag(byte[] password, byte[] salt);
    }
}
