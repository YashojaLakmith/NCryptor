using NCryptor.Events.EventArguments;

namespace NCryptor.Events
{
    public class ProgressReportEventImpl : IProgressReportEventService
    {
        public event EventHandler<ProgressPercentageReportedEventArgs>? ProgressPercentageReported;

        public void ProgressPublished(int progressPercentage)
        {
            var arg = new ProgressPercentageReportedEventArgs(progressPercentage);
            RaiseEvent(arg);
        }

        protected virtual void RaiseEvent(ProgressPercentageReportedEventArgs e)
        {
            ProgressPercentageReported?.Invoke(this, e);
        }
    }
}
