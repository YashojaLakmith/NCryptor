using System;

namespace NCryptor.GUI.Events
{
    internal class TaskFinishedEventArgs : EventArgs
    {
        public TaskFinishedDueTo Reason { get; }

        public TaskFinishedEventArgs(TaskFinishedDueTo reason)
        {
            Reason = reason;
        }
    }
}
