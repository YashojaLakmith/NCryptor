using NCryptor.FileQueueHandlers;

namespace NCryptor.Events
{
    /// <summary>
    /// Contains the events that can be raised from <see cref="IFileQueueHandler"/>.
    /// </summary>
    public interface IFileQueueEvents : IProgressReportable, IProcessingFileCountReportable, ILogEmittable, ITaskFinishedReasonReportable
    {
    }
}
