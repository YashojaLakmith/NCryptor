using System.Text;

using NCryptor.ServiceFactories;

namespace NCryptor.Forms
{
    /// <summary>
    /// Collects the required information from the UI and validates them before proceeding to the encryption.
    /// </summary>
    public class EncryptDataCollectionWindow : BaseDataCollectionWindow
    {
        public EncryptDataCollectionWindow() : base()
        {
            Text = @"Encrypt Files";
        }

        protected override void Btn_BrowseFiles_OnClick(object? sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog()){
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Title = @"Select files to encrypt";
                ofd.Filter = @"All files (*.*)|*.*";
                ofd.Multiselect = true;

                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    AddToListBox(ofd.FileNames);
                }
            }
            ValidateStartButton();
        }

        protected override async Task BeginTaskAsync()
        {
            var byteKey = Encoding.ASCII.GetBytes(textBox_Key.Text);
            TokenSource = new CancellationTokenSource();
            try
            {
                var factory = new ServiceFactory();
                var handler = factory.CreateFileQueueHandler();
                var statusWindow = factory.CreateEncryptStatusWindow();

                statusWindow.Shown += StatusWindow_OnShow;
                statusWindow.FormClosed += StatusWindow_OnClose;
                statusWindow.CancellationSignalled += OnCancellationSignalled;

                statusWindow.Show();
                await handler.EncryptTheFilesAsync(FilePaths, OutputDirectory, byteKey, TokenSource.Token);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Array.Clear(byteKey);
                TokenSource.Dispose();
            }
        }
    }
}
