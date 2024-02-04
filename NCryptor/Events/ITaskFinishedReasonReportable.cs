using NCryptor.Events.EventArguments;

namespace NCryptor.Events
{
    /// <summary>
    /// Provides capability to publish the finish of a task along with the reason.
    /// </summary>
    internal interface ITaskFinishedReasonReportable
    {
        /// <summary>
        /// Raised when a task is finished.
        /// </summary>
        event EventHandler<TaskFinishedEventArgs> TaskFinished;
    }
}
