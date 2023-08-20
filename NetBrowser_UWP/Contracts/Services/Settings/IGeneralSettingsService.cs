using NetBrowser_UWP.Services.Settings;

namespace NetBrowser_UWP.Contracts.Services.Settings;

public interface IGeneralSettingsService
{
    /// <summary>
    ///     Gets or sets a value indicating whether the initial
    ///     initialization of the application store was successful 
    /// </summary>
    SettingHolder<bool> IsFirstRunInitResultSuccessful { get; }
}