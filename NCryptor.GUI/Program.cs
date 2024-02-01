using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NCryptor.GUI.Crypto;
using NCryptor.GUI.Events;
using NCryptor.GUI.Factories;
using NCryptor.GUI.FileQueueHandlers;
using NCryptor.GUI.Forms;
using NCryptor.GUI.Metadata;
using NCryptor.GUI.Options;
using NCryptor.GUI.Parameters;
using NCryptor.GUI.Streams;

namespace NCryptor.GUI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // A named mutex will be used to allow only a single instance of the application to run.

            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly()
                                                        .GetCustomAttributes(typeof(GuidAttribute), false)
                                                        .GetValue(0)).Value.ToString();
            string mutexId = $"Global\\{appGuid}";
            bool createdNew;
            var rule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                            MutexRights.FullControl,
                                            AccessControlType.Allow);
            var mutexSettings = new MutexSecurity();
            mutexSettings.AddAccessRule(rule);

            using(var mutex = new Mutex(false, mutexId, out createdNew, mutexSettings))
            {
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

                    var serviceFactory = CompositionRoot();
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
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<NCryptorOptions>();
                    services.AddTransient<IFileStreamFactory, FileStreamFactoryImpl>();
                    services.AddTransient<IMetadataHandler, MetadataHandlerImpl>();
                    services.AddTransient<ISymmetricCryptoService, SymmetricCryptoService>();
                    services.AddSingleton<IFileQueueHandler, FileQueueHandler>();
                    services.AddTransient<MainWindow>();
                    services.AddTransient<EncryptWindow>();
                    services.AddTransient<DecryptWindow>();
                    services.AddTransient<StatusWindow>();
                    services.AddTransient<SymmetricAlgorithm>(container =>
                    {
                        var alg = Aes.Create();
                        alg.KeySize = 256;
                        alg.Padding = PaddingMode.PKCS7;
                        alg.Mode = CipherMode.CBC;
                        return alg;
                    });
                    services.AddTransient<Func<IFileQueueEvents, CancellationTokenSource, string, StatusWindow>>(
                        container =>
                            (events, tokeSource, title) =>
                            {
                                return new StatusWindow(events, tokeSource, title);
                            });
                    services.AddTransient<Func<IEnumerable<string>, string, byte[], CancellationToken, IFileQueueHandler>>(
                        container =>
                            (filePaths, outputDirectory, key, cancellationToken) =>
                            {
                                var cryptoService = container.GetRequiredService<ISymmetricCryptoService>();
                                var metadataHandler = container.GetRequiredService<IMetadataHandler>();
                                var streamFactory = container.GetRequiredService<IFileStreamFactory>();
                                var options = container.GetRequiredService<NCryptorOptions>();

                                return new FileQueueHandler(cryptoService, metadataHandler, streamFactory, options, filePaths, outputDirectory, key, cancellationToken);
                            });
                });
        }

        private static IServiceFactory CompositionRoot()
        {
            var host = CreateHostBuilder().Build();
            var serviceProvider = host.Services;
            var serviceFactory = new DIBasedServiceFactory(serviceProvider);
            ServiceFactory.SetProvider(serviceFactory);

            return serviceFactory;
        }
    }
}
