namespace NCryptor.Events.EventArguments;

/// <summary>
/// Represents event data for the index of current file index and total number of files.
/// </summary>
public class ProcessingFileCountEventArgs : EventArgs
{
    public int TotalFiles { get; }
    public int CurrentFile { get; }

    public ProcessingFileCountEventArgs(int total, int current)
    {
        TotalFiles = total;
        CurrentFile = current;
    }
}
