using System;

namespace NCryptor.GUI.Events
{
    internal class ProgressPercentageReportedEventArgs : EventArgs
    {
        public int ProgressPercentage { get; }

        public ProgressPercentageReportedEventArgs(int progressPercentage)
        {
            ProgressPercentage = progressPercentage;
        }
    }
}
