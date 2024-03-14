namespace NCryptor.Metadata
{
    public class NcryptorMetadata : IDisposable, IEquatable<NcryptorMetadata>
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

        public bool Equals(NcryptorMetadata? other)
        {
            if(other is null)
            {
                return false;
            }

            return VerificationTag.SequenceEqual(other.VerificationTag) &&
                Salt.SequenceEqual(other.Salt) &&
                IV.SequenceEqual(other.IV);
        }

        public override bool Equals(object? obj)
            => Equals(obj as NcryptorMetadata);

        public override int GetHashCode()
            => VerificationTag.GetHashCode() ^
                Salt.GetHashCode() ^
                IV.GetHashCode();

        public static bool operator ==(NcryptorMetadata lhs, NcryptorMetadata rhs)
            => lhs.Equals(rhs);

        public static bool operator !=(NcryptorMetadata lhs, NcryptorMetadata rhs)
            => lhs.Equals(rhs);
    }
}
