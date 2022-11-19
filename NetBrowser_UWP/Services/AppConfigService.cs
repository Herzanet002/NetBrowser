using Microsoft.Extensions.Configuration;
using System;

namespace NetBrowser_UWP.Services;

public class AppConfigService
{
    private readonly IConfigurationRoot _configurationRoot;

    public AppConfigService()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", true);

        _configurationRoot = builder.Build();
    }

    public T GetSection<T>(string key)
    {
        return _configurationRoot.GetSection(key).Get<T>();
    }
}