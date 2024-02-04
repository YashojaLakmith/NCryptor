using System.Diagnostics;

using NCryptor.GUI.Crypto;
using NCryptor.GUI.Events;
using NCryptor.GUI.Helpers;
using NCryptor.GUI.Metadata;
using NCryptor.GUI.Streams;

namespace NCryptor.GUI.FileQueueHandlers
{
    internal class FileQueueHandlerImpl : IFileQueueHandler
    {
        private bool disposedValue;
        private readonly ISymmetricCryptoService _cryptoService;
        private readonly IMetadataHandler _metadataHandler;
        private readonly IFileStreamFactory _streamFactory;
        private readonly IFileServices _fileServices;
        private readonly IKeyDerivationServices _keyDerivationService;

        public event EventHandler<ProgressPercentageReportedEventArgs> ProgressPercentageReported;
        public event EventHandler<LogEmittedEventArgs> LogEmitted;
        public event EventHandler<ProcessingFileCountEventArgs> ProcessingFileCountReported;
        public event EventHandler<TaskFinishedEventArgs> TaskFinished;

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
                for (int i = 0; i < count; i++)
                {
                    ReportCurrentlyProcessingFileIndex(new ProcessingFileCountEventArgs(count, i + 1));
                    var currentFilePath = filePaths[i];
                    var outputFilePath = _fileServices.CreateEncryptedFilePath(currentFilePath, outputDirectory);

                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        ReportProgressPercentage(new ProgressPercentageReportedEventArgs(0));

                        if (!_fileServices.CheckFileExistance(currentFilePath))
                        {
                            PublishALog(new LogEmittedEventArgs($"{currentFilePath} not found"));
                            continue;
                        }

                        PublishALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Encrypting {currentFilePath}"));

                        using (var fsIn = _streamFactory.CreateReadFileStream(currentFilePath))
                        {
                            using (var fsOut = _streamFactory.CreateWriteFileStream(outputFilePath))
                            {
                                byte[] iv = _keyDerivationService.GenerateRandomIV();
                                byte[] salt = _keyDerivationService.GenerateRandomSalt();
                                (var encryptionKey, var verificationTag) = _keyDerivationService.DeriveKeyAndVerificationTag(key, salt);

                                using (var metadata = NcryptorMetadata.Create(verificationTag, salt, iv))
                                {
                                    await _metadataHandler.WriteMetadataAsync(metadata, fsOut, cancellationToken);
                                    await _cryptoService.EncryptAsync(fsIn, fsOut, encryptionKey, iv, cancellationToken);
                                }

                                PublishALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Success"));
                                Array.Clear(encryptionKey, 0, encryptionKey.Length);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        timer.Stop();

                        PublishALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Operation cancelled by the user"));
                        ReportTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.CancelledByUser));
                        _fileServices.DeleteFileIfExists(outputFilePath);

                        return;
                    }
                    catch (Exception ex)
                    {
                        PublishALog(new LogEmittedEventArgs($"{ex.Message}"));
                        _fileServices.DeleteFileIfExists(outputFilePath);

                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                PublishALog(new LogEmittedEventArgs(ex.Message));
                ReportTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.ErrorEncountered));

                return;
            }

            timer.Stop();
            ReportProgressPercentage(new ProgressPercentageReportedEventArgs(100));
            ReportTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.RanToSuccess));
        }

        public async Task DecryptTheFilesAsync(IEnumerable<string> fileList, string outputDirectory, byte[] key, CancellationToken cancellationToken)
        {
            var filePaths = fileList.ToList();
            var timer = Stopwatch.StartNew();
            var count = filePaths.Count;

            try
            {
                for (int i = 0; i < count; i++)
                {
                    ReportCurrentlyProcessingFileIndex(new ProcessingFileCountEventArgs(count, i + 1));
                    var currentFilePath = filePaths[i];
                    var outputFilePath = _fileServices.CreateDecryptedFilePath(currentFilePath, outputDirectory);

                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        ReportProgressPercentage(new ProgressPercentageReportedEventArgs(0));

                        if (!_fileServices.CheckFileExistance(currentFilePath))
                        {
                            PublishALog(new LogEmittedEventArgs($"{currentFilePath} not found"));
                            continue;
                        }

                        PublishALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Decrypting {currentFilePath}"));

                        using (var fsIn = _streamFactory.CreateReadFileStream(currentFilePath))
                        {
                            using (var metadata = await _metadataHandler.ReadMetadataAsync(fsIn, cancellationToken))
                            {
                                (var decryptionKey, var calculatedVerificationTag) = _keyDerivationService.DeriveKeyAndVerificationTag(key, metadata.Salt);

                                if (!calculatedVerificationTag.SequenceEqual(metadata.VerificationTag))
                                {
                                    PublishALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Incorrect key."));
                                    continue;
                                }

                                using (var fsOut = _streamFactory.CreateWriteFileStream(outputFilePath))
                                {
                                    await _cryptoService.DecryptAsync(fsIn, fsOut, decryptionKey, metadata.IV, cancellationToken);

                                    PublishALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Success"));
                                    Array.Clear(decryptionKey, 0, decryptionKey.Length);
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        timer.Stop();

                        PublishALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Operation cancelled by the user"));
                        ReportTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.CancelledByUser));
                        _fileServices.DeleteFileIfExists(outputFilePath);

                        return;
                    }
                    catch (Exception ex)
                    {
                        PublishALog(new LogEmittedEventArgs($"{ex.Message}"));
                        _fileServices.DeleteFileIfExists(outputFilePath);
                        continue;
                    }
                }
            }
            catch(Exception ex)
            {
                PublishALog(new LogEmittedEventArgs(ex.Message));
                ReportTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.ErrorEncountered));
                return;
            }

            timer.Stop();
            ReportProgressPercentage(new ProgressPercentageReportedEventArgs(100));
            ReportTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.RanToSuccess));
        }

        protected virtual void OnProgressReportedByCryptoService(object sender, ProgressPercentageReportedEventArgs e)
        {
            ReportProgressPercentage(e);
        }

        public virtual void ReportProgressPercentage(ProgressPercentageReportedEventArgs e)
        {
            ProgressPercentageReported?.Invoke(this, e);
        }

        public virtual void PublishALog(LogEmittedEventArgs e)
        {
            LogEmitted?.Invoke(this, e);
        }

        public virtual void ReportCurrentlyProcessingFileIndex(ProcessingFileCountEventArgs e)
        {
            ProcessingFileCountReported?.Invoke(this, e);
        }

        public virtual void ReportTaskFinished(TaskFinishedEventArgs e)
        {
            TaskFinished?.Invoke(this, e);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
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
