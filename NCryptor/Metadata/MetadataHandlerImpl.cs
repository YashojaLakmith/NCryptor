namespace NCryptor.Metadata
{
    public class MetadataHandlerImpl : IMetadataHandler
    {
        public async Task<NcryptorMetadata> ReadMetadataAsync(Stream sourceStream, CancellationToken cancellationToken = default)
        {
            sourceStream.Position = 0;
            var sectionData = await CreateSectionArray(sourceStream, cancellationToken);

            return NcryptorMetadata.Create(sectionData[0], sectionData[1], sectionData[2]);
        }

        public async Task WriteMetadataAsync(NcryptorMetadata metadata, Stream targetStream, CancellationToken cancellationToken = default)
        {
            targetStream.Position = 0;
            await WriteBufferAsync(targetStream, metadata.Salt, cancellationToken);
            await WriteBufferAsync(targetStream, metadata.VerificationTag, cancellationToken);
            await WriteBufferAsync(targetStream, metadata.IV, cancellationToken);
        }

        private static async Task<byte[][]> CreateSectionArray(Stream source, CancellationToken cancellationToken = default)
        {
            const int sectionCount = 3;
            var sectionData = new byte[sectionCount][];

            for (var i = 0; i < sectionCount; i++)
            {
                sectionData[i] = await ReadSectionAsync(source, cancellationToken);
            }

            return sectionData;
        }

        private static async Task<byte[]> ReadSectionAsync(Stream source, CancellationToken cancellationToken = default)
        {
            const int intLengthInBytes = 4;
            var bufferSizeInBytes = new byte[intLengthInBytes];

            await source.ReadAsync(bufferSizeInBytes, 0, intLengthInBytes, cancellationToken);
            var bufferSize = BitConverter.ToInt32(bufferSizeInBytes);
            var buffer = new byte[bufferSize];
            await source.ReadExactlyAsync(buffer, 0, bufferSize, cancellationToken);

            return buffer;
        }

        private static async Task WriteBufferAsync(Stream target, byte[] buffer, CancellationToken cancellationToken = default)
        {
            var bufferLengthInBytes = BitConverter.GetBytes(buffer.Length);

            await target.WriteAsync(bufferLengthInBytes, 0, bufferLengthInBytes.Length, cancellationToken);
            await target.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
        }
    }
}
