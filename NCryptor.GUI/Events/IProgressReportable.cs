using System;

namespace NCryptor.GUI.Events
{
    /// <summary>
    /// Provides capability to publish the progress precentage of a task.
    /// </summary>
    internal interface IProgressReportable
    {
        /// <summary>
        /// Raised when progress percentage is published.
        /// </summary>
        event EventHandler<ProgressPercentageReportedEventArgs> ProgressPercentageReported;
    }
}
