using System;
using Microsoft.Extensions.Configuration;
using Windows.ApplicationModel;

namespace NetBrowser_UWP.Services
{
    public class AppConfigService
    {
        private readonly IConfigurationRoot _configurationRoot;

        public AppConfigService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true);

            _configurationRoot = builder.Build();
        }

        public T GetSection<T>(string key) => _configurationRoot.GetSection(key).Get<T>();
    }
}
