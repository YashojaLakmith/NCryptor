namespace NCryptor.GUI.Metadata
{
    internal interface IMetadataHandler
    {
        Task WriteMetadataAsync(NcryptorMetadata metadata, Stream targetStream, CancellationToken cancellationToken = default);
        Task<NcryptorMetadata> ReadMetadataAsync(Stream sourceStream, CancellationToken cancellationToken = default);
    }
}
