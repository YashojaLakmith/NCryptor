using System.Security.AccessControl;
using System.Security.Principal;

using Microsoft.Extensions.Hosting;

using NCryptor.Extensions;
using NCryptor.Forms;
using NCryptor.ServiceFactories;

namespace NCryptor;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        // A named mutex will be used to allow only a single instance of the application to run.

        using Mutex mutex = CreateAndConfigureMutex();
        if (!TryAcquireMutex(mutex))
        {
            return;
        }

        TryInitializeApplication(mutex);
    }

    private static Mutex CreateAndConfigureMutex()
    {
        string name = CreateMutexName();
        MutexSecurity security = CreateAndConfigureMutexSecurity();
        Mutex mutex = new(false, name, out _);
        mutex.SetAccessControl(security);

        return mutex;
    }

    private static string CreateMutexName()
    {
        string appGuid = AppConstants.AppGuid;

        return $@"Global\{appGuid}";
    }

    private static MutexSecurity CreateAndConfigureMutexSecurity()
    {
        MutexSecurity mutexSecurity = new();
        MutexAccessRule accessRule = CreateMutexAccessRule();

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
            if (mutex.WaitOne(1000, false))
            {
                return true;
            }

            MessageBox.Show(@"An instance of the application is already running.", @"NCryptor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        catch (AbandonedMutexException)
        {
            return true;
        }
    }

    private static void TryInitializeApplication(Mutex namedMutex)
    {
        try
        {
            BuildServiceFactory();
            ConfigureApplicationStartup();
            ExecuteMainWindow();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, @"An error occured.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            namedMutex.ReleaseMutex();
        }
    }

    private static void ConfigureApplicationStartup()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
    }

    private static void ExecuteMainWindow()
    {
        MainWindow mainWindow = ResolveMainWindow();
        Application.Run(mainWindow);
    }

    private static MainWindow ResolveMainWindow()
    {
        ServiceFactory serviceFactory = new();
        return serviceFactory.CreateMainWindow();
    }

    private static DIBasedServiceFactory BuildServiceFactory()
    {
        IHostBuilder hostBuilder = CreateHostBuilder();
        ConfigureServices(hostBuilder);
        IHost host = hostBuilder.Build();
        DIBasedServiceFactory serviceFactory = new(host.Services);
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
            services.AddLogEventService();
            services.AddProgressReporteventService();
            services.AddProcessingFileIndexEventService();
            services.AddTaskFinishedEventService();
            services.AddFileServices();
            services.AddFileStreamFactory();
            services.AddMetadataHandler();
            services.AddInputValidations();
            services.AddKeyDerivationOptions();
            services.AddFileSystemOptions();
            services.AddSymmetricCryptoService();
            services.AddKeyDerivationServices();
            services.AddTaskModeratorEventService();
            services.AddEncryptTaskModerator();
            services.AddDecryptTaskModerator();
            services.AddCryptographicOptions();
            services.AddSymmetricAlgorithm();

            services.AddMainWindow();
            services.AddEncryptDataCollectionWindow();
            services.AddDecryptDataCollectionWindow();
            services.AddEncryptStatusWindow();
            services.AddDecryptStatusWindow();
        });
    }
}
