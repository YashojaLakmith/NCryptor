using NCryptor.GUI.FileQueueHandlers;

namespace NCryptor.GUI.Events
{
    /// <summary>
    /// Contains the events that can be raised from <see cref="IFileQueueHandler"/>.
    /// </summary>
    internal interface IFileQueueEvents : IProgressReportable, IProcessingFileCountReportable, ILogEmittable, ITaskFinishedReasonReportable
    {
    }
}
