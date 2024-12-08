namespace NCryptor.Events.EventArguments;

/// <summary>
/// Represents event data for the progress percentage.
/// </summary>
public class ProgressPercentageReportedEventArgs : EventArgs
{
    public int ProgressPercentage { get; }

    public ProgressPercentageReportedEventArgs(int progressPercentage)
    {
        ProgressPercentage = progressPercentage;
    }
}
