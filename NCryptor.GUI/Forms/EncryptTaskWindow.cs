using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

using NCryptor.GUI.Crypto;
using NCryptor.GUI.Helpers;

namespace NCryptor.GUI.Forms
{
    internal class EncryptTaskWindow : StatusWindow
    {
        public EncryptTaskWindow(IParentWindowAccess parentWindow, IEnumerable<string> paths, SymmetricAlgorithm alg, string outputDir, byte[] key) : base(parentWindow, paths, alg, outputDir, key)
        {
            Text = "Encryption in progress";
        }

        protected override async Task BeginTask()
        {
            var timer = Stopwatch.StartNew();
            var count = _paths.Count;
            var keySize = _algorithm.KeySize / 8;
            const int BUFFER_SIZE = 81920;

            for(int i = 0; i < count; i++)
            {
                progressBar.Value = 0;
                label_Status.Text = $"File {i + 1} of {count}";
                string outputPath = GetOutputFilePath(_paths[i]);

                try
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (!File.Exists(_paths[i]))
                    {
                        LogToWindow($"{_paths[i]} not found");
                        continue;
                    }

                    ChangeFileNameIfExists(outputPath);
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Encrypting {_paths[i]}");

                    var salt = RNG.GenRandomBytes(32);
                    (var encKey, var tag) = KeyDerivation.GetKeyAndVerificationTag(_key, salt, keySize, _tagSize, 100000);
                    _algorithm.Key = encKey;
                    _algorithm.GenerateIV();

                    var metadata = new List<byte[]>() { _algorithm.IV, salt, tag };

                    using (var streamIn = new FileStream(_paths[i], FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using(var streamOut = new FileStream(outputPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                        {
                            await FileStreams.WriteMetadataAsync(streamOut, 0, metadata, _cancellationTokenSource.Token);

                            using (var encryptor = _algorithm.CreateEncryptor())
                            {
                                using(var cs = new CryptoStream(streamOut, encryptor, CryptoStreamMode.Write))
                                {
                                    byte[] buffer = new byte[BUFFER_SIZE];
                                    int bytesRead;

                                    while ((bytesRead = await streamIn.ReadAsync(buffer, 0, BUFFER_SIZE, _cancellationTokenSource.Token)) > 0)
                                    {
                                        UpdateProgress(streamIn.Position, streamIn.Length);
                                        await cs.WriteAsync(buffer, 0, bytesRead, _cancellationTokenSource.Token);
                                    }
                                    cs.FlushFinalBlock();
                                }
                            }
                        }
                    }
                    ExternalMethods.ZeroMemset(encKey);
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Success");
                }
                catch(OperationCanceledException)
                {
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Operation cancelled by the user");
                    ClearUnfinishedFiles(outputPath);

                    progressBar.Value = 0;
                    Text = "Cancelled";
                    label_Status.Text = "Cancelled";
                    btn_Cancel.Enabled = false;
                    _isInProgress = false;

                    return;

                }
                catch(Exception aex)
                {
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Error: {aex.Message}");
                    ClearUnfinishedFiles(outputPath);
                    continue;
                }
                finally
                {
                }
            }
            progressBar.Value = 100;
            Text = "Completed";
            label_Status.Text = "Completed";
            btn_Cancel.Enabled = false;
            _isInProgress = false;
        }

        protected override string GetOutputFilePath(string inputFile)
        {
            var name = Path.GetFileName(inputFile);
            name += ".NCRYPT";
            return Path.Combine(_outputDir, name);
        }
    }
}
