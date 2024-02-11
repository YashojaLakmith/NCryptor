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
        EncryptStatusWindow CreateEncryptStatusWindow();
        DecryptStatusWindow CreateDecryptStatusWindow();
        IEncryptTaskModerator CreateEncryptTaskModerator(ManualModeratorParameters moderatorParameters);
        IDecryptTaskModerator CreateDecryptTaskModerator(ManualModeratorParameters moderatorParameters);
    }
}
