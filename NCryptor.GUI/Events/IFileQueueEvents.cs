namespace NCryptor.GUI.Events
{
    internal interface IFileQueueEvents : IProgressReportable, IProcessingFileCountReportable, ILogEmittable, ITaskFinishedReasonReportable
    {
    }
}
