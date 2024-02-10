using NCryptor.Crypto;
using NCryptor.Events;
using NCryptor.Helpers;
using NCryptor.Metadata;
using NCryptor.Streams;

namespace NCryptor.TaskModerators
{
    public class TaskModeratorImpl : ITaskModerator
    {
        private bool _disposedValue;
        private readonly ISymmetricCryptoService _cryptoService;
        private readonly IMetadataHandler _metadataHandler;
        private readonly IFileStreamFactory _streamFactory;
        private readonly IFileServices _fileServices;
        private readonly IKeyDerivationServices _keyDerivationService;
        private readonly ITaskModeratorEventService _eventService;

        public TaskModeratorImpl(ISymmetricCryptoService cryptoService, IMetadataHandler metadataHandler, IFileStreamFactory streamFactory, IFileServices fileServices, IKeyDerivationServices keyDerivationServices, ITaskModeratorEventService eventService)
        {
            _cryptoService = cryptoService;
            _metadataHandler = metadataHandler;
            _streamFactory = streamFactory;
            _fileServices = fileServices;
            _keyDerivationService = keyDerivationServices;
            _eventService = eventService;
        }

        public async Task EncryptTheFilesAsync(IEnumerable<string> fileList, string outputDirectory, byte[] key, CancellationToken cancellationToken)
        {
            var filePaths = fileList.ToList();
            var count = filePaths.Count;

            try
            {
                for (var i = 0; i < count; i++)
                {
                    var currentFilePath = filePaths[i];
                    var outputFilePath = _fileServices.CreateEncryptedFilePath(currentFilePath, outputDirectory);

                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (!_fileServices.CheckFileExistance(currentFilePath))
                        {
                            _eventService.FileNotFoundEvent(currentFilePath);
                            continue;
                        }

                        _eventService.BeginOfFileEncryptionEvent(currentFilePath, i, count);

                        await using var fsIn = _streamFactory.CreateReadFileStream(currentFilePath);
                        await using var fsOut = _streamFactory.CreateWriteFileStream(outputFilePath);
                        var iv = _keyDerivationService.GenerateRandomIv();
                        var salt = _keyDerivationService.GenerateRandomSalt();
                        var (encryptionKey, verificationTag) = _keyDerivationService.DeriveKeyAndVerificationTag(key, salt);

                        using var metadata = NcryptorMetadata.Create(salt, verificationTag, iv);
                        await _metadataHandler.WriteMetadataAsync(metadata, fsOut, cancellationToken);
                        await _cryptoService.EncryptAsync(fsIn, fsOut, encryptionKey, iv, cancellationToken);

                        _eventService.SuccessfulCompletionEvent();
                        Array.Clear(encryptionKey);
                    }
                    catch (OperationCanceledException)
                    {
                        _eventService.CancellationEvent();
                        _fileServices.DeleteFileIfExists(outputFilePath);

                        return;
                    }
                    catch (Exception ex)
                    {
                        _eventService.ErrorEvent(ex.Message);
                        _fileServices.DeleteFileIfExists(outputFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                _eventService.CompletionDueToErrorEvent(ex.Message);

                return;
            }

            _eventService.CompletionDueToSuccessEvent();
        }

        public async Task DecryptTheFilesAsync(IEnumerable<string> fileList, string outputDirectory, byte[] key, CancellationToken cancellationToken)
        {
            var filePaths = fileList.ToList();
            var count = filePaths.Count;

            try
            {
                for (var i = 0; i < count; i++)
                {
                    var currentFilePath = filePaths[i];
                    var outputFilePath = _fileServices.CreateDecryptedFilePath(currentFilePath, outputDirectory);

                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (!_fileServices.CheckFileExistance(currentFilePath))
                        {
                            _eventService.FileNotFoundEvent(currentFilePath);
                            continue;
                        }

                        _eventService.BeginOfFileDecryptionEvent(currentFilePath, i, count);

                        await using var fsIn = _streamFactory.CreateReadFileStream(currentFilePath);
                        using var metadata = await _metadataHandler.ReadMetadataAsync(fsIn, cancellationToken);
                        var (decryptionKey, calculatedVerificationTag) = _keyDerivationService.DeriveKeyAndVerificationTag(key, metadata.Salt);

                        if (!metadata.AreVerificationTagsEqual(calculatedVerificationTag))
                        {
                            _eventService.ErrorEvent(@"Incorrect key");
                            continue;
                        }

                        await using var fsOut = _streamFactory.CreateWriteFileStream(outputFilePath);
                        await _cryptoService.DecryptAsync(fsIn, fsOut, decryptionKey, metadata.IV, cancellationToken);

                        _eventService.SuccessfulCompletionEvent();
                        Array.Clear(decryptionKey);
                    }
                    catch (OperationCanceledException)
                    {
                        _eventService.CancellationEvent();
                        _fileServices.DeleteFileIfExists(outputFilePath);

                        return;
                    }
                    catch (Exception ex)
                    {
                        _eventService.ErrorEvent(ex.Message);
                        _fileServices.DeleteFileIfExists(outputFilePath);
                    }
                }
            }
            catch(Exception ex)
            {
                _eventService.CompletionDueToErrorEvent(ex.Message);
                return;
            }

            _eventService.CompletionDueToSuccessEvent();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                    
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
