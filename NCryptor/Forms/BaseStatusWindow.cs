using NCryptor.Events;
using NCryptor.Events.EventArguments;

namespace NCryptor.Forms
{
    /// <summary>
    /// Represents the form for communicating the progress of the operation.
    /// </summary>
    public abstract partial class BaseStatusWindow : Form
    {
        private bool _isInProgress;
        private readonly IProgressReportEventService _progressEventService;
        private readonly IProcessingFileIndexEventService _processingFileEventService;
        private readonly ILogEventService _logEventService;
        private readonly ITaskFinishedEventService _taskFinishedEventService;

        public event EventHandler? CancellationSignalled;

        public BaseStatusWindow(IProgressReportEventService progressReportEventService, IProcessingFileIndexEventService processingFileIndexService, ILogEventService logEventService, ITaskFinishedEventService taskFinishedEventService)
        {
            _isInProgress = true;
            _progressEventService = progressReportEventService;
            _processingFileEventService = processingFileIndexService;
            _logEventService = logEventService;
            _taskFinishedEventService = taskFinishedEventService;

            FormClosing += OnCloseButtonClick;
            _taskFinishedEventService.TaskFinished += OnTaskFinished;
            _logEventService.LogEmitted += OnLogReported;
            _processingFileEventService.ProcessingFileIndexReported += OnFileIndexReported;
            _progressEventService.ProgressPercentageReported += OnProgressReported;

            InitializeComponent();

            label_Status.Text = string.Empty;
            listbox_Log.HorizontalScrollbar = true;
        }

        private void OnProgressReported(object? sender, ProgressPercentageReportedEventArgs e)
            => progressBar.Value = e.ProgressPercentage;

        private void OnTaskFinished(object? sender, TaskFinishedEventArgs e)
        {
            label_Status.Text = e.Reason switch
            {
                TaskFinishedDueTo.RanToSuccess => @"Completed",
                TaskFinishedDueTo.CancelledByUser => @"Cancelled",
                TaskFinishedDueTo.ErrorEncountered => @"An error occured",
                _ => label_Status.Text
            };

            _isInProgress = false;
            btn_Cancel.Enabled = false;
        }

        private void OnFileIndexReported(object? sender, ProcessingFileCountEventArgs e)
            => label_Status.Text = $@"File {e.CurrentFile} of {e.TotalFiles}";

        private void OnLogReported(object? sender, LogEmittedEventArgs e)
            => listbox_Log.Items.Add(e.Message);

        private void OnCloseButtonClick(object? sender, FormClosingEventArgs e)
        {
            if (!_isInProgress) { return; }

            if (e.CloseReason != CloseReason.UserClosing)
            {
                PublishCancellationSignalled();
                _isInProgress = false;
                return;
            }

            var result = MessageBox.Show(@"Cancel current operation?", @"Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                PublishCancellationSignalled();
                _isInProgress = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void Btn_Cancel_OnClick(object sender, EventArgs e)
        {
            var result = MessageBox.Show(@"Cancel current operation?", @"Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result != DialogResult.OK) return;

            PublishCancellationSignalled();
            btn_Cancel.Enabled = false;
            _isInProgress = false;
        }

        protected virtual void PublishCancellationSignalled()
        {
            CancellationSignalled?.Invoke(this, EventArgs.Empty);
        }
    }
}
