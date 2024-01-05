using System;
using System.Windows.Forms;

namespace NCryptor.GUI
{
    internal class EncryptWindow : OpWindow
    {
        public EncryptWindow(IParentWindowAccess parentWindowAccess) : base(parentWindowAccess)
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
                else
                {
                    MessageBox.Show("An error occured.", "Failed to open browse", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        protected override void Btn_Start_OnClick(object sender, EventArgs e)
        {
            new StatusWindow(this).Show();
        }
    }
}
