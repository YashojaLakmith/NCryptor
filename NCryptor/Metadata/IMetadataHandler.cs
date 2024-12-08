namespace NCryptor.Metadata;

/// <summary>
/// Defines methods for handling application metadata.
/// </summary>
public interface IMetadataHandler
{
    /// <summary>
    /// Asynchronously writes the given metadata to the start of the stream and monitor for cancellation requests.
    /// </summary>
    Task WriteMetadataAsync(NcryptorMetadata metadata, Stream targetStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously reads the metadata begining from the start of the stream and monitor for cancellation requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> wrapping the read metadata.</returns>
    Task<NcryptorMetadata> ReadMetadataAsync(Stream sourceStream, CancellationToken cancellationToken = default);
}
