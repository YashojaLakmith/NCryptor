using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using NCryptor.GUI.Crypto;
using NCryptor.GUI.Events;
using NCryptor.GUI.Metadata;
using NCryptor.GUI.Options;
using NCryptor.GUI.Streams;

namespace NCryptor.GUI.FileQueueHandlers
{
    internal class FileQueueHandler : IFileQueueHandler
    {
        private bool disposedValue;
        private readonly ISymmetricCryptoService _cryptoService;
        private readonly IMetadataHandler _metadataHandler;
        private readonly IFileStreamFactory _streamFactory;
        private readonly NCryptorOptions _options;
        private readonly List<string> _filePaths;
        private readonly string _outputDirectory;
        private readonly byte[] _key;
        private readonly CancellationToken _cancellationToken;

        public event EventHandler<ProgressPercentageReportedEventArgs> ProgressPercentageReported;
        public event EventHandler<LogEmittedEventArgs> LogEmitted;
        public event EventHandler<ProcessingFileCountEventArgs> ProcessingFileCountReported;
        public event EventHandler<TaskFinishedEventArgs> TaskFinished;

        public FileQueueHandler(ISymmetricCryptoService cryptoService, IMetadataHandler metadataHandler, IFileStreamFactory streamFactory, NCryptorOptions options, IEnumerable<string> fileList, string outputDirectory, byte[] key, CancellationToken cancellationToken)
        {
            _cryptoService = cryptoService;
            _metadataHandler = metadataHandler;
            _streamFactory = streamFactory;
            _options = options;
            _filePaths = fileList.ToList();
            _outputDirectory = outputDirectory;
            _key = key;
            _cancellationToken = cancellationToken;

            _cryptoService.ProgressPercentageReported += OnProgressReportedByCryptoService;
        }

        public async Task EncryptTheFilesAsync()
        {
            var count = _filePaths.Count;
            var timer = Stopwatch.StartNew();

            for(int i = 0; i < count; i++)
            {
                ReportCurrentlyProcessingFileIndex(new ProcessingFileCountEventArgs(count, i + 1));
                var currentFilePath = _filePaths[i];
                var outputFilePath = GetEncryptionOutputFilePath(currentFilePath);

                try
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    ReportProgressPercentage(new ProgressPercentageReportedEventArgs(0));

                    if (!CheckFileExistance(currentFilePath))
                    {
                        EmitALog(new LogEmittedEventArgs($"{currentFilePath} not found"));
                    }

                    EmitALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Decrypting {currentFilePath}"));

                    using (var fsIn = _streamFactory.CreateFileStream(currentFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var fsOut = _streamFactory.CreateFileStream(outputFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                        {
                            byte[] iv = GenerateRandomBytes(_cryptoService.IVSizeInBytes);
                            byte[] salt = GenerateRandomBytes(32);
                            byte[] encryptionKey = DeriveKey(_key, salt, _cryptoService.KeySizeInBytes, _options.KeyDerivationIterations);
                            byte[] verificationTag = DeriveVerificationTag(encryptionKey, _options.VerificationTagLength);

                            using (var metadata = NcryptorMetadata.Create(verificationTag, salt, iv))
                            {
                                await _metadataHandler.WriteMetadataAsync(metadata, fsOut, _cancellationToken);
                                await _cryptoService.EncryptAsync(fsIn, fsOut, encryptionKey, iv, _cancellationToken);
                            }

                            EmitALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Success"));
                            Array.Clear(encryptionKey, 0, encryptionKey.Length);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    timer.Stop();

                    EmitALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Operation cancelled by the user"));
                    ReportTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.CancelledByUser));
                    DeleteFile(outputFilePath);

                    return;
                }
                catch (Exception ex)
                {
                    EmitALog(new LogEmittedEventArgs($"{ex.Message}"));
                    DeleteFile(outputFilePath);
                    continue;
                }
            }

            timer.Stop();
            ReportProgressPercentage(new ProgressPercentageReportedEventArgs(100));
            ReportTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.RanToSuccess));
        }

        public async Task DecryptTheFilesAsync()
        {
            var timer = Stopwatch.StartNew();
            var count = _filePaths.Count;

            for(int i = 0; i < count; i++)
            {
                ReportCurrentlyProcessingFileIndex(new ProcessingFileCountEventArgs(count, i + 1));
                var currentFilePath = _filePaths[i];
                var outputFilePath = GetDecryptionOutputFilePath(currentFilePath);

                try
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    ReportProgressPercentage(new ProgressPercentageReportedEventArgs(0));

                    if (!CheckFileExistance(currentFilePath))
                    {
                        EmitALog(new LogEmittedEventArgs($"{currentFilePath} not found"));
                    }

                    EmitALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Decrypting {currentFilePath}"));

                    using (var fsIn = _streamFactory.CreateFileStream(currentFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var metadata = await _metadataHandler.ReadMetadataAsync(fsIn, _cancellationToken))
                        {
                            var decryptionKey = DeriveKey(_key, metadata.Salt, _cryptoService.KeySizeInBytes, _options.KeyDerivationIterations);
                            var calculatedVerificationTag = DeriveVerificationTag(decryptionKey, _options.VerificationTagLength);

                            if (!calculatedVerificationTag.SequenceEqual(metadata.VerificationTag))
                            {
                                EmitALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Incorrect key."));
                                continue;
                            }

                            using (var fsOut = _streamFactory.CreateFileStream(outputFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                            {
                                await _cryptoService.DecryptAsync(fsIn, fsOut, decryptionKey, metadata.IV, _cancellationToken);

                                EmitALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Success"));
                                Array.Clear(decryptionKey, 0, decryptionKey.Length);
                            }
                        }
                    }                    
                }
                catch (OperationCanceledException)
                {
                    timer.Stop();

                    EmitALog(new LogEmittedEventArgs($"{timer.Elapsed:hh\\:mm\\:ss}: Operation cancelled by the user"));
                    ReportTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.CancelledByUser));
                    DeleteFile(outputFilePath);

                    return;
                }
                catch (Exception ex)
                {
                    EmitALog(new LogEmittedEventArgs($"{ex.Message}"));
                    DeleteFile(outputFilePath);
                    continue;
                }
            }
            timer.Stop();
            ReportProgressPercentage(new ProgressPercentageReportedEventArgs(100));
            ReportTaskFinished(new TaskFinishedEventArgs(TaskFinishedDueTo.RanToSuccess));
        }

        public virtual void OnProgressReportedByCryptoService(object sender, ProgressPercentageReportedEventArgs e)
        {
            ReportProgressPercentage(e);
        }

        public virtual void ReportProgressPercentage(ProgressPercentageReportedEventArgs e)
        {
            ProgressPercentageReported?.Invoke(this, e);
        }

        public virtual void EmitALog(LogEmittedEventArgs e)
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

        protected virtual string GetDecryptionOutputFilePath(string filePath)
        {
            var name = Path.GetFileNameWithoutExtension(Path.GetFileName(filePath));
            var fullPath = Path.Combine(_outputDirectory, name);

            if (File.Exists(fullPath))
            {
                fullPath = ChangeFileNameIfExists(fullPath);
            }

            return fullPath;
        }

        protected virtual string GetEncryptionOutputFilePath(string filePath)
        {
            var name = Path.GetFileName(filePath);
            name += ".NCRYPT";

            if (File.Exists(name))
            {
                name = ChangeFileNameIfExists(name);
            }

            return Path.Combine(_outputDirectory, name);
        }

        protected virtual void GenereateRandomBytes(byte[] byteArrayToFill)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(byteArrayToFill);
            }
        }

        protected virtual byte[] GenerateRandomBytes(int numberOfBytes)
        {
            byte[] bytes = new byte[numberOfBytes];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return bytes;
        }

        protected virtual byte[] DeriveKey(byte[] password, byte[] salt, int size, int iterations)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512))
            {
                return pbkdf2.GetBytes(size);
            }
        }

        protected virtual byte[] DeriveVerificationTag(byte[] key, int verificationTagSize)
        {
            using (var hash = SHA512.Create())
            {
                var buffer = key.Skip(key.Length / 2).ToArray();
                var tag = hash.ComputeHash(buffer);
                Array.Clear(buffer, 0, buffer.Length);

                return tag;
            }
        }

        protected virtual string ChangeFileNameIfExists(string path)
        {
            if (!File.Exists(path))
            {
                return path;
            }

            var ext = Path.GetExtension(path);
            var file = Path.GetFileNameWithoutExtension(path);

            file += $" ({DateTime.Now})";
            return Path.ChangeExtension(file, ext);
        }

        protected virtual bool CheckFileExistance(string path)
        {
            return File.Exists(path);
        }

        protected virtual void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
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
