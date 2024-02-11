using System.Text;

using NCryptor.ServiceFactories;
using NCryptor.TaskModerators;
using NCryptor.Validations;

namespace NCryptor.Forms
{
    /// <summary>
    /// Collects the required information from the UI and validates them before proceeding to the encryption.
    /// </summary>
    public class EncryptDataCollectionWindow : BaseDataCollectionWindow
    {
        public EncryptDataCollectionWindow(IInputValidations inputValidations) : base(inputValidations)
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
                var parameters = new ManualModeratorParameters()
                {
                    FilePathCollection = FilePaths,
                    OutputDirectory = OutputDirectory,
                    UserKey = byteKey,
                    CancellationToken = TokenSource.Token
                };
                var factory = new ServiceFactory();
                var moderator = factory.CreateEncryptTaskModerator(parameters);
                var statusWindow = factory.CreateEncryptStatusWindow();

                statusWindow.Shown += StatusWindow_OnShow;
                statusWindow.FormClosed += StatusWindow_OnClose;
                statusWindow.CancellationSignalled += OnCancellationSignalled;

                statusWindow.Show();
                await moderator.ModerateFileEncryptionAsync();
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
