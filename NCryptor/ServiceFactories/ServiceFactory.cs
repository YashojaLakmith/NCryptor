using NCryptor.Events;
using NCryptor.FileQueueHandlers;
using NCryptor.Forms;

namespace NCryptor.ServiceFactories
{
    public class ServiceFactory : IServiceFactory
    {
        private static IServiceFactory _provider;

        public static void SetProvider(IServiceFactory provider)
            => _provider = provider;

        public DecryptWindow CreateDecryptWindow()
            => _provider.CreateDecryptWindow();

        public EncryptWindow CreateEncryptWindow()
            => _provider.CreateEncryptWindow();

        public IFileQueueHandler CreateFileQueueHandler()
            => _provider.CreateFileQueueHandler();

        public MainWindow CreateMainWindow()
            => _provider.CreateMainWindow();

        public StatusWindow CreateStatusWindow(IFileQueueEvents fileQueueEvents, CancellationTokenSource tokenSource, string title)
            => _provider.CreateStatusWindow(fileQueueEvents, tokenSource, title);
    }
}
