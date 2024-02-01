using System;

namespace NCryptor.GUI.Events
{
    internal class LogEmittedEventArgs : EventArgs
    {
        public string Message { get; }

        public LogEmittedEventArgs(string message)
        {
            Message = message;
        }
    }
}
