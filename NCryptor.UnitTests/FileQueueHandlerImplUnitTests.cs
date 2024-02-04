using Moq;
using NCryptor.Crypto;
using NCryptor.Helpers;
using NCryptor.Metadata;
using NCryptor.Streams;
using NUnit.Framework;

namespace NCryptor.UnitTests
{
    [TestFixture]
    public class FileQueueHandlerImplUnitTests
    {
        private readonly Mock<ISymmetricCryptoService> _cryptoServiceStub = new();
        private readonly Mock<IMetadataHandler> _metadataHandlerStub = new();
        private readonly Mock<IFileStreamFactory> _streamFactoryStub = new();
        private readonly Mock<IFileServices> _fileServicesStub = new();
        private readonly Mock<IKeyDerivationServices> _keyDerivationServicesStub = new();
    }
}
