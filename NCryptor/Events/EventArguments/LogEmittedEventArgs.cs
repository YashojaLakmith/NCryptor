namespace NCryptor.Events.EventArguments
{
    /// <summary>
    /// Represents event data for logs.
    /// </summary>
    internal class LogEmittedEventArgs : EventArgs
    {
        public string Message { get; }

        public LogEmittedEventArgs(string message)
        {
            Message = message;
        }
    }
}
