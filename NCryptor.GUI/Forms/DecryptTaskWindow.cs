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
    internal class DecryptTaskWindow : StatusWindow
    {
        public DecryptTaskWindow(IParentWindowAccess parentWindow, IEnumerable<string> paths, SymmetricAlgorithm alg, string outputDir, byte[] key) : base(parentWindow, paths, alg, outputDir, key)
        {
            Text = "Decryption in progress";
        }

        protected override async Task BeginTask()
        {
            var count = _paths.Count;
            var timer = Stopwatch.StartNew();
            const int BUFFER_SIZE = 81920;

            for (int i = 0; i < count; i++)
            {
                var outputPath = GetOutputFilePath(_paths[i]);

                try
                {
                    progressBar.Value = 0;
                    label_Status.Text = $"File {i + 1} of {count}";
                    
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (!File.Exists(_paths[i]))
                    {
                        LogToWindow($"{_paths[i]} not found");
                        continue;
                    }
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Decrypting {_paths[i]}");

                    using (var fsIn = new FileStream(_paths[i], FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // 1.IV, 2.Salt, 3.Tag
                        (var iv, var salt, var tag) = await FileStreams.ReadMetadataAsync(fsIn, 0, _cancellationTokenSource.Token);
                        (var encKey, var calculatedTag) = KeyDerivation.GetKeyAndVerificationTag(_key, salt, 32, 32, 100000);

                        if (!CompareByteArrays(tag, calculatedTag))
                        {
                            LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Incorrect key.");
                        }

                        using (var fsOut = new FileStream(outputPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                        {
                            _algorithm.Key = encKey;
                            _algorithm.IV = iv;

                            using (var decryptor = _algorithm.CreateDecryptor())
                            {
                                using (var cs = new CryptoStream(fsOut, decryptor, CryptoStreamMode.Write))
                                {
                                    var buffer = new byte[BUFFER_SIZE];
                                    int bytesRead;

                                    while ((bytesRead = await fsIn.ReadAsync(buffer, 0, BUFFER_SIZE)) > 0)
                                    {
                                        UpdateProgress(fsIn.Position, fsIn.Length);
                                        await cs.WriteAsync(buffer, 0, bytesRead, _cancellationTokenSource.Token);
                                    }
                                    cs.FlushFinalBlock();
                                }
                            }
                        }
                        MemsetArray(encKey);
                    }
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Success");
                }
                catch (OperationCanceledException)
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
                catch (Exception ex)
                {
                    LogToWindow($"{ex.Message}");
                    ClearUnfinishedFiles(outputPath);
                }
                finally
                {
                }
            }

            timer.Stop();
               
            progressBar.Value = 0;
            Text = "Completed";
            label_Status.Text = "Completed";
            btn_Cancel.Enabled = false;
            _isInProgress = false;
        }

        protected override string GetOutputFilePath(string inputFile)
        {
            var name = Path.GetFileNameWithoutExtension(Path.GetFileName(inputFile));
            return Path.Combine(_outputDir, name);
        }
    }
}
