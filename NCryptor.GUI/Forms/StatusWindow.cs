using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCryptor.GUI.Forms
{
    public abstract partial class StatusWindow : Form
    {
        private readonly IParentWindowAccess _parentWindow;
        protected readonly CancellationTokenSource _cancellationTokenSource;
        protected readonly SymmetricAlgorithm _algorithm;
        protected readonly byte[] _key;
        protected readonly List<string> _paths;
        protected readonly string _outputDir;
        protected bool _isInProgress;
        

        internal StatusWindow(IParentWindowAccess parentWindow, IEnumerable<string> paths, SymmetricAlgorithm algorithm, string outputDir, byte[] key)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _parentWindow = parentWindow;
            _algorithm = algorithm;
            _key = key;
            _paths = paths.ToList();
            _outputDir = outputDir;
            _isInProgress = true;

            Shown += Form_OnShow_HideParent;
            FormClosing += OnCloseButtonClick;
            FormClosed += Form_OnClose_ShowParent;

            InitializeComponent();

            label_Status.Text = string.Empty;
            listbox_Log.HorizontalScrollbar = true;
        }

        private async void OnCloseButtonClick(object sender, FormClosingEventArgs e)
        {
            if (!_isInProgress)
            {
                return;
            }

            if(e.CloseReason != CloseReason.UserClosing)
            {
                return;
            }

            var result = MessageBox.Show("Cancel current operation?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                _cancellationTokenSource.Cancel();
                await Task.Delay(500);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private async void Form_OnShow_HideParent(object sender, EventArgs e)
        {
            _parentWindow.HideParentWindow();
            await BeginTask();
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
                _cancellationTokenSource.Cancel();
                btn_Cancel.Enabled = false;
                _isInProgress = false;
            }
        }

        protected abstract Task BeginTask();

        protected byte[] DeriveKey(byte[] bytes, byte[] salt, int size)
        {
            using(var pbkdf2 = new Rfc2898DeriveBytes(bytes, salt, 100000, HashAlgorithmName.SHA512))
            {
                return pbkdf2.GetBytes(size);
            }
        }

        protected void LogToWindow(string msg)
        {
            listbox_Log.Items.Add(msg);
        }

        protected abstract string GetOutputFilePath(string inputFile);

        protected void UpdateProgress(long current, long total)
        {
            progressBar.Value = (int)(current * 100 / total);
        }

        protected void ClearUnfinishedFiles(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        protected bool CompareByteArrays(byte[] a, byte[] b)
        {
            return a.Length == b.Length && memcmp(a, b, a.Length) == 0;
        }

        protected void MemsetArray(byte[] a)
        {
            memset(a, 0, a.Length);
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        [DllImport("msvcrt.dll", CallingConvention =CallingConvention.Cdecl)]
        static extern IntPtr memset(byte[] b, int value, long count);
    }
}
