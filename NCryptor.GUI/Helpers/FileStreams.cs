using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NCryptor.GUI.Helpers
{
    /// <summary>
    /// Contains methods for working with the file streams.
    /// </summary>
    internal class FileStreams
    {
        /// <summary>
        /// Writes the data in the given order to the stream starting from the given position.
        /// </summary>
        /// <returns>The position of the stream after the data was written.</returns>
        internal static async Task<long> WriteMetadataAsync(Stream targetStream, long startPosition, IEnumerable<byte[]> metadata, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            targetStream.Position = startPosition;

            foreach (var entry in metadata)
            {
                if(entry is null)
                {
                    continue;
                }

                await targetStream.WriteAsync(entry, 0, entry.Length, cancellationToken);
            }

            return targetStream.Position;
        }

        /// <summary>
        /// Reads the IV, Salt and the Verification tag from the stream.
        /// </summary>
        /// <returns>A tuple containing the IV, Salt and the Verification tag.</returns>
        internal static async Task<(byte[], byte[], byte[])> ReadMetadataAsync(Stream inputStream, long startPosition, CancellationToken cancellationToken = default)
        {
            // 1.IV, 2.Salt, 3.Tag

            cancellationToken.ThrowIfCancellationRequested();

            inputStream.Position = startPosition;
            var iv = new byte[16];
            var salt = new byte[32];
            var tag = new byte[32];

            await inputStream.ReadAsync(iv, 0, 16, cancellationToken);
            await inputStream.ReadAsync(salt, 0, 32, cancellationToken);
            await inputStream.ReadAsync(tag, 0, 32, cancellationToken);

            return (iv, salt, tag);
        }
    }
}
