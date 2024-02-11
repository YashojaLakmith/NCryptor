﻿using NCryptor.Events.EventArguments;

namespace NCryptor.Events
{
    /// <summary>
    /// Provides capability to publish the finish of a task along with the reason.
    /// </summary>
    public interface ITaskFinishedEventService
    {
        /// <summary>
        /// Raised when a task is finished.
        /// </summary>
        event EventHandler<TaskFinishedEventArgs> TaskFinished;

        void PublishTaskFinished(TaskFinishedDueTo taskFinishedDueTo);
    }
}
