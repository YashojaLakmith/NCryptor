﻿using NCryptor.Events.EventArguments;

namespace NCryptor.Events
{
    /// <summary>
    /// Provides capability to publish the index of the processing file out of all files.
    /// </summary>
    public interface IProcessingFileCountReportable
    {
        /// <summary>
        /// Raised when index of the currently processing file is published.
        /// </summary>
        event EventHandler<ProcessingFileCountEventArgs> ProcessingFileIndexReported;
    }
}
