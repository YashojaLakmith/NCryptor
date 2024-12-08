using NCryptor.Events.EventArguments;

namespace NCryptor.Events;

public class ProcessingFileIndexEventImpl : IProcessingFileIndexEventService
{
    public event EventHandler<ProcessingFileCountEventArgs>? ProcessingFileIndexReported;

    public void PublishCurrentlyProcessingFileIndex(int zeroBasedIndex, int totalFiles)
    {
        ProcessingFileCountEventArgs arg = new(totalFiles, zeroBasedIndex + 1);
        RaiseEvent(arg);
    }

    protected virtual void RaiseEvent(ProcessingFileCountEventArgs e)
    {
        ProcessingFileIndexReported?.Invoke(this, e);
    }
}
