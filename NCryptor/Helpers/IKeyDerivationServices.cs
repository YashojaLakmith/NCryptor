namespace NCryptor.Helpers
{
    public interface IKeyDerivationServices
    {
        byte[] GenerateRandomBytes(int length);
        byte[] GenerateRandomIv();
        byte[] GenerateRandomSalt();
        (byte[], byte[]) DeriveKeyAndVerificationTag(byte[] password, byte[] salt);
    }
}
