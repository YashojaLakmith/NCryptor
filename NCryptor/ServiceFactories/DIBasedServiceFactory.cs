using Microsoft.Extensions.DependencyInjection;

using NCryptor.TaskModerators;
using NCryptor.Forms;

namespace NCryptor.ServiceFactories
{
    public class DIBasedServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DIBasedServiceFactory(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public DecryptDataCollectionWindow CreateDecryptWindow()
            => _serviceProvider.GetRequiredService<DecryptDataCollectionWindow>();

        public EncryptDataCollectionWindow CreateEncryptWindow()
            => _serviceProvider.GetRequiredService<EncryptDataCollectionWindow>();

        public ITaskModerator CreateFileQueueHandler()
            => _serviceProvider.GetRequiredService<ITaskModerator>();

        public EncryptStatusWindow CreateEncryptStatusWindow()
            => _serviceProvider.GetRequiredService<EncryptStatusWindow>();

        public DecryptStatusWindow CreateDecryptStatusWindow()
            => _serviceProvider.GetRequiredService<DecryptStatusWindow>();

        public MainWindow CreateMainWindow()
            => _serviceProvider.GetRequiredService<MainWindow>();
    }
}
