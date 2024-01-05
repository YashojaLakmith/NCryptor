using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NCryptor.GUI
{
    public abstract partial class OpWindow : Form, IParentWindowAccess
    {
        private readonly IParentWindowAccess _mainWindowAccess;
        protected string _outputDir;
        protected List<string> _filePaths;

        internal OpWindow(IParentWindowAccess mainWindowAccess)
        {
            _mainWindowAccess = mainWindowAccess;
            _filePaths = new List<string>();
            _outputDir = "";

            InitializeComponent();
        }

        private void Form_OnShow_HideParent(object sender, EventArgs e)
        {
            _mainWindowAccess.HideParentWindow();
        }

        private void Form_OnClose_ShowParent(object sender, EventArgs e)
        {
            _mainWindowAccess.ShowParentWindow();

        }

        private void Btn_BrowseOut_OnClick(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the directory to output files.";

                if(fbd.ShowDialog() == DialogResult.OK)
                {
                    _outputDir = fbd.SelectedPath;
                    textbox_OutputDir.Text = _outputDir;
                }
            }
        }

        private void Textbox_Outputdir_OnTextChange(object sender, EventArgs e)
        {
            _outputDir = textbox_OutputDir.Text;
        }

        protected abstract void Btn_Start_OnClick(object sender, EventArgs e);

        protected abstract void Btn_BrowseFiles_OnClick(object sender, EventArgs e);

        private void Btn_Clear_OnClick(object sender, EventArgs e)
        {
            listBox_SelectedFiles.Items.Clear();
            _filePaths.Clear();
        }

        private void Btn_Remove_OnClick(object sender, EventArgs e)
        {
            var selected = listBox_SelectedFiles.SelectedItems;
            foreach (string s in selected.OfType<string>().ToList())
            {
                _filePaths.Remove(s);
                listBox_SelectedFiles.Items.Remove(s);
            }
        }

        protected void AddToListBox(string[] paths)
        {
            _filePaths = paths.Union(_filePaths).ToList();

            listBox_SelectedFiles.Items.Clear();
            listBox_SelectedFiles.Items.AddRange(_filePaths.ToArray());
        }

        private void TextBox_Key_OnTextChanged(object sender, EventArgs e)
        {
            const string pattern = @"^[a-zA-Z0-9!@#$%^&]{6,14}$";

            if(!Regex.IsMatch(textBox_Key.Text, pattern))
            {
                btn_Start.Enabled = false;
            }
            else
            {
                btn_Start.Enabled = true;
            }

        }

        public void HideParentWindow()
        {
            Hide();
        }

        public void ShowParentWindow()
        {
            textBox_Key.Text = string.Empty;
            Show();
        }
    }
}
