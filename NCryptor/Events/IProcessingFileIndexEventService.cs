using NCryptor.Events.EventArguments;

namespace NCryptor.Events
{
    /// <summary>
    /// Provides capability to publish the index of the processing file out of all files.
    /// </summary>
    public interface IProcessingFileIndexEventService
    {
        /// <summary>
        /// Raised when index of the currently processing file is published.
        /// </summary>
        event EventHandler<ProcessingFileCountEventArgs> ProcessingFileIndexReported;

        /// <summary>
        /// Publish an event with total files being processed an zero based index of the file being processed.
        /// </summary>
        void PublishCurrentlyProcessingFileIndex(int zeroBasedIndex, int totalFiles);
    }
}
