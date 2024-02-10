using NCryptor.Events.EventArguments;

namespace NCryptor.Events
{
    public class TaskFinishedEventImpl : ITaskFinishedEventService
    {
        public event EventHandler<TaskFinishedEventArgs>? TaskFinished;

        public void PublishTaskFinished(TaskFinishedDueTo taskFinishedDueTo)
        {
            var arg = new TaskFinishedEventArgs(taskFinishedDueTo);
            RaiseEvent(arg);
        }

        protected virtual void RaiseEvent(TaskFinishedEventArgs e)
        {
            TaskFinished?.Invoke(this, e);
        }
    }
}
