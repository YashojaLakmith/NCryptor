using System;
using System.Text;
using System.Windows.Forms;

namespace NCryptor.GUI.Forms
{
    /// <summary>
    /// Collects the required information from the UI and validates them before proceeding to the encryption.
    /// </summary>
    internal class EncryptWindow : OpWindow
    {
        internal EncryptWindow(IParentWindowAccess parentWindowAccess) : base(parentWindowAccess)
        {
            Text = "Encrypt Files";
        }

        protected override void Btn_BrowseFiles_OnClick(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog()){
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Title = "Select files to encrypt";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.Multiselect = true;

                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    AddToListBox(ofd.FileNames);
                }
            }
            ValidateStartButton();
        }

        protected override void OpenProgressWindow()
        {
            var bKey = Encoding.ASCII.GetBytes(textBox_Key.Text);
            new EncryptTaskWindow(this, _filePaths, _alg, _outputDir, bKey).Show();
        }
    }
}
