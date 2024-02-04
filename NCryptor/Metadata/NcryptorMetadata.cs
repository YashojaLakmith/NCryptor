namespace NCryptor.Metadata
{
    public class NcryptorMetadata : IDisposable
    {
        private bool _disposedValue;

        public byte[] VerificationTag { get; }
        public byte[] Salt { get; }
        public byte[] IV { get; }

        private NcryptorMetadata(byte[] tag, byte[] salt, byte[] iv)
        {
            VerificationTag = tag;
            Salt = salt;
            IV = iv;
        }

        public static NcryptorMetadata Create(byte[] tag, byte[] salt, byte[] iv) 
            => new(tag, salt, iv);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                Array.Clear(VerificationTag, 0, VerificationTag.Length);
                Array.Clear(Salt, 0, Salt.Length);
                Array.Clear(IV, 0, IV.Length);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Metadata()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
