using Microsoft.Extensions.DependencyInjection;

using NCryptor.TaskModerators;
using NCryptor.Forms;

namespace NCryptor.ServiceFactories
{
    /// <summary>
    /// Implemetaion of <see cref="IServiceFactory"/> backed by dependency injection container.
    /// </summary>
    public class DIBasedServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DIBasedServiceFactory(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public DecryptDataCollectionWindow CreateDecryptWindow()
            => _serviceProvider.GetRequiredService<DecryptDataCollectionWindow>();

        public EncryptDataCollectionWindow CreateEncryptWindow()
            => _serviceProvider.GetRequiredService<EncryptDataCollectionWindow>();

        public EncryptStatusWindow CreateEncryptStatusWindow()
            => _serviceProvider.GetRequiredService<EncryptStatusWindow>();

        public DecryptStatusWindow CreateDecryptStatusWindow()
            => _serviceProvider.GetRequiredService<DecryptStatusWindow>();

        public MainWindow CreateMainWindow()
            => _serviceProvider.GetRequiredService<MainWindow>();

        public IEncryptTaskModerator CreateEncryptTaskModerator(ManualModeratorParameters moderatorParameters)
        {
            var factory = _serviceProvider.GetRequiredService<Func<ManualModeratorParameters, IEncryptTaskModerator>>();
            return factory(moderatorParameters);
        }

        public IDecryptTaskModerator CreateDecryptTaskModerator(ManualModeratorParameters moderatorParameters)
        {
            var factory = _serviceProvider.GetRequiredService<Func<ManualModeratorParameters, IDecryptTaskModerator>>();
            return factory(moderatorParameters);
        }
    }
}
