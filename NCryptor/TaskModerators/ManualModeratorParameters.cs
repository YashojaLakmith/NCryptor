namespace NCryptor.TaskModerators;

public class ManualModeratorParameters
{
    public IEnumerable<string> FilePathCollection { get; set; }
    public string OutputDirectory { get; set; }
    public byte[] UserKey { get; set; }
    public CancellationToken CancellationToken { get; set; }
}
