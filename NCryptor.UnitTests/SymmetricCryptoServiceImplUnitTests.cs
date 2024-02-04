using System.Security.Cryptography;
using FluentAssertions;
using Moq;
using NCryptor.Crypto;

using NUnit.Framework;

namespace NCryptor.UnitTests
{
    [TestFixture]
    public class Tests
    {
        private readonly Mock<SymmetricAlgorithm> _symmetricAlgorithmStub = new();

        [Test]
        public async Task AnyMemberExceptDispose_WhenObjectHasBeenDisposed_ShouldExactlyThrowObjectDisposedException()
        {
            using var aes = Aes.Create();
            var service = CreateObject(aes);
            service.Dispose();

            var encryptFunc = () => service.EncryptAsync(It.IsAny<Stream>(), It.IsAny<Stream>(), It.IsAny<byte[]>(),
                It.IsAny<byte[]>(), It.IsAny<CancellationToken>());
            var decryptFunc = () => service.DecryptAsync(It.IsAny<Stream>(), It.IsAny<Stream>(), It.IsAny<byte[]>(),
                It.IsAny<byte[]>(), It.IsAny<CancellationToken>());
            var ivGetFunc = () => service.IvSizeInBytes;
            var keySizeGetFunc = () => service.KeySizeInBytes;

            await encryptFunc.Should().ThrowExactlyAsync<ObjectDisposedException>();
            await decryptFunc.Should().ThrowExactlyAsync<ObjectDisposedException>();
            ivGetFunc.Should().ThrowExactly<ObjectDisposedException>();
            keySizeGetFunc.Should().ThrowExactly<ObjectDisposedException>();
        }

        [Test]
        public async Task EncryptAsyncAndDecryptAsync_OnCancellationShouldExactlyThrowOperationCancelledException()
        {
            using var aes = Aes.Create();
            using var service = CreateObject(aes);
            var key = aes.Key;
            var iv = aes.IV;
            using var tokenSource = new CancellationTokenSource();
            var bytes = RandomNumberGenerator.GetBytes(128);
            using var msIn = new MemoryStream(bytes);
            using var msOut = new MemoryStream();

            await tokenSource.CancelAsync();

            var encryptFunc = () => service.EncryptAsync(msIn, msOut, key, iv, tokenSource.Token);
            var decryptFunc = () => service.DecryptAsync(msIn, msOut, key, iv, tokenSource.Token);

            await encryptFunc.Should().ThrowExactlyAsync<OperationCanceledException>();
            await decryptFunc.Should().ThrowExactlyAsync<OperationCanceledException>();
        }
            
        private SymmetricCryptoService CreateObject()
            => new(_symmetricAlgorithmStub.Object);

        private SymmetricCryptoService CreateObject(SymmetricAlgorithm alg)
            => new(alg);

        private static Mock<SymmetricCryptoService> CreateStubOfThisObject()
            => new();
    }
}