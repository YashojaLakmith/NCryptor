namespace NCryptor.Metadata
{
    public class NcryptorMetadata : IDisposable
    {
        private bool _disposedValue;

        public byte[] VerificationTag { get; }
        public byte[] Salt { get; }
        public byte[] IV { get; }

        private NcryptorMetadata(byte[] salt, byte[] tag, byte[] iv)
        {
            VerificationTag = tag;
            Salt = salt;
            IV = iv;
        }

        public static NcryptorMetadata Create(byte[] salt, byte[] tag, byte[] iv) 
            => new(salt, tag, iv);

        public bool AreVerificationTagsEqual(byte[] otherTag)
            => VerificationTag.SequenceEqual(otherTag);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                Array.Clear(VerificationTag);
                Array.Clear(Salt);
                Array.Clear(IV);
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
