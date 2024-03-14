using System.Diagnostics;
using NCryptor.Events.EventArguments;

namespace NCryptor.Events
{
    public class TaskModeratorEventServiceImpl : ITaskModeratorEventService
    {
        private readonly IProgressReportEventService _progressReport;
        private readonly IProcessingFileIndexEventService _fileIndexEvent;
        private readonly ILogEventService _logEvent;
        private readonly ITaskFinishedEventService _taskFinishedEvent;
        private readonly Stopwatch _stopWatch;

        public TaskModeratorEventServiceImpl(IProgressReportEventService progressReport, IProcessingFileIndexEventService fileIndexEvent, ILogEventService logEvent, ITaskFinishedEventService taskFinishedEvent)
        {
            _progressReport = progressReport;
            _fileIndexEvent = fileIndexEvent;
            _logEvent = logEvent;
            _taskFinishedEvent = taskFinishedEvent;

            _stopWatch?.Reset();
            _stopWatch = Stopwatch.StartNew();
        }

        public void FileNotFoundEvent(string path)
        {
            var timeString = BuildTimeString();
            var message = $"{timeString}: {path} Not found";

            _logEvent.PublishALog(message);
        }

        public void ErrorEvent(string errorMessage)
        {
            var timeString = BuildTimeString();
            var message = $"{timeString}: {errorMessage}";

            _logEvent.PublishALog(message);
        }

        public void SuccessfulSingleFileCompletionEvent()
        {
            var timeString = BuildTimeString();
            var message = $"{timeString}: @Completed processing the file.";

            _logEvent.PublishALog(message);
            _progressReport.ProgressPublished(100);
        }

        public void BeginOfFileEncryptionEvent(string filePath, int zeroBasedIndex, int totalFiles)
        {
            var timeString = BuildTimeString();
            var message = $"{timeString}: Encrypting {filePath}";

            _logEvent.PublishALog(message);
            _progressReport.ProgressPublished(0);
            _fileIndexEvent.PublishCurrentlyProcessingFileIndex(zeroBasedIndex, totalFiles);
        }

        public void BeginOfFileDecryptionEvent(string filePath, int zeroBasedIndex, int totalFiles)
        {
            var timeString = BuildTimeString();
            var message = $"{timeString}: Decrypting {filePath}";

            _logEvent.PublishALog(message);
            _progressReport.ProgressPublished(0);
            _fileIndexEvent.PublishCurrentlyProcessingFileIndex(zeroBasedIndex, totalFiles);
        }

        public void CancellationEvent()
        {
            var timeString = BuildTimeString();
            var message = $"\n{timeString}: Cancelled by the user.";
            
            _logEvent.PublishALog(message);
            _progressReport.ProgressPublished(0);
            _taskFinishedEvent.PublishTaskFinished(TaskFinishedDueTo.CancelledByUser);
        }

        public void CompletionDueToSuccessEvent()
        {
            var timeString = BuildTimeString();
            var message = $"\n{timeString}: Completed.";

            _logEvent.PublishALog(message);
            _progressReport.ProgressPublished(100);
            _taskFinishedEvent.PublishTaskFinished(TaskFinishedDueTo.RanToSuccess);
        }

        public void CompletionDueToErrorEvent(string message)
        {
            var timeString = BuildTimeString();
            message = $"\n{timeString}: {message}.";

            _logEvent.PublishALog(message);
            _progressReport.ProgressPublished(0);
            _taskFinishedEvent.PublishTaskFinished(TaskFinishedDueTo.ErrorEncountered);
        }

        private string BuildTimeString()
        {
            return $"{_stopWatch.Elapsed:hh\\:mm\\:ss}";
        }
    }
}
