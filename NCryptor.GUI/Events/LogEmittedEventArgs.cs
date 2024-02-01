using System;

namespace NCryptor.GUI.Events
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
