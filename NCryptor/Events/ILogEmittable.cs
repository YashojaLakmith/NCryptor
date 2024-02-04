﻿namespace NCryptor.GUI.Events
{
    /// <summary>
    /// Provides log event raising.
    /// </summary>
    internal interface ILogEmittable
    {
        /// <summary>
        /// Raised when a new log message is published.
        /// </summary>
        event EventHandler<LogEmittedEventArgs> LogEmitted;
    }
}