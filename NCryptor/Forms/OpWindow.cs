using System.Reflection;
using System.Text.RegularExpressions;

namespace NCryptor.Forms
{
    /// <summary>
    /// Abstract base class for collecting information, validating and proceeding to the encryption and decryption process.
    /// </summary>
    public abstract partial class OpWindow : Form
    {
        protected string OutputDirectory;
        protected List<string> FilePaths;

        protected OpWindow()
        {
            FilePaths = [];
            OutputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            InitializeComponent();

            textbox_OutputDir.Text = OutputDirectory;
            textbox_OutputDir.TextChanged += Textbox_OutputDirectory_OnTextChange;
            btn_Start.Click += Btn_Start_OnClick;
        }

        private void Btn_BrowseOutputDirectory_OnClick(object? sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = @"Select the directory to output files.";
                fbd.ShowNewFolderButton = true;
                fbd.SelectedPath = OutputDirectory;

                if(fbd.ShowDialog() == DialogResult.OK)
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
            if(OutputDirectory == string.Empty)
            {
                OutputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                textbox_OutputDir.Text = OutputDirectory;
            }

            if (FilePaths.Count < 1)
            {
                MessageBox.Show(@"Select one or more files to continue", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (!IsValidKey())
            {
                MessageBox.Show("""
                                The key:
                                    -Must be equal of higher than 6 characters.
                                    -Must be equal or less than 14 character.
                                    -Can contain alphanumeric characters and !@#$%^&
                                """
                    , @"Invalid key"
                                , MessageBoxButtons.OK
                                , MessageBoxIcon.Error);
                return;
            }

            try
            {
                CreateDirectoryIfNotExists();
            }
            catch (Exception)
            {
                MessageBox.Show($@"Unable to create directory {OutputDirectory}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!EvaluateAccessRules())
            {
                MessageBox.Show($@"Do not have enough access permissions at {OutputDirectory}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await BeginTaskAsync();
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
            var selected = listBox_SelectedFiles.SelectedItems;
            foreach (var s in selected.OfType<string>().ToList())
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
            listBox_SelectedFiles.Items.AddRange(FilePaths.ToArray());
            ValidateStartButton();
        }

        private void TextBox_Key_OnTextChanged(object? sender, EventArgs e)
            => ValidateStartButton();

        private void CreateDirectoryIfNotExists()
        {
            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }
        }

        /// <returns><c>true</c> if has directory access rules for modifying. Otherwise <c>false</c></returns>
        private bool EvaluateAccessRules()
        {
            //bool writeAllow = false;
            //bool writeDeny = false;
            //var controlList = Directory.GetAccessControl(OutputDirectory);Directory.

            //if (controlList is null)
            //{
            //    return false;
            //}

            //var rules = controlList.GetAccessRules(true, true, typeof(SecurityIdentifier));
            //if (rules is null)
            //{
            //    return false;
            //}

            //foreach (FileSystemAccessRule rule in rules)
            //{
            //    if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
            //    {
            //        if (AccessControlType.Allow == rule.AccessControlType)
            //        {
            //            writeAllow = true;
            //        }
            //        else if (AccessControlType.Deny == rule.AccessControlType)
            //        {
            //            writeDeny = true;
            //        }
            //        continue;
            //    }
            //}

            //return writeAllow && !writeDeny;
            return true;
        }

        private bool IsValidKey()
        {
            const string pattern = @"^[a-zA-Z0-9!@#$%^&]{6,14}$";

            return Regex.IsMatch(textBox_Key.Text, pattern);
        }

        protected void ValidateStartButton()
        {
            var status = true;

            status &= (FilePaths.Count > 0);
            status &= IsValidKey();

            btn_Start.Enabled = status;
        }
    }
}
