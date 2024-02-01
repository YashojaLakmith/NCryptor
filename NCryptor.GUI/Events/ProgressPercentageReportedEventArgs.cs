using System;

namespace NCryptor.GUI.Events
{
    /// <summary>
    /// Represents event data for the progress percentage.
    /// </summary>
    internal class ProgressPercentageReportedEventArgs : EventArgs
    {
        public int ProgressPercentage { get; }

        public ProgressPercentageReportedEventArgs(int progressPercentage)
        {
            ProgressPercentage = progressPercentage;
        }
    }
}
