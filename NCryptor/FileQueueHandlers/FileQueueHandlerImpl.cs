using System.Diagnostics;

using NCryptor.Crypto;
using NCryptor.Events.EventArguments;
using NCryptor.Helpers;
using NCryptor.Metadata;
using NCryptor.Streams;

namespace NCryptor.FileQueueHandlers
{
    public class FileQueueHandlerImpl : IFileQueueHandler
    {
        private bool _disposedValue;
        private readonly ISymmetricCryptoService _cryptoService;
        private readonly IMetadataHandler _metadataHandler;
        private readonly IFileStreamFactory _streamFactory;
        private readonly IFileServices _fileServices;
        private readonly IKeyDerivationServices _keyDerivationService;

        public event EventHandler<ProgressPercentageReportedEventArgs>? ProgressPercentageReported;
        public event EventHandler<LogEmittedEventArgs>? LogEmitted;
        public event EventHandler<ProcessingFileCountEventArgs>? ProcessingFileIndexReported;
        public event EventHandler<TaskFinishedEventArgs>? TaskFinished;

        public FileQueueHandlerImpl(ISymmetricCryptoService cryptoService, IMetadataHandler metadataHandler, IFileStreamFactory streamFactory, IFileServices fileServices, IKeyDerivationServices keyDerivationServices)
        {
            _cryptoService = cryptoService;
            _metadataHandler = metadataHandler;
            _streamFactory = streamFactory;
            _fileServices = fileServices;
            _keyDerivationService = keyDerivationServices;

            _cryptoService.ProgressPercentageReported += OnProgressReportedByCryptoService;
        }

        public async Task EncryptTheFilesAsync(IEnumerable<string> fileList, string outputDirectory, byte[] key, CancellationToken cancellationToken)
        {
            var filePaths = fileList.ToList();
            var count = filePaths.Count;
            var timer = Stopwatch.StartNew();

            try
            {
                for (var i = 0; i < count; i++)
                {
                    PublishProcessingFileIndex(new ProcessingFileCountEventArgs(count, i + 1));
                    var currentFilePath = filePaths[i];
                    var outputFilePath = _fileServices.CreateEncryptedFilePath(currentFilePath, outputDirectory);

                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        PublishProgressPercentage(new ProgressPercentageReportedEventArgs(0));

                        if (!_fileServices.CheckFileExistance(currentFilePath))
                        {
                            PublishLog(new LogEmittedEventArgs($"{currentFilePath} not found"));
                            continue;
                        }

                        PublishLog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Encrypting {currentFilePath}"));

                        await using var fsIn = _streamFactory.CreateReadFileStream(currentFilePath);
                        await using var fsOut = _streamFactory.CreateWriteFileStream(outputFilePath);
                        var iv = _keyDerivationService.GenerateRandomIv();
                        var salt = _keyDerivationService.GenerateRandomSalt();
                        var (encryptionKey, verificationTag) = _keyDerivationService.DeriveKeyAndVerificationTag(key, salt);

                        using var metadata = NcryptorMetadata.Create(verificationTag, salt, iv);
                        await _metadataHandler.WriteMetadataAsync(metadata, fsOut, cancellationToken);
                        await _cryptoService.EncryptAsync(fsIn, fsOut, encryptionKey, iv, cancellationToken);

                        PublishLog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Success"));
                        Array.Clear(encryptionKey, 0, encryptionKey.Length);
                    }
                    catch (OperationCanceledException)
                    {
                        timer.Stop();

                        PublishLog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Operation cancelled by the user"));
                        PublishTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.CancelledByUser));
                        _fileServices.DeleteFileIfExists(outputFilePath);

                        return;
                    }
                    catch (Exception ex)
                    {
                        PublishLog(new LogEmittedEventArgs($"{ex.Message}"));
                        _fileServices.DeleteFileIfExists(outputFilePath);

                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                PublishLog(new LogEmittedEventArgs(ex.Message));
                PublishTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.ErrorEncountered));

                return;
            }

            timer.Stop();
            PublishProgressPercentage(new ProgressPercentageReportedEventArgs(100));
            PublishTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.RanToSuccess));
        }

        public async Task DecryptTheFilesAsync(IEnumerable<string> fileList, string outputDirectory, byte[] key, CancellationToken cancellationToken)
        {
            var filePaths = fileList.ToList();
            var timer = Stopwatch.StartNew();
            var count = filePaths.Count;

            try
            {
                for (var i = 0; i < count; i++)
                {
                    PublishProcessingFileIndex(new ProcessingFileCountEventArgs(count, i + 1));
                    var currentFilePath = filePaths[i];
                    var outputFilePath = _fileServices.CreateDecryptedFilePath(currentFilePath, outputDirectory);

                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        PublishProgressPercentage(new ProgressPercentageReportedEventArgs(0));

                        if (!_fileServices.CheckFileExistance(currentFilePath))
                        {
                            PublishLog(new LogEmittedEventArgs($"{currentFilePath} not found"));
                            continue;
                        }

                        PublishLog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Decrypting {currentFilePath}"));

                        await using var fsIn = _streamFactory.CreateReadFileStream(currentFilePath);
                        using var metadata = await _metadataHandler.ReadMetadataAsync(fsIn, cancellationToken);
                        var (decryptionKey, calculatedVerificationTag) = _keyDerivationService.DeriveKeyAndVerificationTag(key, metadata.Salt);

                        if (!calculatedVerificationTag.SequenceEqual(metadata.VerificationTag))
                        {
                            PublishLog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Incorrect key."));
                            continue;
                        }

                        await using var fsOut = _streamFactory.CreateWriteFileStream(outputFilePath);
                        await _cryptoService.DecryptAsync(fsIn, fsOut, decryptionKey, metadata.IV, cancellationToken);

                        PublishLog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Success"));
                        Array.Clear(decryptionKey, 0, decryptionKey.Length);
                    }
                    catch (OperationCanceledException)
                    {
                        timer.Stop();

                        PublishLog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Operation cancelled by the user"));
                        PublishTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.CancelledByUser));
                        _fileServices.DeleteFileIfExists(outputFilePath);

                        return;
                    }
                    catch (Exception ex)
                    {
                        PublishLog(new LogEmittedEventArgs($"{ex.Message}"));
                        _fileServices.DeleteFileIfExists(outputFilePath);
                        continue;
                    }
                }
            }
            catch(Exception ex)
            {
                PublishLog(new LogEmittedEventArgs(ex.Message));
                PublishTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.ErrorEncountered));
                return;
            }

            timer.Stop();
            PublishProgressPercentage(new ProgressPercentageReportedEventArgs(100));
            PublishTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.RanToSuccess));
        }

        protected virtual void OnProgressReportedByCryptoService(object? sender, ProgressPercentageReportedEventArgs e)
            => PublishProgressPercentage(e);

        protected virtual void PublishProgressPercentage(ProgressPercentageReportedEventArgs e)
            => ProgressPercentageReported?.Invoke(this, e);

        protected virtual void PublishLog(LogEmittedEventArgs e)
            => LogEmitted?.Invoke(this, e);

        protected virtual void PublishProcessingFileIndex(ProcessingFileCountEventArgs e)
            => ProcessingFileIndexReported?.Invoke(this, e);

        protected virtual void PublishTaskFinished(TaskFinishedEventArgs e)
            => TaskFinished?.Invoke(this, e);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EncryptionFileQueueHandler()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
