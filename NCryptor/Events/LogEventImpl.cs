using NCryptor.Events.EventArguments;

namespace NCryptor.Events;

public class LogEventImpl : ILogEventService
{
    public event EventHandler<LogEmittedEventArgs>? LogEmitted;

    public void PublishALog(string message)
    {
        LogEmittedEventArgs arg = new(message);
        RaiseEvent(arg);
    }

    protected virtual void RaiseEvent(LogEmittedEventArgs e)
    {
        LogEmitted?.Invoke(this, e);
    }
}
