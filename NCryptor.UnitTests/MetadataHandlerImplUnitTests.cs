using System.Security.Cryptography;
using FluentAssertions;
using NCryptor.Metadata;
using NUnit.Framework;

namespace NCryptor.UnitTests
{
    [TestFixture]
    public class MetadataHandlerImplUnitTests
    {
        [Test]
        public async Task ReadMetadataAsyncAndWriteMetadataAsync_ReadingAfterWritingToAStream_ShouldReturnCorrectRepresentation()
        {
            var handler = new MetadataHandlerImpl();
            using var writeMetadata = CreateRandomMetadata();
            using var stream = new MemoryStream();

            await handler.WriteMetadataAsync(writeMetadata, stream);
            using var readMetadata = await handler.ReadMetadataAsync(stream);
            var result = writeMetadata.Equals(readMetadata);

            result.Should().BeTrue();
        }

        private static NcryptorMetadata CreateRandomMetadata()
        {
            var salt = RandomNumberGenerator.GetBytes(32);
            var iv = RandomNumberGenerator.GetBytes(16);
            var tag = RandomNumberGenerator.GetBytes(32);

            return NcryptorMetadata.Create(tag, salt, iv);
        }
    }
}
