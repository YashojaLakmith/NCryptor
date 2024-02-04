using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NCryptor.Crypto;
using NCryptor.Events;
using NCryptor.ServiceFactories;
using NCryptor.FileQueueHandlers;
using NCryptor.Forms;
using NCryptor.Helpers;
using NCryptor.Metadata;
using NCryptor.Options;
using NCryptor.Streams;

namespace NCryptor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // A named mutex will be used to allow only a single instance of the application to run.

            string appGuid = AppConstants.APP_GUID;
            string mutexId = $"Global\\{appGuid}";
            bool createdNew;
            var rule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                            MutexRights.FullControl,
                                            AccessControlType.Allow);
            var mutexSettings = new MutexSecurity();
            mutexSettings.AddAccessRule(rule);

            using var mutex = new Mutex(false, mutexId, out createdNew);
            mutex.SetAccessControl(mutexSettings);
            bool hasHandle = false;
            try
            {
                try
                {
                    hasHandle = mutex.WaitOne(1000, false);
                    if (!hasHandle)
                    {
                        MessageBox.Show("An instance of the application is already running.", "NCryptor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (AbandonedMutexException)
                {
                    hasHandle = true;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var serviceFactory = BuildServiceFactory();
                Application.Run(serviceFactory.CreateMainWindow());
            }
            finally
            {
                if (hasHandle)
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder();
        }

        private static void ConfigureServices(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IKeyDerivationServices, KeyDerivationServiceImpl>();
                services.AddSingleton<IFileServices, FileServicesImpl>();
                services.AddSingleton<IFileStreamFactory, FileStreamFactoryImpl>();
                services.AddSingleton<IMetadataHandler, MetadataHandlerImpl>();
                services.AddSingleton<CryptographicOptions>();
                services.AddSingleton<FileSystemOptions>();

                services.AddTransient<ISymmetricCryptoService, SymmetricCryptoService>();
                services.AddTransient<IFileQueueHandler, FileQueueHandlerImpl>();
                services.AddTransient<MainWindow>();
                services.AddTransient<EncryptWindow>();
                services.AddTransient<DecryptWindow>();
                services.AddTransient<Func<IFileQueueEvents, CancellationTokenSource, string, StatusWindow>>(
                    container =>
                        (events, tokenSource, title) => new StatusWindow(events, tokenSource, title));
                services.AddTransient<SymmetricAlgorithm>(
                    container =>
                    {
                        var alg = Aes.Create();
                        alg.KeySize = 256;
                        alg.Mode = CipherMode.CBC;
                        alg.Padding = PaddingMode.PKCS7;

                        return alg;
                    });
            });
        }

        private static DIBasedServiceFactory BuildServiceFactory()
        {
            var hostBuilder = CreateHostBuilder();
            ConfigureServices(hostBuilder);

            var host = hostBuilder.Build();
            var serviceProvider = host.Services;
            var serviceFactory = new DIBasedServiceFactory(serviceProvider);
            ServiceFactory.SetProvider(serviceFactory);

            return serviceFactory;
        }
    }
}
