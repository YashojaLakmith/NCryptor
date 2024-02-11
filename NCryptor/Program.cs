using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NCryptor.Crypto;
using NCryptor.Events;
using NCryptor.ServiceFactories;
using NCryptor.TaskModerators;
using NCryptor.Forms;
using NCryptor.Helpers;
using NCryptor.Metadata;
using NCryptor.Options;
using NCryptor.Streams;
using NCryptor.Validations;

namespace NCryptor
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            // A named mutex will be used to allow only a single instance of the application to run.

            using var mutex = CreateAndConfigureMutex();
            if (!TryAcquireMutex(mutex)) return;
            try
            {
                SetupAndExecuteMainWindow();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private static Mutex CreateAndConfigureMutex()
        {
            var name = CreateMutexName();
            var security = CreateAndConfigureMutexSecurity();
            var mutex = new Mutex(false, name, out _);
            mutex.SetAccessControl(security);

            return mutex;
        }

        private static string CreateMutexName()
        {
            var appGuid = AppConstants.AppGuid;

            return $@"Global\{appGuid}";
        }

        private static MutexSecurity CreateAndConfigureMutexSecurity()
        {
            var mutexSecurity = new MutexSecurity();
            var accessRule = CreateMutexAccessRule();

            mutexSecurity.AddAccessRule(accessRule);

            return mutexSecurity;
        }

        private static MutexAccessRule CreateMutexAccessRule()
        {
            return new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow);
        }

        private static bool TryAcquireMutex(Mutex mutex)
        {
            try
            {
                if(mutex.WaitOne(1000, false)) return true;

                MessageBox.Show(@"An instance of the application is already running.", @"NCryptor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (AbandonedMutexException)
            {
                return true;
            }
        }

        private static void SetupAndExecuteMainWindow()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainWindow = ResolveMainWindow();
            Application.Run(mainWindow);
        }

        private static MainWindow ResolveMainWindow()
        {
            var serviceFactory = BuildServiceFactory();
            return serviceFactory.CreateMainWindow();
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

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder();
        }

        private static void ConfigureServices(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<ILogEventService, LogEventImpl>();
                services.AddSingleton<IProgressReportEventService, ProgressReportEventImpl>();
                services.AddSingleton<IProcessingFileIndexEventService, ProcessingFileIndexEventImpl>();
                services.AddSingleton<ITaskFinishedEventService, TaskFinishedEventImpl>();
                services.AddSingleton<IFileServices, FileServicesImpl>();
                services.AddSingleton<IFileStreamFactory, FileStreamFactoryImpl>();
                services.AddSingleton<IMetadataHandler, MetadataHandlerImpl>();
                services.AddSingleton<IInputValidations, InputValidationsImpl>();
                services.AddSingleton<KeyDerivationOptions>();
                services.AddSingleton<FileSystemOptions>();

                services.AddTransient<ISymmetricCryptoService, SymmetricCryptoServiceImpl>();
                services.AddTransient<IKeyDerivationServices, KeyDerivationServiceImpl>();
                services.AddTransient<ITaskModeratorEventService, TaskModeratorEventServiceImpl>();
                services.AddTransient<MainWindow>();
                services.AddTransient<EncryptDataCollectionWindow>();
                services.AddTransient<DecryptDataCollectionWindow>();
                services.AddTransient<EncryptStatusWindow>();
                services.AddTransient<DecryptStatusWindow>();
                services.AddTransient<Func<ManualModeratorParameters, IEncryptTaskModerator>>(
                    container =>
                        parameters =>
                        {
                            var cryptoService = container.GetRequiredService<ISymmetricCryptoService>();
                            var metadataHandler = container.GetRequiredService<IMetadataHandler>();
                            var streamFactory = container.GetRequiredService<IFileStreamFactory>();
                            var fileServices = container.GetRequiredService<IFileServices>();
                            var keyServices = container.GetRequiredService<IKeyDerivationServices>();
                            var eventServices = container.GetRequiredService<ITaskModeratorEventService>();

                            return new EncryptTaskModeratorImpl(cryptoService, metadataHandler, streamFactory, fileServices, keyServices, eventServices, parameters);
                        });
                services.AddTransient<Func<ManualModeratorParameters, IDecryptTaskModerator>>(
                    container =>
                        parameters =>
                        {
                            var cryptoService = container.GetRequiredService<ISymmetricCryptoService>();
                            var metadataHandler = container.GetRequiredService<IMetadataHandler>();
                            var streamFactory = container.GetRequiredService<IFileStreamFactory>();
                            var fileServices = container.GetRequiredService<IFileServices>();
                            var keyServices = container.GetRequiredService<IKeyDerivationServices>();
                            var eventServices = container.GetRequiredService<ITaskModeratorEventService>();

                            return new DecryptTaskModeratorImpl(cryptoService, metadataHandler, streamFactory, fileServices, keyServices, eventServices, parameters);
                        });
                services.AddTransient<ICryptographicOptions>(
                    container => container.GetRequiredService<ISymmetricCryptoService>());
                services.AddTransient<SymmetricAlgorithm>(
                    container =>            // Inject the symmetric algorithm along with its options
                    {
                        var alg = Aes.Create();
                        alg.KeySize = 256;
                        alg.Mode = CipherMode.CBC;
                        alg.Padding = PaddingMode.PKCS7;

                        return alg;
                    });
            });
        }
    }
}
