using System;

namespace NCryptor.GUI.Events
{
    internal interface ILogEmittable
    {
        event EventHandler<LogEmittedEventArgs> LogEmitted;
    }
}
