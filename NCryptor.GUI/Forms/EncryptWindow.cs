using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using NCryptor.GUI.Factories;

namespace NCryptor.GUI.Forms
{
    /// <summary>
    /// Collects the required information from the UI and validates them before proceeding to the encryption.
    /// </summary>
    internal class EncryptWindow : OpWindow
    {
        public EncryptWindow() : base()
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

        protected override async Task OpenProgressWindow()
        {
            var tokenSource = new CancellationTokenSource();
            var byteKey = Encoding.ASCII.GetBytes(textBox_Key.Text);
            try
            {
                var factory = new ServiceFactory();
                var handler = factory.CreateFileQueueHandler(_filePaths, _outputDir, byteKey, tokenSource.Token);
                var progressWindow = factory.CreateStatusWindow(handler, tokenSource, "Encrypting");

                progressWindow.Shown += ProgressWindow_OnShow;
                progressWindow.FormClosed += ProgressWindow_OnClose;

                progressWindow.Show();
                await handler.EncryptTheFilesAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Array.Clear(byteKey, 0, byteKey.Length);
                tokenSource.Dispose();
            }
        }
    }
}
