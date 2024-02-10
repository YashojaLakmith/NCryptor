using NCryptor.Events;
using NCryptor.TaskModerators;
using NCryptor.Forms;

namespace NCryptor.ServiceFactories
{
    public interface IServiceFactory
    {
        MainWindow CreateMainWindow();
        EncryptDataCollectionWindow CreateEncryptWindow();
        DecryptDataCollectionWindow CreateDecryptWindow();
        ITaskModerator CreateFileQueueHandler();
        EncryptStatusWindow CreateEncryptStatusWindow();
        DecryptStatusWindow CreateDecryptStatusWindow();
    }
}
