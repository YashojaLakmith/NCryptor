using FluentAssertions;
using NCryptor.Metadata;
using System.Security.Cryptography;

using NUnit.Framework;

namespace NCryptor.UnitTests
{
    [TestFixture]
    public class NCryptorMetadataUnitTests
    {
        [Test]
        public void Equals_WithAObjectWithSameMetadata_ShouldReturnTrue()
        {
            var salt = RandomNumberGenerator.GetBytes(32);
            var iv = RandomNumberGenerator.GetBytes(16);
            var tag = RandomNumberGenerator.GetBytes(32);
            using var metadata1 = NcryptorMetadata.Create(tag, salt, iv);
            using var metadata2 = NcryptorMetadata.Create(tag, salt, iv);

            var result = metadata1.Equals(metadata2);

            result.Should().BeTrue();
        }

        [Test]
        public void Equals_WithAObjectWithDifferentMetadata_ShouldReturnFalse()
        {
            using var metadata1 = CreateRandomMetadata();
            using var metadata2 = CreateRandomMetadata();

            var result = metadata1.Equals(metadata2);

            result.Should().BeFalse();
        }

        [Test]
        public void EqualOperator_WithAObjectWithSameMetadata_ShouldReturnTrue()
        {
            var salt = RandomNumberGenerator.GetBytes(32);
            var iv = RandomNumberGenerator.GetBytes(16);
            var tag = RandomNumberGenerator.GetBytes(32);
            using var metadata1 = NcryptorMetadata.Create(tag, salt, iv);
            using var metadata2 = NcryptorMetadata.Create(tag, salt, iv);

            var result = metadata1 == metadata2;

            result.Should().BeTrue();
        }

        [Test]
        public void EqualOperator_WithAObjectWithDifferentMetadata_ShouldReturnFalse()
        {
            using var metadata1 = CreateRandomMetadata();
            using var metadata2 = CreateRandomMetadata();

            var result = metadata1 == metadata2;

            result.Should().BeFalse();
        }

        [Test]
        public void InEqualOperator_WithAObjectWithSameMetadata_ShouldReturnFalse()
        {
            var salt = RandomNumberGenerator.GetBytes(32);
            var iv = RandomNumberGenerator.GetBytes(16);
            var tag = RandomNumberGenerator.GetBytes(32);
            using var metadata1 = NcryptorMetadata.Create(tag, salt, iv);
            using var metadata2 = NcryptorMetadata.Create(tag, salt, iv);

            var result = metadata1 != metadata2;

            result.Should().BeFalse();
        }

        [Test]
        public void InEqualOperator_WithAObjectWithDifferentMetadata_ShouldReturnTrue()
        {
            using var metadata1 = CreateRandomMetadata();
            using var metadata2 = CreateRandomMetadata();

            var result = metadata1 != metadata2;

            result.Should().BeTrue();
        }

        [Test]
        public void GetHashCode_WithAObjectWithSameMetadata_ShouldReturnSameValues()
        {
            var salt = RandomNumberGenerator.GetBytes(32);
            var iv = RandomNumberGenerator.GetBytes(16);
            var tag = RandomNumberGenerator.GetBytes(32);
            using var metadata1 = NcryptorMetadata.Create(tag, salt, iv);
            using var metadata2 = NcryptorMetadata.Create(tag, salt, iv);

            var hashCode1 = metadata1.GetHashCode();
            var hashCode2 = metadata2.GetHashCode();

            hashCode1.Should().Be(hashCode2);
        }

        [Test]
        public void DisposeMethod_ShouldClearAllProperties()
        {
            var metadata = CreateRandomMetadata();

            metadata.Dispose();

            metadata.Salt.All(x => x == 0)
                .Should().BeTrue();
            metadata.IV.All(x => x == 0)
                .Should().BeTrue();
            metadata.VerificationTag.All(x => x == 0)
                .Should().BeTrue();
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
