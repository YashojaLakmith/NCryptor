using System;
using System.Text;
using System.Windows.Forms;

namespace NCryptor.GUI.Forms
{
    /// <summary>
    /// Collects the necessary information with UI and validates the before proceeding to the decryption.
    /// </summary>
    internal class DecryptWindow : OpWindow
    {
        internal DecryptWindow(IParentWindowAccess parentWindowAccess) : base(parentWindowAccess)
        {
            Text = "Decrypt Files";
        }

        protected override void Btn_BrowseFiles_OnClick(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Title = "Select files to encrypt";
                ofd.Filter = "Encryptor files (*.NCRYPT)|*.NCRYPT";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    AddToListBox(ofd.FileNames);
                }
            }
            ValidateStartButton();
        }

        protected override void OpenProgressWindow()
        {
            var bKey = Encoding.ASCII.GetBytes(textBox_Key.Text);
            new DecryptTaskWindow(this, _filePaths, _alg, _outputDir, bKey).Show();
        }
    }
}
