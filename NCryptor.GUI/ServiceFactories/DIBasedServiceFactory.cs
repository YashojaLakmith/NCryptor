using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

using NCryptor.GUI.Events;
using NCryptor.GUI.FileQueueHandlers;
using NCryptor.GUI.Forms;

namespace NCryptor.GUI.Factories
{
    internal class DIBasedServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DIBasedServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public DecryptWindow CreateDecryptWindow()
        {
            return _serviceProvider.GetRequiredService<DecryptWindow>();
        }

        public EncryptWindow CreateEncryptWindow()
        {
            return _serviceProvider.GetRequiredService<EncryptWindow>();
        }

        public IFileQueueHandler CreateFileQueueHandler(IEnumerable<string> fileList, string outputDirectory, byte[] key, CancellationToken cancellationToken)
        {
            var factory = _serviceProvider.GetRequiredService<Func<IEnumerable<string>, string, byte[], CancellationToken, IFileQueueHandler>>();
            return factory(fileList, outputDirectory, key, cancellationToken);
        }

        public MainWindow CreateMainWindow()
        {
            return _serviceProvider.GetRequiredService<MainWindow>();
        }

        public StatusWindow CreateStatusWindow(IFileQueueEvents fileQueueEvents, CancellationTokenSource tokenSource, string title)
        {
            var factory = _serviceProvider.GetRequiredService<Func<IFileQueueEvents, CancellationTokenSource, string, StatusWindow>>();
            return factory(fileQueueEvents, tokenSource, title);
        }
    }
}
