using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts.Services.Settings;

public interface ILocalSettingsService
{
    Task<T> ReadSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);

    T ReadSetting<T>(string key);

    void SaveSetting<T>(string key, T value);
}