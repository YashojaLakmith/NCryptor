using System.Collections.Generic;
using System.Threading;

using NCryptor.GUI.Events;
using NCryptor.GUI.FileQueueHandlers;
using NCryptor.GUI.Forms;

namespace NCryptor.GUI.Factories
{
    internal class ServiceFactory : IServiceFactory
    {
        private static IServiceFactory _provider;

        public static void SetProvider(IServiceFactory provider)
        {
            _provider = provider;
        }        

        public DecryptWindow CreateDecryptWindow()
        {
            return _provider.CreateDecryptWindow();
        }

        public EncryptWindow CreateEncryptWindow()
        {
            return _provider.CreateEncryptWindow();
        }

        public IFileQueueHandler CreateFileQueueHandler()
        {
            return _provider.CreateFileQueueHandler();
        }

        public MainWindow CreateMainWindow()
        {
            return _provider.CreateMainWindow();
        }

        public StatusWindow CreateStatusWindow(IFileQueueEvents fileQueueEvents, CancellationTokenSource tokenSource, string title)
        {
            return _provider.CreateStatusWindow(fileQueueEvents, tokenSource, title);
        }
    }
}
