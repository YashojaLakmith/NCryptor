using System.Text;

using NCryptor.GUI.Factories;
using NCryptor.GUI.Options;

namespace NCryptor.GUI.Forms
{
    /// <summary>
    /// Collects the necessary information with UI and validates the before proceeding to the decryption.
    /// </summary>
    internal class DecryptWindow : OpWindow
    {
        private readonly FileSystemOptions _fileSystemOptions;

        public DecryptWindow(FileSystemOptions fileSystemOptions) : base()
        {
            _fileSystemOptions = fileSystemOptions;
            Text = "Decrypt Files";
        }

        protected override void Btn_BrowseFiles_OnClick(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Title = "Select files to encrypt";
                ofd.Filter = $"Encryptor files (*{_fileSystemOptions.Extension})|*{_fileSystemOptions.Extension}";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    AddToListBox(ofd.FileNames);
                }
            }
            ValidateStartButton();
        }

        protected override async Task BeginTaskAsync()
        {
            var bKey = Encoding.ASCII.GetBytes(textBox_Key.Text);
            var tokenSource = new CancellationTokenSource();
            try
            {
                var factory = new ServiceFactory();

                var handler = factory.CreateFileQueueHandler();
                var progressWindow = factory.CreateStatusWindow(handler, tokenSource, "Decrypting");

                progressWindow.Shown += StatusWindow_OnShow;
                progressWindow.FormClosed += StatusWindow_OnClose;

                progressWindow.Show();
                await handler.DecryptTheFilesAsync(_filePaths, _outputDir, bKey, tokenSource.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                tokenSource.Dispose();
                Array.Clear(bKey, 0, bKey.Length);
            }
        }
    }
}
