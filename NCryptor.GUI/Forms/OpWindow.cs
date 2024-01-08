﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using NCryptor.GUI.Crypto;

namespace NCryptor.GUI.Forms
{
    public abstract partial class OpWindow : Form, IParentWindowAccess
    {
        private readonly IParentWindowAccess _mainWindowAccess;
        protected string _outputDir;
        protected List<string> _filePaths;

        // Should be disposed in the dispose method of the parent class.
        protected SymmetricAlgorithm _alg;

        internal OpWindow(IParentWindowAccess mainWindowAccess)
        {
            _mainWindowAccess = mainWindowAccess;
            _filePaths = new List<string>();
            _alg = AES_256_CBC_PKCS7.CreateObject();
            _outputDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            InitializeComponent();

            textbox_OutputDir.Text = _outputDir;
            textbox_OutputDir.TextChanged += Textbox_Outputdir_OnTextChange;
            btn_Start.Click += Btn_Start_OnClick;
        }

        private void Form_OnShow_HideParent(object sender, EventArgs e)
        {
            textBox_Key.Text = string.Empty;
            _mainWindowAccess.HideParentWindow();
            ValidateStartButton();
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

        private void Btn_Start_OnClick(object sender, EventArgs e)
        {
            if(_outputDir == string.Empty)
            {
                _outputDir = Assembly.GetExecutingAssembly().Location;
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

            OpenProgressWindow();
        }

        protected abstract void OpenProgressWindow();

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
