using NCryptor.Events.EventArguments;

namespace NCryptor.Events
{
    /// <summary>
    /// Provides log event raising.
    /// </summary>
    public interface ILogEventService
    {
        /// <summary>
        /// Raised when a new log message is published.
        /// </summary>
        event EventHandler<LogEmittedEventArgs> LogEmitted;

        /// <summary>
        /// Publishes a log event with given message.
        /// </summary>
        void PublishALog(string message);
    }
}
