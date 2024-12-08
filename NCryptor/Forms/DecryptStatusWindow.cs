using NCryptor.Events;

namespace NCryptor.Forms;

public class DecryptStatusWindow : BaseStatusWindow
{
    public DecryptStatusWindow(IProgressReportEventService progressReportEventService, IProcessingFileIndexEventService processingFileIndexService, ILogEventService logEventService, ITaskFinishedEventService taskFinishedEventService) : base(progressReportEventService, processingFileIndexService, logEventService, taskFinishedEventService)
    {
        Text = @"Decrypting";
    }
}
