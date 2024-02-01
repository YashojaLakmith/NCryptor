using System;

namespace NCryptor.GUI.Events
{
    internal interface IProgressReportable
    {
        event EventHandler<ProgressPercentageReportedEventArgs> ProgressPercentageReported;
    }
}
