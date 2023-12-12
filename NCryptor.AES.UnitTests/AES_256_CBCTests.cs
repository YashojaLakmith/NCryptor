using System.Security.Cryptography;

using AES;

using FluentAssertions;

using Moq;

using NCryptor.Core;

namespace NCryptor.AES.UnitTests
{
    public class AES_256_CBCTests
    {
        private Mock<IStreamProvider> _streamStub = new();
        private Mock<IAESKeyMaterial> _keyMaterialStub = new();

        [Fact]
        public void Ctor_OnNullKey_ThrowsArgumentNullException()
        {
            _keyMaterialStub.Reset();

            byte[] key = null;
            _keyMaterialStub.SetupGet(x => x.Key).Returns(key);
            _keyMaterialStub.SetupGet(x => x.IV).Returns(GenRandomBytes(16));

            var act = () => CreateObject();

            act.Should()
                    .Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_OnNullIV_ThrowsArgumentNullException()
        {
            _keyMaterialStub.Reset();

            byte[] iv = null;
            _keyMaterialStub.SetupGet(x => x.Key).Returns(GenRandomBytes(32));
            _keyMaterialStub.SetupGet(x => x.IV).Returns(iv);

            var act = () => CreateObject();

            act.Should()
                    .Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_OnKeyLengthLessThan32Bytes_ThrowsArgumentOutOfRangeException()
        {
            _keyMaterialStub.Reset();

            _keyMaterialStub.SetupGet(x => x.Key).Returns(GenRandomBytes(31));
            _keyMaterialStub.SetupGet(x => x.IV).Returns(GenRandomBytes(16));

            var act = () => CreateObject();

            act.Should()
                    .Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Ctor_OnIVLengthLessThan16Bytes_ThrowsArgumentOutOfRangeException()
        {
            _keyMaterialStub.Reset();

            _keyMaterialStub.SetupGet(x => x.Key).Returns(GenRandomBytes(32));
            _keyMaterialStub.SetupGet(x => x.IV).Returns(GenRandomBytes(15));

            var act = () => CreateObject();

            act.Should()
                    .Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Ctor_OnKeyLengthHigherThan32Bytes_ThrowsArgumentOutOfRangeException()
        {
            _keyMaterialStub.Reset();

            _keyMaterialStub.SetupGet(x => x.Key).Returns(GenRandomBytes(33));
            _keyMaterialStub.SetupGet(x => x.IV).Returns(GenRandomBytes(16));

            var act = () => CreateObject();

            act.Should()
                    .Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Ctor_OnIVLengthHigherThan16Bytes_ThrowsArgumentOutOfRangeException()
        {
            _keyMaterialStub.Reset();

            _keyMaterialStub.SetupGet(x => x.Key).Returns(GenRandomBytes(32));
            _keyMaterialStub.SetupGet(x => x.IV).Returns(GenRandomBytes(17));

            var act = () => CreateObject();

            act.Should()
                    .Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task EncryptAsync_OnCancellationTrigger_ThrowsOperationCancelledException()
        {
            _keyMaterialStub.Reset();
            SetupValidKeyAndIv();
            var obj = CreateObject();
            var tokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));

            await Task.Delay(200);
            var act = () => obj.EncryptAsync(tokenSource.Token);

            await act.Should()
                        .ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task DecryptAsync_OnCancellationTrigger_ThrowsOperationCancelledException()
        {
            _keyMaterialStub.Reset();
            SetupValidKeyAndIv();
            var obj = CreateObject();
            var tokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));

            await Task.Delay(200);
            var act = () => obj.DecryptAsync(tokenSource.Token);

            await act.Should()
                        .ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task EncryptAsync_OnCompletion_OutStreamContainsCiphertext()
        {
            // AES-256-CBC
            // Padding: PKCS7
            // Key: C29C1E86767BA2310C4D74D6EAF17173951F6BC8802DCF96E1302D2669A5A7DF
            // IV: 244E1CE79FBA449407FBA75C47556DDD
            // Plain: AA18B7EF6802D8150F4447A7C641B2CF
            // Cipher: 2A748F7F5EE4EBFF81087B8EF6C43BACCAFA18862005C623ADDBFC3405EF9384

            _keyMaterialStub.Reset();
            _streamStub.Reset();

            var key = Convert.FromHexString("C29C1E86767BA2310C4D74D6EAF17173951F6BC8802DCF96E1302D2669A5A7DF");
            var iv = Convert.FromHexString("244E1CE79FBA449407FBA75C47556DDD");
            var plainBytes = Convert.FromHexString("AA18B7EF6802D8150F4447A7C641B2CF");
            var cipherBytes = Convert.FromHexString("2A748F7F5EE4EBFF81087B8EF6C43BACCAFA18862005C623ADDBFC3405EF9384");

            _keyMaterialStub.SetupGet(x => x.Key).Returns(key);
            _keyMaterialStub.SetupGet(x => x.IV).Returns(iv);

            byte[] outStreamBytes;

            using var inStream = new MemoryStream(plainBytes);
            using var outStream = new MemoryStream();
            inStream.Position = 0;
            _streamStub.SetupGet(x => x.InputStream).Returns(inStream);
            _streamStub.SetupGet(x => x.OutputStream).Returns(outStream);

            using var obj = CreateObject();
            await obj.EncryptAsync();

            outStreamBytes = outStream.ToArray();
            outStreamBytes.Should()
                            .BeEquivalentTo(cipherBytes);
        }

        [Fact]
        public async Task DecryptAsync_OnCompletion_OutStreamContainsPlaintext()
        {
            // AES-256-CBC
            // Key: C29C1E86767BA2310C4D74D6EAF17173951F6BC8802DCF96E1302D2669A5A7DF
            // IV: 244E1CE79FBA449407FBA75C47556DDD
            // Plain: AA18B7EF6802D8150F4447A7C641B2CF
            // Cipher: 2A748F7F5EE4EBFF81087B8EF6C43BACCAFA18862005C623ADDBFC3405EF9384

            _keyMaterialStub.Reset();
            _streamStub.Reset();

            var key = Convert.FromHexString("C29C1E86767BA2310C4D74D6EAF17173951F6BC8802DCF96E1302D2669A5A7DF");
            var iv = Convert.FromHexString("244E1CE79FBA449407FBA75C47556DDD");
            var plainBytes = Convert.FromHexString("AA18B7EF6802D8150F4447A7C641B2CF");
            var cipherBytes = Convert.FromHexString("2A748F7F5EE4EBFF81087B8EF6C43BACCAFA18862005C623ADDBFC3405EF9384");
            byte[] outStreamBytes;

            using var input = new MemoryStream(cipherBytes);
            using var outStream = new MemoryStream();
            input.Position = 0;
            _streamStub.SetupGet(x => x.InputStream).Returns(input);
            _streamStub.SetupGet(x => x.OutputStream).Returns(outStream);

            _keyMaterialStub.SetupGet(x => x.Key).Returns(key);
            _keyMaterialStub.SetupGet(x => x.IV).Returns(iv);

            using var obj = new AES_256_CBC(_streamStub.Object, _keyMaterialStub.Object);
            await obj.DecryptAsync();

            outStreamBytes = outStream.ToArray();
            outStreamBytes.Should()
                                .BeEquivalentTo(plainBytes);
        }

        [Fact]
        public async void UsingAfterDisposing_ShouldThrowObjectDisposedException()
        {
            _keyMaterialStub.Reset();
            _streamStub.Reset();
            SetupValidKeyAndIv();

            _streamStub.SetupGet(x => x.InputStream).Returns(new MemoryStream());
            _streamStub.SetupGet(x => x.OutputStream).Returns(new MemoryStream());

            var obj = CreateObject();
            obj.Dispose();

            var act = () => obj.EncryptAsync();

            await act.Should()
                            .ThrowAsync<ObjectDisposedException>();
        }

        private AES_256_CBC CreateObject()
        {
            return new AES_256_CBC(_streamStub.Object, _keyMaterialStub.Object);
        }

        private void SetupValidKeyAndIv()
        {
            _keyMaterialStub.SetupGet(x => x.Key).Returns(GenRandomBytes(32));
            _keyMaterialStub.SetupGet(x => x.IV).Returns(GenRandomBytes(16));
        }

        private static byte[] GenRandomBytes(int length)
        {
            return RandomNumberGenerator.GetBytes(length);
        }
    }
}