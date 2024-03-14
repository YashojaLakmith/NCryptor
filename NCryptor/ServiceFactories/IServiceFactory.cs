using NCryptor.Forms;
using NCryptor.TaskModerators;

namespace NCryptor.ServiceFactories
{
    /// <summary>
    /// Defines methods for creating various services that would not implicitely be resolved by the dependency injection container.
    /// </summary>
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
