using NCryptor.Events;
using NCryptor.FileQueueHandlers;
using NCryptor.Forms;

namespace NCryptor.ServiceFactories
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
