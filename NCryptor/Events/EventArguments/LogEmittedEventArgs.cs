namespace NCryptor.Events.EventArguments;

/// <summary>
/// Represents event data for logs.
/// </summary>
public class LogEmittedEventArgs : EventArgs
{
    public string Message { get; }

    public LogEmittedEventArgs(string message)
    {
        Message = message;
    }
}
