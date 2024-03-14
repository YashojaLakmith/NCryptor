using NCryptor.Events.EventArguments;

namespace NCryptor.Events
{
    /// <summary>
    /// Provides capability to publish the progress percentage of a task.
    /// </summary>
    public interface IProgressReportEventService
    {
        /// <summary>
        /// Raised when progress percentage is published.
        /// </summary>
        event EventHandler<ProgressPercentageReportedEventArgs> ProgressPercentageReported;

        /// <summary>
        /// Publishes an event with a percentage value which indicates the progress.
        /// </summary>
        void ProgressPublished(int progressPercentage);
    }
}
