namespace NCryptor.Events.EventArguments
{
    /// <summary>
    /// Represents reasons for the finish of a task.
    /// </summary>
    public enum TaskFinishedDueTo
    {
        RanToSuccess,
        CancelledByUser,
        ErrorEncountered
    }
}
