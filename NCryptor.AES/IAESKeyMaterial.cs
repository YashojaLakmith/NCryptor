namespace AES
{
    public interface IAESKeyMaterial
    {
        byte[] Key { get; }
        byte[] IV { get; }
    }
}
