using NCryptor.Events;

namespace NCryptor.Forms
{
    public class EncryptStatusWindow : BaseStatusWindow
    {
        public EncryptStatusWindow(IProgressReportEventService progressReportEventService, IProcessingFileIndexEventService processingFileIndexService, ILogEventService logEventService, ITaskFinishedEventService taskFinishedEventService) : base(progressReportEventService, processingFileIndexService, logEventService, taskFinishedEventService)
        {
            Text = @"Encrypting";
        }
    }
}
