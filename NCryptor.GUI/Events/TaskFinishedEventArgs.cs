using System;

namespace NCryptor.GUI.Events
{
    /// <summary>
    /// Represents the event data for reason of finishing of a task.
    /// </summary>
    internal class TaskFinishedEventArgs : EventArgs
    {
        public TaskFinishedDueTo Reason { get; }

        public TaskFinishedEventArgs(TaskFinishedDueTo reason)
        {
            Reason = reason;
        }
    }
}
