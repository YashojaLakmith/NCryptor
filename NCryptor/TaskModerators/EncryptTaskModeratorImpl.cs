using NCryptor.Crypto;
using NCryptor.Events;
using NCryptor.Helpers;
using NCryptor.Metadata;
using NCryptor.Streams;

namespace NCryptor.TaskModerators
{
    public class EncryptTaskModeratorImpl : IEncryptTaskModerator
    {
        private readonly ISymmetricCryptoService _cryptoService;
        private readonly IMetadataHandler _metadataHandler;
        private readonly IFileStreamFactory _streamFactory;
        private readonly IFileServices _fileServices;
        private readonly IKeyDerivationServices _keyServices;
        private readonly ITaskModeratorEventService _eventServices;
        private readonly List<string> _fileList;
        private readonly string _outputDirectory;
        private readonly byte[] _userKey;
        private readonly CancellationToken _cancellationToken;

        public EncryptTaskModeratorImpl(ISymmetricCryptoService cryptoService,
        IMetadataHandler metadataHandler,
        IFileStreamFactory streamFactory,
        IFileServices fileServices,
        IKeyDerivationServices keyDerivationServices,
        ITaskModeratorEventService eventService,
        ManualModeratorParameters moderatorParameters)
        {
            _cryptoService = cryptoService;
            _metadataHandler = metadataHandler;
            _streamFactory = streamFactory;
            _fileServices = fileServices;
            _keyServices = keyDerivationServices;
            _eventServices = eventService;
            _fileList = moderatorParameters.FilePathCollection.ToList();
            _outputDirectory = moderatorParameters.OutputDirectory;
            _userKey = moderatorParameters.UserKey;
            _cancellationToken = moderatorParameters.CancellationToken;
        }

        public async Task ModerateFileEncryptionAsync()
        {
            try
            {
                for (var i = 0; i < _fileList.Count; i++)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    await EncryptCurrentFileAsync(i);
                }
            }
            catch (OperationCanceledException)
            {
                _eventServices.CancellationEvent();
                return;
            }
            catch (Exception ex)
            {
                _eventServices.CompletionDueToErrorEvent(ex.Message);
                return;
            }

            _eventServices.CompletionDueToSuccessEvent();
        }

        private async Task EncryptCurrentFileAsync(int indexOfFile)
        {
            var inputFilePath = _fileList[indexOfFile];
            var fullOutputPath = _fileServices.CreateEncryptedFilePath(inputFilePath, _outputDirectory);

            try
            {
                if (!_fileServices.CheckFileExistance(inputFilePath))
                {
                    _eventServices.FileNotFoundEvent(inputFilePath);
                    return;
                }

                _eventServices.BeginOfFileEncryptionEvent(inputFilePath, indexOfFile, _fileList.Count);
                await TryEncryptInputFileAsync(inputFilePath, fullOutputPath);
                _eventServices.SuccessfulCompletionEvent();
            }
            catch (OperationCanceledException)
            {
                _fileServices.DeleteFileIfExists(fullOutputPath);
                throw;
            }
            catch (Exception ex)
            {
                _fileServices.DeleteFileIfExists(fullOutputPath);
                _eventServices.ErrorEvent(ex.Message);
            }
        }

        private async Task TryEncryptInputFileAsync(string inputFilePath, string outputFilePath)
        {
            await using var fsIn = _streamFactory.CreateReadFileStream(inputFilePath);
            await using var fsOut = _streamFactory.CreateWriteFileStream(outputFilePath);

            var salt = _keyServices.GenerateRandomSalt();
            var iv = _keyServices.GenerateRandomIv();
            var (encryptionKey, tag) = _keyServices.DeriveKeyAndVerificationTag(_userKey, salt);

            using var metadata = NcryptorMetadata.Create(salt, tag, iv);
            await _metadataHandler.WriteMetadataAsync(metadata, fsOut, _cancellationToken);

            await _cryptoService.EncryptAsync(fsIn, fsOut, encryptionKey, iv, _cancellationToken);

            Array.Clear(encryptionKey);
        }
    }
}
