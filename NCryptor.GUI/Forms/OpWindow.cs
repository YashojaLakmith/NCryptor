using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using NCryptor.GUI.Crypto;
using NCryptor.GUI.Parameters;

namespace NCryptor.GUI.Forms
{
    /// <summary>
    /// Abstract base class for collecting information, validating and proceeding to the encryption and decryption process.
    /// </summary>
    internal abstract partial class OpWindow : Form
    {
        private readonly IParentWindowAccess _mainWindowAccess;
        protected string _outputDir;
        protected List<string> _filePaths;

        public OpWindow()
        {
            _filePaths = new List<string>();
            _outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            InitializeComponent();

            textbox_OutputDir.Text = _outputDir;
            textbox_OutputDir.TextChanged += Textbox_Outputdir_OnTextChange;
            btn_Start.Click += Btn_Start_OnClick;
        }

        private void Btn_BrowseOut_OnClick(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the directory to output files.";
                fbd.ShowNewFolderButton = true;
                fbd.SelectedPath = _outputDir;

                if(fbd.ShowDialog() == DialogResult.OK)
                {
                    _outputDir = fbd.SelectedPath;
                    textbox_OutputDir.Text = _outputDir;
                }
            }
            ValidateStartButton();
        }

        private void Textbox_Outputdir_OnTextChange(object sender, EventArgs e)
        {
            _outputDir = textbox_OutputDir.Text;
        }

        private async void Btn_Start_OnClick(object sender, EventArgs e)
        {
            if(_outputDir == string.Empty)
            {
                var location = Assembly.GetExecutingAssembly().Location;
                _outputDir = Path.GetDirectoryName(location);
                textbox_OutputDir.Text = _outputDir;
            }

            if (_filePaths.Count < 1)
            {
                MessageBox.Show("Select one or more files to continue", "Eror", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (!IsValidKey())
            {
                MessageBox.Show("The key:\n-Must be equal of higher than 6 characters.\n-Must be equal or less than 14 character\n-Can contain alphanumeric charcters and !@#$%^&"
                                , "Invalid key"
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
                MessageBox.Show($"Unable to create directory {_outputDir}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!EvaluateAccessRules())
            {
                MessageBox.Show($"Do not have enough access permissions at {_outputDir}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await OpenProgressWindow();
        }

        protected abstract Task OpenProgressWindow();

        protected abstract void Btn_BrowseFiles_OnClick(object sender, EventArgs e);

        private void Btn_Clear_OnClick(object sender, EventArgs e)
        {
            listBox_SelectedFiles.Items.Clear();
            _filePaths.Clear();
            ValidateStartButton();
        }

        private void Btn_Remove_OnClick(object sender, EventArgs e)
        {
            var selected = listBox_SelectedFiles.SelectedItems;
            foreach (string s in selected.OfType<string>().ToList())
            {
                _filePaths.Remove(s);
                listBox_SelectedFiles.Items.Remove(s);
            }
            ValidateStartButton();
        }

        protected void ProgressWindow_OnShow(object sender, EventArgs e)
        {
            Hide();
        }

        protected void ProgressWindow_OnClose(object sender, EventArgs e)
        {
            Show();
        }

        protected void AddToListBox(string[] paths)
        {
            _filePaths = paths.Union(_filePaths).ToList();

            listBox_SelectedFiles.Items.Clear();
            listBox_SelectedFiles.Items.AddRange(_filePaths.ToArray());
            ValidateStartButton();
        }

        private void TextBox_Key_OnTextChanged(object sender, EventArgs e)
        {
            ValidateStartButton();
        }

        public void HideParentWindow()
        {
            Hide();
        }

        public void ShowParentWindow()
        {
            textBox_Key.Text = string.Empty;
            Show();
            ValidateStartButton();
        }

        private void CreateDirectoryIfNotExists()
        {
            if (!Directory.Exists(_outputDir))
            {
                Directory.CreateDirectory(_outputDir);
            }
        }

        /// <returns><c>true</c> if has directory access rules for modifying. Otherwise <c>false</c></returns>
        private bool EvaluateAccessRules()
        {
            bool writeAllow = false;
            bool writeDeny = false;
            var controlList = Directory.GetAccessControl(_outputDir);

            if (controlList is null)
            {
                return false;
            }

            var rules = controlList.GetAccessRules(true, true, typeof(SecurityIdentifier));
            if (rules is null)
            {
                return false;
            }

            foreach (FileSystemAccessRule rule in rules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                {
                    if (AccessControlType.Allow == rule.AccessControlType)
                    {
                        writeAllow = true;
                    }
                    else if (AccessControlType.Deny == rule.AccessControlType)
                    {
                        writeDeny = true;
                    }
                    continue;
                }
            }

            return writeAllow && !writeDeny;
        }

        private bool IsValidKey()
        {
            const string PATTERN = @"^[a-zA-Z0-9!@#$%^&]{6,14}$";

            return Regex.IsMatch(textBox_Key.Text, PATTERN);
        }

        protected void ValidateStartButton()
        {
            bool status = true;

            status &= (_filePaths.Count > 0);
            status &= IsValidKey();

            btn_Start.Enabled = status;
        }
    }
}
