using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Helpers;
using System.Threading.Tasks;
using Windows.Storage;

namespace NetBrowser_UWP.Services
{
    public class LocalSettingsService : ILocalSettingsService
    {
        public async Task<T> ReadSettingAsync<T>(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }

            return default;
        }

        public async Task SaveSettingAsync<T>(string key, T value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value);
        }
    }
}
