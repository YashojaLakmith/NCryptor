using System;
using System.Threading;
using System.Windows.Forms;

namespace NCryptor.GUI
{
    public partial class StatusWindow : Form
    {
        private readonly IParentWindowAccess _parentWindow;
        private readonly CancellationToken cancellationToken;

        internal StatusWindow(IParentWindowAccess parentWindow)
        {
            _parentWindow = parentWindow;
            InitializeComponent();
        }

        private void Form_OnShow_HideParent(object sender, EventArgs e)
        {
            _parentWindow.HideParentWindow();
        }

        private void Form_OnClose_ShowParent(object sender, EventArgs e)
        {
            _parentWindow.ShowParentWindow();

        }

        private void Btn_Cancel_OnClick(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Cancel current operation?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                // Cancel
            }
        }
    }
}
