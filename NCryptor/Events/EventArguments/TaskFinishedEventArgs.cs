namespace NCryptor.Events.EventArguments;

/// <summary>
/// Represents the event data for reason of finishing of a task.
/// </summary>
public class TaskFinishedEventArgs : EventArgs
{
    public TaskFinishedDueTo Reason { get; }

    public TaskFinishedEventArgs(TaskFinishedDueTo reason)
    {
        Reason = reason;
    }
}
