using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services.Settings;

namespace NetBrowser_UWP.Contracts.Services.Settings;

public interface IAppearanceSettingsService : IBaseSettingsService
{
    /// <summary>
    ///     Gets or sets a selected theme of the application
    /// </summary>
    SettingHolder<ThemeItem> SelectedTheme { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether the suggestion
    ///     bar on the start page is enabled
    /// </summary>
    SettingHolder<bool> IsSuggestionBarEnabled { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether the animation on
    ///     the start page is enabled
    /// </summary>

    SettingHolder<bool> IsAnimationEnabled { get; }

    /// <summary>
    ///     Gets or sets a value indicating the orientation of the grid
    ///     tiles on the start page
    /// </summary>
    SettingHolder<int> StartPageGridViewOrientation { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether the home button on
    ///     the address bar is enabled
    /// </summary>
    SettingHolder<bool> IsHomeButtonEnabled { get; }
}