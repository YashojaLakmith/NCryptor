using System;

namespace NCryptor.GUI.Events
{
    internal interface IProcessingFileCountReportable
    {
        event EventHandler<ProcessingFileCountEventArgs> ProcessingFileCountReported;
    }
}
