using System;

namespace NCryptor.GUI.Events
{
    internal interface ITaskFinishedReasonReportable
    {
        event EventHandler<TaskFinishedEventArgs> TaskFinished;
    }
}
