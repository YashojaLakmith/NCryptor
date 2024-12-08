using NCryptor.Validations;

namespace NCryptor.Forms;

/// <summary>
/// Abstract base class for collecting information, validating and proceeding to the encryption and decryption process.
/// </summary>
public abstract partial class BaseDataCollectionWindow : Form
{
    protected string OutputDirectory;
    private readonly IInputValidations _inputValidations;
    protected List<string> FilePaths;
    protected CancellationTokenSource TokenSource;

    protected BaseDataCollectionWindow(IInputValidations inputValidations)
    {
        FilePaths = [];
        OutputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _inputValidations = inputValidations;

        InitializeComponent();

        textbox_OutputDir.Text = OutputDirectory;
        textbox_OutputDir.TextChanged += Textbox_OutputDirectory_OnTextChange;
        btn_Start.Click += Btn_Start_OnClick;
    }

    private void Btn_BrowseOutputDirectory_OnClick(object? sender, EventArgs e)
    {
        using (FolderBrowserDialog fbd = new())
        {
            fbd.Description = @"Select the directory to output files.";
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = OutputDirectory;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                OutputDirectory = fbd.SelectedPath;
                textbox_OutputDir.Text = OutputDirectory;
            }
        }
        ValidateStartButton();
    }

    private void Textbox_OutputDirectory_OnTextChange(object? sender, EventArgs e)
    {
        OutputDirectory = textbox_OutputDir.Text;
    }

    private async void Btn_Start_OnClick(object? sender, EventArgs e)
    {
        await BeginTaskAsync();

        textBox_Key.Text = string.Empty;
    }

    protected abstract Task BeginTaskAsync();

    protected abstract void Btn_BrowseFiles_OnClick(object? sender, EventArgs e);

    private void Btn_Clear_OnClick(object? sender, EventArgs e)
    {
        listBox_SelectedFiles.Items.Clear();
        FilePaths.Clear();
        ValidateStartButton();
    }

    private void Btn_Remove_OnClick(object? sender, EventArgs e)
    {
        ListBox.SelectedObjectCollection selected = listBox_SelectedFiles.SelectedItems;
        foreach (string? s in selected.OfType<string>().ToList())
        {
            FilePaths.Remove(s);
            listBox_SelectedFiles.Items.Remove(s);
        }
        ValidateStartButton();
    }

    protected void StatusWindow_OnShow(object? sender, EventArgs e)
        => Hide();

    protected void StatusWindow_OnClose(object? sender, EventArgs e)
        => Show();

    protected void AddToListBox(IEnumerable<string> paths)
    {
        FilePaths = paths.Union(FilePaths).ToList();

        listBox_SelectedFiles.Items.Clear();
        listBox_SelectedFiles.Items.AddRange([.. FilePaths]);
        ValidateStartButton();
    }

    private void TextBox_Key_OnTextChanged(object? sender, EventArgs e)
        => ValidateStartButton();

    protected void ValidateStartButton()
    {
        bool status = true;

        status &= (FilePaths.Count > 0);
        status &= _inputValidations.IsValidPassword(textBox_Key.Text);

        btn_Start.Enabled = status;
    }

    protected async void OnCancellationSignalled(object? sender, EventArgs e)
    {
        await TokenSource.CancelAsync();
    }
}
