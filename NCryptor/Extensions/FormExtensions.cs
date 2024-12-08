using Microsoft.Extensions.DependencyInjection;

using NCryptor.Forms;

namespace NCryptor.Extensions;

public static class FormExtensions
{
    public static IServiceCollection AddMainWindow(this IServiceCollection services)
    {
        return services.AddTransient<MainWindow>();
    }

    public static IServiceCollection AddEncryptDataCollectionWindow(this IServiceCollection services)
    {
        return services.AddTransient<EncryptDataCollectionWindow>();
    }

    public static IServiceCollection AddDecryptDataCollectionWindow(this IServiceCollection services)
    {
        return services.AddTransient<DecryptDataCollectionWindow>();
    }

    public static IServiceCollection AddEncryptStatusWindow(this IServiceCollection services)
    {
        return services.AddTransient<EncryptStatusWindow>();
    }

    public static IServiceCollection AddDecryptStatusWindow(this IServiceCollection services)
    {
        return services.AddTransient<DecryptStatusWindow>();
    }
}
