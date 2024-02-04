using NCryptor.Events;
using NCryptor.Events.EventArguments;

namespace NCryptor.Forms
{
    /// <summary>
    /// Represents the form for communicating the progress of the operation.
    /// </summary>
    internal partial class StatusWindow : Form
    {
        private readonly IFileQueueEvents _events;
        protected readonly CancellationTokenSource _cancellationTokenSource;
        private bool _isInProgress;

        public StatusWindow(IFileQueueEvents fileQueueEvents, CancellationTokenSource tokenSource, string title)
        {
            _events = fileQueueEvents;
            _cancellationTokenSource = tokenSource;
            _isInProgress = true;

            FormClosing += OnCloseButtonClick;
            _events.TaskFinished += OnTaskFinished;
            _events.LogEmitted += OnLogReported;
            _events.ProcessingFileIndexReported += OnFileIndexReported;
            _events.ProgressPercentageReported += OnProgressReported;
            

            InitializeComponent();

            label_Status.Text = string.Empty;
            listbox_Log.HorizontalScrollbar = true;
            Text = title;
        }

        private void OnProgressReported(object sender, ProgressPercentageReportedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void OnTaskFinished(object sender, TaskFinishedEventArgs e)
        {
            if (e.Reason == TaskFinishedDueTo.RanToSuccess) label_Status.Text = "Completed";
            if (e.Reason == TaskFinishedDueTo.CancelledByUser) label_Status.Text = "Cancelled";
            if (e.Reason == TaskFinishedDueTo.ErrorEncountered) label_Status.Text = "An error occured";

            _isInProgress = false;
            btn_Cancel.Enabled = false;
        }

        private void OnFileIndexReported(object sender, ProcessingFileCountEventArgs e)
        {
            label_Status.Text = $"File {e.CurrentFile} of {e.TotalFiles}";
        }

        private void OnLogReported(object sender, LogEmittedEventArgs e)
        {
            listbox_Log.Items.Add(e.Message);
        }

        private void OnCloseButtonClick(object sender, FormClosingEventArgs e)
        {
            if (!_isInProgress) { return; }

            if (e.CloseReason != CloseReason.UserClosing)
            {
                _cancellationTokenSource.Cancel();
                _isInProgress = false;
                return;
            }

            var result = MessageBox.Show("Cancel current operation?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                _cancellationTokenSource.Cancel();
                _isInProgress = false;
            }
            else
            {
                e.Cancel = true;
            }
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
    }
}
