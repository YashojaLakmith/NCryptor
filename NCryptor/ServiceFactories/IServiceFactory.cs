using NCryptor.GUI.Events;
using NCryptor.GUI.FileQueueHandlers;
using NCryptor.GUI.Forms;

namespace NCryptor.GUI.Factories
{
    internal interface IServiceFactory
    {
        MainWindow CreateMainWindow();
        EncryptWindow CreateEncryptWindow();
        DecryptWindow CreateDecryptWindow();
        IFileQueueHandler CreateFileQueueHandler();
        StatusWindow CreateStatusWindow(IFileQueueEvents fileQueueEvents, CancellationTokenSource tokenSource, string title);
    }
}
