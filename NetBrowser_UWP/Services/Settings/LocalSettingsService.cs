using System.Threading.Tasks;
using Windows.Storage;
using NetBrowser_UWP.Contracts.Services.Settings;
using NetBrowser_UWP.Helpers;

namespace NetBrowser_UWP.Services.Settings;

internal class LocalSettingsService : ILocalSettingsService
{
    public async Task<T> ReadSettingAsync<T>(string key)
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            return await Json.ToObjectAsync<T>((string)obj);

        return default;
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value);
    }

    public T ReadSetting<T>(string key)
    {
        return ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj)
            ? Json.ToObject<T>((string)obj)
            : default;
    }

    public void SaveSetting<T>(string key, T value)
    {
        ApplicationData.Current.LocalSettings.Values[key] = Json.Stringify(value);
    }
}