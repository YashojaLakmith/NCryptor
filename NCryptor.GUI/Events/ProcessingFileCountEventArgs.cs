using System;

namespace NCryptor.GUI.Events
{
    internal class ProcessingFileCountEventArgs : EventArgs
    {
        public int TotalFiles { get; }
        public int CurrentFile {  get; }

        public ProcessingFileCountEventArgs(int total, int current)
        {
            TotalFiles = total;
            CurrentFile = current;
        }
    }
}
