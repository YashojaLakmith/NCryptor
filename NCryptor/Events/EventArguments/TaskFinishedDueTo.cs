namespace NCryptor.Events.EventArguments
{
    /// <summary>
    /// Represents reasons for the finish of a task.
    /// </summary>
    internal enum TaskFinishedDueTo
    {
        RanToSuccess,
        CancelledByUser,
        ErrorEncountered
    }
}
