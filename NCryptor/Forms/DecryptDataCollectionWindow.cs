using System.Text;

using NCryptor.ServiceFactories;
using NCryptor.Options;
using NCryptor.TaskModerators;
using NCryptor.Validations;

namespace NCryptor.Forms;

/// <summary>
/// Collects the necessary information with UI and validates the before proceeding to the decryption.
/// </summary>
public class DecryptDataCollectionWindow : BaseDataCollectionWindow
{
    private readonly FileSystemOptions _fileSystemOptions;

    public DecryptDataCollectionWindow(FileSystemOptions fileSystemOptions, IInputValidations inputValidations) : base(inputValidations)
    {
        _fileSystemOptions = fileSystemOptions;
        Text = @"Decrypt Files";
    }

    protected override void Btn_BrowseFiles_OnClick(object? sender, EventArgs e)
    {
        using (OpenFileDialog ofd = new())
        {
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Title = @"Select files to encrypt";
            ofd.Filter = $@"Encryptor files (*{_fileSystemOptions.FileExtension})|*{_fileSystemOptions.FileExtension}";
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
        byte[] bKey = Encoding.ASCII.GetBytes(textBox_Key.Text);
        TokenSource = new CancellationTokenSource();
        try
        {
            ManualModeratorParameters parameters = new()
            {
                FilePathCollection = FilePaths,
                OutputDirectory = OutputDirectory,
                UserKey = bKey,
                CancellationToken = TokenSource.Token
            };
            ServiceFactory factory = new();

            IDecryptTaskModerator moderator = factory.CreateDecryptTaskModerator(parameters);
            DecryptStatusWindow statusWindow = factory.CreateDecryptStatusWindow();

            statusWindow.Shown += StatusWindow_OnShow;
            statusWindow.FormClosed += StatusWindow_OnClose;
            statusWindow.CancellationSignalled += OnCancellationSignalled;

            statusWindow.Show();
            await moderator.ModerateFileDecryptionAsync();
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
