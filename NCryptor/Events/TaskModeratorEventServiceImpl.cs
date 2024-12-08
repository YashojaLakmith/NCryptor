using System.Diagnostics;

using NCryptor.Events.EventArguments;

namespace NCryptor.Events;

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
        string timeString = BuildTimeString();
        string message = $"{timeString}: {path} Not found";

        _logEvent.PublishALog(message);
    }

    public void ErrorEvent(string errorMessage)
    {
        string timeString = BuildTimeString();
        string message = $"{timeString}: {errorMessage}";

        _logEvent.PublishALog(message);
    }

    public void SuccessfulSingleFileCompletionEvent()
    {
        string timeString = BuildTimeString();
        string message = $"{timeString}: @Completed processing the file.";

        _logEvent.PublishALog(message);
        _progressReport.ProgressPublished(100);
    }

    public void BeginOfFileEncryptionEvent(string filePath, int zeroBasedIndex, int totalFiles)
    {
        string timeString = BuildTimeString();
        string message = $"{timeString}: Encrypting {filePath}";

        _logEvent.PublishALog(message);
        _progressReport.ProgressPublished(0);
        _fileIndexEvent.PublishCurrentlyProcessingFileIndex(zeroBasedIndex, totalFiles);
    }

    public void BeginOfFileDecryptionEvent(string filePath, int zeroBasedIndex, int totalFiles)
    {
        string timeString = BuildTimeString();
        string message = $"{timeString}: Decrypting {filePath}";

        _logEvent.PublishALog(message);
        _progressReport.ProgressPublished(0);
        _fileIndexEvent.PublishCurrentlyProcessingFileIndex(zeroBasedIndex, totalFiles);
    }

    public void CancellationEvent()
    {
        string timeString = BuildTimeString();
        string message = $"\n{timeString}: Cancelled by the user.";

        _logEvent.PublishALog(message);
        _progressReport.ProgressPublished(0);
        _taskFinishedEvent.PublishTaskFinished(TaskFinishedDueTo.CancelledByUser);
    }

    public void CompletionDueToSuccessEvent()
    {
        string timeString = BuildTimeString();
        string message = $"\n{timeString}: Completed.";

        _logEvent.PublishALog(message);
        _progressReport.ProgressPublished(100);
        _taskFinishedEvent.PublishTaskFinished(TaskFinishedDueTo.RanToSuccess);
    }

    public void CompletionDueToErrorEvent(string message)
    {
        string timeString = BuildTimeString();
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
