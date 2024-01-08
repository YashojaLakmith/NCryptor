using System;
using System.Windows.Forms;

namespace NCryptor.GUI.Forms
{
    /// <summary>
    /// Starter form.
    /// </summary>
    internal partial class MainWindow : Form, IParentWindowAccess
    {
        internal MainWindow()
        {
            InitializeComponent();
            Text = "NCryptor";
        }

        public void HideParentWindow()
        {
            Hide();
        }

        public void ShowParentWindow()
        {
            Show();
        }

        private void Btn_Encrypt_OnClick(object sender, EventArgs e)
        {
            new EncryptWindow(this).Show();
        }

        private void Btn_Decrypt_Click(object sender, EventArgs e)
        {
            new DecryptWindow(this).Show();
        }
    }
}
