using Microsoft.Extensions.DependencyInjection;

namespace NetBrowser.Storage;

public static class DependencyInjection
{
    public static IServiceCollection AddStorageFeature(this ServiceCollection services)
        => services.AddSingleton<IDataService, DataService>();
}