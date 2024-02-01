﻿using System;

namespace NCryptor.GUI.Events
{
    /// <summary>
    /// Provides capability to publish the index of the processing file out of all files.
    /// </summary>
    internal interface IProcessingFileCountReportable
    {
        /// <summary>
        /// Raised when index of the currently processing file is published.
        /// </summary>
        event EventHandler<ProcessingFileCountEventArgs> ProcessingFileCountReported;
    }
}
