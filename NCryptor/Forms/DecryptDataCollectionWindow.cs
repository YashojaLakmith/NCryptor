using System.Text;

using NCryptor.ServiceFactories;
using NCryptor.Options;

namespace NCryptor.Forms
{
    /// <summary>
    /// Collects the necessary information with UI and validates the before proceeding to the decryption.
    /// </summary>
    public class DecryptDataCollectionWindow : BaseDataCollectionWindow
    {
        private readonly FileSystemOptions _fileSystemOptions;

        public DecryptDataCollectionWindow(FileSystemOptions fileSystemOptions) : base()
        {
            _fileSystemOptions = fileSystemOptions;
            Text = @"Decrypt Files";
        }

        protected override void Btn_BrowseFiles_OnClick(object? sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Title = @"Select files to encrypt";
                ofd.Filter = $@"Encryptor files (*{_fileSystemOptions.Extension})|*{_fileSystemOptions.Extension}";
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
            TokenSource = new CancellationTokenSource();
            try
            {
                var factory = new ServiceFactory();

                var handler = factory.CreateFileQueueHandler();
                var statusWindow = factory.CreateDecryptStatusWindow();

                statusWindow.Shown += StatusWindow_OnShow;
                statusWindow.FormClosed += StatusWindow_OnClose;
                statusWindow.CancellationSignalled += OnCancellationSignalled;

                statusWindow.Show();
                await handler.DecryptTheFilesAsync(FilePaths, OutputDirectory, bKey, TokenSource.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                TokenSource.Dispose();
                Array.Clear(bKey);
            }
        }
    }
}
