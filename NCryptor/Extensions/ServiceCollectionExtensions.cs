using System.Security.Cryptography;

using Microsoft.Extensions.DependencyInjection;

using NCryptor.Crypto;
using NCryptor.Events;
using NCryptor.Helpers;
using NCryptor.Metadata;
using NCryptor.Options;
using NCryptor.Streams;
using NCryptor.TaskModerators;
using NCryptor.Validations;

namespace NCryptor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLogEventService(this IServiceCollection services)
    {
        return services.AddSingleton<ILogEventService, LogEventImpl>();
    }

    public static IServiceCollection AddProgressReporteventService(this IServiceCollection services)
    {
        return services.AddSingleton<IProgressReportEventService, ProgressReportEventImpl>();
    }

    public static IServiceCollection AddProcessingFileIndexEventService(this IServiceCollection services)
    {
        return services.AddSingleton<IProcessingFileIndexEventService, ProcessingFileIndexEventImpl>();
    }

    public static IServiceCollection AddTaskFinishedEventService(this IServiceCollection services)
    {
        return services.AddSingleton<ITaskFinishedEventService, TaskFinishedEventImpl>();
    }

    public static IServiceCollection AddFileServices(this IServiceCollection services)
    {
        return services.AddSingleton<IFileServices, FileServicesImpl>();
    }

    public static IServiceCollection AddFileStreamFactory(this IServiceCollection services)
    {
        return services.AddSingleton<IFileStreamFactory, FileStreamFactoryImpl>();
    }

    public static IServiceCollection AddMetadataHandler(this IServiceCollection services)
    {
        return services.AddSingleton<IMetadataHandler, MetadataHandlerImpl>();
    }

    public static IServiceCollection AddInputValidations(this IServiceCollection services)
    {
        return services.AddSingleton<IInputValidations, InputValidationsImpl>();
    }

    public static IServiceCollection AddKeyDerivationOptions(this IServiceCollection services)
    {
        return services.AddSingleton<KeyDerivationOptions>();
    }

    public static IServiceCollection AddFileSystemOptions(this IServiceCollection services)
    {
        return services.AddSingleton<FileSystemOptions>();
    }

    public static IServiceCollection AddSymmetricCryptoService(this IServiceCollection services)
    {
        return services.AddTransient<ISymmetricCryptoService, SymmetricCryptoServiceImpl>();
    }

    public static IServiceCollection AddKeyDerivationServices(this IServiceCollection services)
    {
        return services.AddTransient<IKeyDerivationServices, KeyDerivationServiceImpl>();
    }

    public static IServiceCollection AddTaskModeratorEventService(this IServiceCollection services)
    {
        return services.AddTransient<ITaskModeratorEventService, TaskModeratorEventServiceImpl>();
    }

    public static IServiceCollection AddEncryptTaskModerator(this IServiceCollection services)
    {
        return services.AddTransient<Func<ManualModeratorParameters, IEncryptTaskModerator>>(
            container =>
                parameters =>
                {
                    ISymmetricCryptoService cryptoService = container.GetRequiredService<ISymmetricCryptoService>();
                    IMetadataHandler metadataHandler = container.GetRequiredService<IMetadataHandler>();
                    IFileStreamFactory streamFactory = container.GetRequiredService<IFileStreamFactory>();
                    IFileServices fileServices = container.GetRequiredService<IFileServices>();
                    IKeyDerivationServices keyServices = container.GetRequiredService<IKeyDerivationServices>();
                    ITaskModeratorEventService eventServices = container.GetRequiredService<ITaskModeratorEventService>();

                    return new EncryptTaskModeratorImpl(cryptoService, metadataHandler, streamFactory, fileServices, keyServices, eventServices, parameters);
                });
    }

    public static IServiceCollection AddDecryptTaskModerator(this IServiceCollection services)
    {
        return services.AddTransient<Func<ManualModeratorParameters, IDecryptTaskModerator>>(
            container =>
                parameters =>
                {
                    ISymmetricCryptoService cryptoService = container.GetRequiredService<ISymmetricCryptoService>();
                    IMetadataHandler metadataHandler = container.GetRequiredService<IMetadataHandler>();
                    IFileStreamFactory streamFactory = container.GetRequiredService<IFileStreamFactory>();
                    IFileServices fileServices = container.GetRequiredService<IFileServices>();
                    IKeyDerivationServices keyServices = container.GetRequiredService<IKeyDerivationServices>();
                    ITaskModeratorEventService eventServices = container.GetRequiredService<ITaskModeratorEventService>();

                    return new DecryptTaskModeratorImpl(cryptoService, metadataHandler, streamFactory, fileServices, keyServices, eventServices, parameters);
                });
    }

    public static IServiceCollection AddCryptographicOptions(this IServiceCollection services)
    {
        return services.AddTransient<ICryptographicOptions>(
            container => container.GetRequiredService<ISymmetricCryptoService>());
    }

    public static IServiceCollection AddSymmetricAlgorithm(this IServiceCollection services)
    {
        return services.AddTransient<SymmetricAlgorithm>(
            _ =>
            {
                Aes aes = Aes.Create();
                aes.KeySize = 256;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                return aes;
            });
    }
}
