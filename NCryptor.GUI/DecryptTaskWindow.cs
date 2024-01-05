using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCryptor.GUI
{
    internal class DecryptTaskWindow : StatusWindow
    {
        public DecryptTaskWindow(IParentWindowAccess parentWindow, IEnumerable<string> paths, string outputDir, byte[] key) : base(parentWindow, paths, outputDir, key)
        {
            Text = "Decryption in progress";
        }

        protected override async Task BeginTask()
        {
            var count = _paths.Count;
            var timer = Stopwatch.StartNew();

            for(int i = 0; i < count; i++)
            {
                var outputPath = GetOutputFilePath(_paths[i]);

                try
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
                    //iv, salt, tag

                    using(var fsIn = new FileStream(_paths[i], FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Decrypting {_paths[i]}");

                        var salt = new byte[32];
                        var iv = new byte[16];
                        var verifyTag = new byte[32];

                        fsIn.Position = 0;
                        await fsIn.ReadAsync(iv, 0, 16, _cancellationTokenSource.Token);
                        await fsIn.ReadAsync(salt, 0, 32, _cancellationTokenSource.Token);
                        await fsIn.ReadAsync(verifyTag, 0, 32, _cancellationTokenSource.Token);

                        var keyM = DeriveKey(_key, salt, 64);
                        var key = keyM.Take(32).ToArray();
                        var calculatedTag = keyM.Skip(32).ToArray();

                        if(!CompareByteArrays(verifyTag, calculatedTag))
                        {
                            LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Incorrect key.");
                        }

                        using (var fsOut = new FileStream(outputPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                        {
                            using (var aesAlg = Aes.Create())
                            {
                                aesAlg.KeySize = 256;
                                aesAlg.Mode = CipherMode.CBC;
                                aesAlg.Padding = PaddingMode.PKCS7;
                                aesAlg.Key = key;
                                aesAlg.IV = iv;

                                using (var decryptor = aesAlg.CreateDecryptor())
                                {
                                    using(var cs = new CryptoStream(fsOut, decryptor, CryptoStreamMode.Write))
                                    {
                                        const int BUFFER_SIZE = 81920;
                                        var buffer = new byte[BUFFER_SIZE];
                                        int bytesRead;

                                        while((bytesRead = await fsIn.ReadAsync(buffer, 0, BUFFER_SIZE)) > 0)
                                        {
                                            UpdateProgress(fsIn.Position, fsIn.Length);
                                            await cs.WriteAsync(buffer, 0, bytesRead, _cancellationTokenSource.Token);
                                        }
                                        cs.FlushFinalBlock();
                                    }
                                }
                            }
                        }
                        ZeroArray(keyM);
                        ZeroArray(key);
                    }
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Success");
                }
                catch(OperationCanceledException ex)
                {
                    LogToWindow($"{timer.Elapsed:hh\\:mm\\:ss}: Operation cancelled by the user");
                    ClearUnfinishedFiles(outputPath);
                }
                catch(Exception ex)
                {
                    LogToWindow($"{ex.Message}");
                    ClearUnfinishedFiles(outputPath);
                }
                finally
                {
                }
            }
            progressBar.Value = 0;
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
