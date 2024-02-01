using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using NCryptor.GUI.Factories;

namespace NCryptor.GUI.Forms
{
    /// <summary>
    /// Collects the necessary information with UI and validates the before proceeding to the decryption.
    /// </summary>
    internal class DecryptWindow : OpWindow
    {
        public DecryptWindow() : base()
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

        protected override async Task OpenProgressWindow()
        {
            var tokenSource = new CancellationTokenSource();
            var bKey = Encoding.ASCII.GetBytes(textBox_Key.Text);
            try
            {
                var factory = new ServiceFactory();

                var handler = factory.CreateFileQueueHandler(_filePaths, _outputDir, bKey, tokenSource.Token);
                var progressWindow = factory.CreateStatusWindow(handler, tokenSource, "Decrypting");

                progressWindow.Shown += ProgressWindow_OnShow;
                progressWindow.FormClosed += ProgressWindow_OnClose;

                progressWindow.Show();
                await handler.DecryptTheFilesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                tokenSource.Dispose();
                Array.Clear(bKey, 0, bKey.Length);
            }
        }
    }
}
