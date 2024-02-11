using NCryptor.ServiceFactories;

namespace NCryptor.Forms
{
    /// <summary>
    /// Starter form.
    /// </summary>
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            Text = @"NCryptor";
        }

        private void Btn_Encrypt_OnClick(object? sender, EventArgs e)
        {
            var window = new ServiceFactory().CreateEncryptWindow();
            window.Shown += OpWindow_OnShow;
            window.FormClosed += OpWindow_OnClose;

            window.Show();
        }

        private void Btn_Decrypt_OnClick(object? sender, EventArgs e)
        {
            var window = new ServiceFactory().CreateDecryptWindow();
            window.Shown += OpWindow_OnShow;
            window.FormClosed += OpWindow_OnClose;

            window.Show();
        }

        private void OpWindow_OnShow(object? sender, EventArgs e)
            => Hide();

        private void OpWindow_OnClose(object? sender, EventArgs e)
            => Show();
    }
}
