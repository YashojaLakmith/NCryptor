using FluentAssertions;

using Moq;

namespace NCryptor.Core.UnitTests
{
    public class NCryptorTaskUnitTest
    {
        private readonly Mock<ICryptoService> _cryptoSrvStub = new();
        private readonly Mock<IStreamProvider> _streamProviderStub = new();
        private readonly Mock<IKeyMaterial> _keyMaterialStub = new();
        private readonly Mock<ILogger> _loggerStub = new();

        [Fact]
        public async Task EncryptAsync_WithSuccessfullCompletion_ReturnsCompletedTask()
        {
            _cryptoSrvStub.Setup(stub => stub.EncryptAsync(It.IsAny<CancellationToken>()))
                                .Returns(Task.CompletedTask)
                                .Verifiable();

            var obj = CreateNewNCryptorTask();
            await obj.HandleEncryptAsync();

            _cryptoSrvStub.Verify();
        }

        [Fact]
        public async Task DecryptAsync_WithSuccessfullCompletion_ReturnsCompletedTask()
        {
            _cryptoSrvStub.Setup(stub => stub.DecryptAsync(It.IsAny<CancellationToken>()))
                                .Returns(Task.CompletedTask)
                                .Verifiable();

            var obj = CreateNewNCryptorTask();
            await obj.HandleDecryptAsync();

            _cryptoSrvStub.Verify();
        }

        [Fact]
        public async Task EncryptAsync_OnCancellation_ThrowsOpCancelledException()
        {
            _cryptoSrvStub.Setup(stub => stub.EncryptAsync(It.IsAny<CancellationToken>()))
                                .Throws(new OperationCanceledException());

            var obj = CreateNewNCryptorTask();
            Func<Task> act = () => obj.HandleEncryptAsync();

            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task DecryptAsync_OnCancellation_ThrowsOpCancelledException()
        {
            _cryptoSrvStub.Setup(stub => stub.DecryptAsync(It.IsAny<CancellationToken>()))
                                .Throws(new OperationCanceledException());
            
            var obj = CreateNewNCryptorTask();
            Func<Task> act = () => obj.HandleDecryptAsync();
            
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        private NCryptorTask CreateNewNCryptorTask()
        {
            return new NCryptorTask(_cryptoSrvStub.Object, _keyMaterialStub.Object, _loggerStub.Object, _streamProviderStub.Object);
        }
    }
}