using System;
using System.Text;
using System.Windows.Forms;

namespace NCryptor.GUI
{
    internal class DecryptWindow : OpWindow
    {
        public DecryptWindow(IParentWindowAccess parentWindowAccess) : base(parentWindowAccess)
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
                else
                {
                    MessageBox.Show("An error occured.", "Failed to open browse", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void Btn_Start_OnClick(object sender, EventArgs e)
        {
            var bKey = Encoding.ASCII.GetBytes(textBox_Key.Text);
            new DecryptTaskWindow(this, _filePaths, _outputDir, bKey).Show();
        }
    }
}
