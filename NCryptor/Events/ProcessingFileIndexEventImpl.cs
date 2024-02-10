using NCryptor.Events.EventArguments;

namespace NCryptor.Events
{
    public class ProcessingFileIndexEventImpl : IProcessingFileIndexEventService
    {
        public event EventHandler<ProcessingFileCountEventArgs>? ProcessingFileIndexReported;

        public void PublishCurrentlyProcessingFileIndex(int zeroBasedIndex, int totalFiles)
        {
            var arg = new ProcessingFileCountEventArgs(totalFiles, zeroBasedIndex + 1);
            RaiseEvent(arg);
        }

        protected virtual void RaiseEvent(ProcessingFileCountEventArgs e)
        {
            ProcessingFileIndexReported?.Invoke(this, e);
        }
    }
}
