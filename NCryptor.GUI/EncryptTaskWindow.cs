using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace NCryptor.GUI
{
    internal class EncryptTaskWindow : StatusWindow
    {
        public EncryptTaskWindow(IParentWindowAccess parentWindow, IEnumerable<string> paths, string outputDir, byte[] key) : base(parentWindow, paths, outputDir, key)
        {
            Text = "Encryption in progress";
        }

        protected override async Task BeginTask()
        {
            var timer = Stopwatch.StartNew();
            var count = _paths.Count;

            for(int i = 0; i < count; i++)
            {
                progressBar.Value = 0;
                label_Status.Text = $"File {i + 1} of {count}";

                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Operation cancelled by the user");
                    break;
                }

                if (!File.Exists(_paths[i]))
                {
                    LogToWindow($"{_paths[i]} not found");
                    continue;
                }

                var outputPath = GetOutputFilePath(_paths[i]);
                var salt = new byte[32];

                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                var keyM = DeriveKey(_key, salt, 64);
                var encKey = keyM.Take(32).ToArray();
                var verifyTag = keyM.Skip(32).ToArray();

                try
                {                
                    using(var streamIn = new FileStream(_paths[i], FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using(var streamOut = new FileStream(outputPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                        {
                            using (var aesAlg = Aes.Create())
                            {
                                aesAlg.KeySize = 256;
                                aesAlg.Mode = CipherMode.CBC;
                                aesAlg.Padding = PaddingMode.PKCS7;
                                aesAlg.Key = encKey;
                                aesAlg.GenerateIV();

                                streamOut.Position = 0;
                                await streamOut.WriteAsync(aesAlg.IV, 0, 16, _cancellationTokenSource.Token);
                                await streamOut.WriteAsync(salt, 0, salt.Length, _cancellationTokenSource.Token);
                                await streamOut.WriteAsync(verifyTag, 0, verifyTag.Length, _cancellationTokenSource.Token);

                                LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Encrypting {_paths[i]}");

                                using (var encryptor = aesAlg.CreateEncryptor())
                                {
                                    using(var cs = new CryptoStream(streamOut, encryptor, CryptoStreamMode.Write))
                                    {
                                        const int BUFFER_SIZE = 81920;
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
                    }                                    
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Success");
                }
                catch(OperationCanceledException _)
                {
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Operation cancelled by the user");
                    ClearUnfinishedFiles(outputPath);

                }
                catch(Exception aex)
                {
                    LogToWindow($"{aex.Message}");
                    ClearUnfinishedFiles(outputPath);
                }
                finally
                {
                    ZeroArray(keyM);
                    ZeroArray(encKey);
                }
            }
            progressBar.Value = 0;            
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
