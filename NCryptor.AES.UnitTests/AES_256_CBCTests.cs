using System.Security.Cryptography;

using AES;

using FluentAssertions;
using FluentAssertions.Equivalency;

using Moq;

using NCryptor.Core;

namespace NCryptor.AES.UnitTests
{
    public class AES_256_CBCTests
    {
        private Mock<IStreamProvider> _streamStub = new();
        private Mock<AESKeyMaterial> _keyMaterialStub = new();

        [Fact]
        public void Ctor_OnNullKey_ThrowsArgumentNullException()
        {
            var keyMaterial = new AESKeyMaterial(null, It.IsAny<byte[]>());

            var act = () => new AES_256_CBC(_streamStub.Object, keyMaterial);

            act.Should()
                    .Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_OnNullIV_ThrowsArgumentNullException()
        {
            var keyMaterial = new AESKeyMaterial(It.IsAny<byte[]>(), null);

            var act = () => new AES_256_CBC(_streamStub.Object, keyMaterial);

            act.Should()
                    .Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_OnKeyLengthLessThan32Bytes_ThrowsArgumentOutOfRangeException()
        {
            var keyMaterial = new AESKeyMaterial(GenRandomBytes(30), It.IsAny<byte[]>());

            var act = () => new AES_256_CBC(_streamStub.Object, keyMaterial);

            act.Should()
                    .Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Ctor_OnIVLengthLessThan16Bytes_ThrowsArgumentOutOfRangeException()
        {
            var keyMaterial = new AESKeyMaterial(It.IsAny<byte[]>(), GenRandomBytes(10));

            var act = () => new AES_256_CBC(_streamStub.Object, keyMaterial);

            act.Should()
                    .Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Ctor_OnKeyLengthHigherThan32Bytes_ThrowsArgumentOutOfRangeException()
        {
            var keyMaterial = new AESKeyMaterial(GenRandomBytes(40), It.IsAny<byte[]>());

            var act = () => new AES_256_CBC(_streamStub.Object, keyMaterial);

            act.Should()
                    .Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Ctor_OnIVLengthHigherThan16Bytes_ThrowsArgumentOutOfRangeException()
        {
            var keyMaterial = new AESKeyMaterial(It.IsAny<byte[]>(), GenRandomBytes(20));

            var act = () => new AES_256_CBC(_streamStub.Object, keyMaterial);

            act.Should()
                    .Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task EncryptAsync_OnCancellationTrigger_ThrowsOperationCancelledException()
        {
            var obj = CreateObject();
            var tokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(2000));

            var act = () => obj.EncryptAsync(tokenSource.Token);

            await act.Should()
                        .ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task DecryptAsync_OnCancellationTrigger_ThrowsOperationCancelledException()
        {
            var obj = CreateObject();
            var tokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(2000));

            var act = () => obj.DecryptAsync(tokenSource.Token);

            await act.Should()
                        .ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task EncryptAsync_OnCompletion_OutStreamContainsCiphertext()
        {
            // mode=aes-256-cbc
            // key=603deb1015ca71be2b73aef0857d77811f352c073b6108d72d9810a30914dff4
            // iv = 000102030405060708090A0B0C0D0E0F
            // plain = 6bc1bee22e409f96e93d7e117393172a
            // cipher = f58c4c04d6e5f1ba779eabfb5f7bfbd6

            var byteKey = Convert.FromHexString("603deb1015ca71be2b73aef0857d77811f352c073b6108d72d9810a30914dff4");
            var byteIv = Convert.FromHexString("000102030405060708090A0B0C0D0E0F");
            var plainBytes = Convert.FromHexString("6bc1bee22e409f96e93d7e117393172a");
            var cipherBytes = Convert.FromHexString("f58c4c04d6e5f1ba779eabfb5f7bfbd6");

            _streamStub.SetupGet(x => x.InputStream).Returns(new MemoryStream(plainBytes));
            _streamStub.SetupGet(x => x.OutputStream).Returns(new MemoryStream());

            _keyMaterialStub.SetupGet(x => x.Key).Returns(byteKey);
            _keyMaterialStub.SetupGet(x => x.IV).Returns(byteIv);

            var obj = CreateObject();

            await obj.EncryptAsync();

            var outStreamBytes = new byte[_streamStub.Object.OutputStream.Length];
            await _streamStub.Object.OutputStream.ReadAsync(outStreamBytes, 0, outStreamBytes.Length);

            outStreamBytes.Should()
                            .BeEquivalentTo(plainBytes, o => o.ComparingByValue<byte>()
                                                                .WithStrictOrdering());
        }

        [Fact]
        public async Task DecryptAsync_OnCompletion_OutStreamContainsPlaintext()
        {
            // mode = aes - 256 -cbc
            // key = 603deb1015ca71be2b73aef0857d77811f352c073b6108d72d9810a30914dff4
            // iv = F58C4C04D6E5F1BA779EABFB5F7BFBD6
            // plain = ae2d8a571e03ac9c9eb76fac45af8e51
            // cipher = 9cfc4e967edb808d679f777bc6702c7d

            var byteKey = Convert.FromHexString("603deb1015ca71be2b73aef0857d77811f352c073b6108d72d9810a30914dff4");
            var byteIv = Convert.FromHexString("F58C4C04D6E5F1BA779EABFB5F7BFBD6");
            var plainBytes = Convert.FromHexString("ae2d8a571e03ac9c9eb76fac45af8e51");
            var cipherBytes = Convert.FromHexString("9cfc4e967edb808d679f777bc6702c7d");

            _streamStub.SetupGet(x => x.InputStream).Returns(new MemoryStream(cipherBytes));
            _streamStub.SetupGet(x => x.OutputStream).Returns(new MemoryStream());

            _keyMaterialStub.SetupGet(x => x.Key).Returns(byteKey);
            _keyMaterialStub.SetupGet(x => x.IV).Returns(byteIv);

            var obj = CreateObject();
            await obj.DecryptAsync();

            var outStreamBytes = new byte[_streamStub.Object.OutputStream.Length];
            await _streamStub.Object.OutputStream.ReadAsync(outStreamBytes, 0, outStreamBytes.Length);
            outStreamBytes.Should()
                                .BeEquivalentTo(cipherBytes, o => o.ComparingByValue<byte>()
                                                                        .WithStrictOrdering());
        }

        [Fact]
        public async void UsingAfterDisposing_ShouldThrowObjectDisposedException()
        {
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

        private static byte[] GenRandomBytes(int length)
        {
            return RandomNumberGenerator.GetBytes(length);
        }
    }
}