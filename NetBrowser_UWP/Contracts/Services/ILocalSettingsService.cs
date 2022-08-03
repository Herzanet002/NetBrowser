using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts.Services
{
    public interface ILocalSettingsService
    {
        Task<T> ReadSettingAsync<T>(string key);

        Task SaveSettingAsync<T>(string key, T value);
    }
}
