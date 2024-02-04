namespace NCryptor.Metadata
{
    internal class MetadataHandlerImpl : IMetadataHandler
    {
        public async Task<NcryptorMetadata> ReadMetadataAsync(Stream sourceStream, CancellationToken cancellationToken = default)
        {
            sourceStream.Position = 0;
            var lengthBytes = new byte[4];

            await sourceStream.ReadAsync(lengthBytes, 0, 4, cancellationToken);
            var saltSize = BitConverter.ToInt32(lengthBytes, 0);
            var salt = new byte[saltSize];
            await sourceStream.ReadAsync(salt, 0, saltSize, cancellationToken);

            await sourceStream.ReadAsync(lengthBytes, 0, 4, cancellationToken);
            var verificationTagSize = BitConverter.ToInt32(lengthBytes, 0);
            var verificationTag = new byte[verificationTagSize];
            await sourceStream.ReadAsync(verificationTag, 0, verificationTagSize, cancellationToken);

            await sourceStream.ReadAsync(lengthBytes, 0, 4, cancellationToken);
            var ivSize = BitConverter.ToInt32(lengthBytes, 0);
            var iv = new byte[ivSize];
            await sourceStream.ReadAsync(iv, 0, ivSize, cancellationToken);

            return NcryptorMetadata.Create(verificationTag, salt, iv);
        }

        public async Task WriteMetadataAsync(NcryptorMetadata metadata, Stream targetStream, CancellationToken cancellationToken = default)
        {
            var saltLengthBytes = BitConverter.GetBytes(metadata.Salt.Length);
            var tagLengthBytes = BitConverter.GetBytes(metadata.VerificationTag.Length);
            var ivLengthBytes = BitConverter.GetBytes(metadata.IV.Length);

            await targetStream.WriteAsync(saltLengthBytes, 0, saltLengthBytes.Length, cancellationToken);
            await targetStream.WriteAsync(metadata.Salt, 0, metadata.Salt.Length, cancellationToken);

            await targetStream.WriteAsync(tagLengthBytes, 0, tagLengthBytes.Length, cancellationToken);
            await targetStream.WriteAsync(metadata.VerificationTag, 0, metadata.VerificationTag.Length, cancellationToken);

            await targetStream.WriteAsync(ivLengthBytes, 0, ivLengthBytes.Length, cancellationToken);
            await targetStream.WriteAsync(metadata.IV, 0, metadata.IV.Length, cancellationToken);
        }
    }
}
