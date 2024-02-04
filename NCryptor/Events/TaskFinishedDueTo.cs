namespace NCryptor.GUI.Events
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
