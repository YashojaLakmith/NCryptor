namespace NCryptor.Events
{
    public interface ITaskModeratorEventService
    {
        void FileNotFoundEvent(string path);
        void ErrorEvent(string errorMessage);
        void SuccessfulCompletionEvent();
        void BeginOfFileEncryptionEvent(string filePath, int zeroBasedIndex, int totalFiles);
        void BeginOfFileDecryptionEvent(string filePath, int zeroBasedIndex, int totalFiles);
        void CancellationEvent();
        void CompletionDueToSuccessEvent();
        void CompletionDueToErrorEvent(string message);
    }
}
