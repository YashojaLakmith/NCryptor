using NCryptor.TaskModerators;
using NCryptor.Forms;

namespace NCryptor.ServiceFactories
{
    public class ServiceFactory : IServiceFactory
    {
        private static IServiceFactory _provider;

        public static void SetProvider(IServiceFactory provider)
            => _provider = provider;

        public DecryptDataCollectionWindow CreateDecryptWindow()
            => _provider.CreateDecryptWindow();

        public EncryptDataCollectionWindow CreateEncryptWindow()
            => _provider.CreateEncryptWindow();

        public EncryptStatusWindow CreateEncryptStatusWindow()
            => _provider.CreateEncryptStatusWindow();

        public DecryptStatusWindow CreateDecryptStatusWindow()
            => _provider.CreateDecryptStatusWindow();

        public MainWindow CreateMainWindow()
            => _provider.CreateMainWindow();

        public IEncryptTaskModerator CreateEncryptTaskModerator(ManualModeratorParameters moderatorParameters)
            => _provider.CreateEncryptTaskModerator(moderatorParameters);

        public IDecryptTaskModerator CreateDecryptTaskModerator(ManualModeratorParameters moderatorParameters)
            => _provider.CreateDecryptTaskModerator(moderatorParameters);
    }
}
