using NetBrowser_UWP.Enums;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Contracts.Services.Settings;

public interface IAppearanceSettingsService : IBaseSettingsService
{
    /// <summary>
    ///     Gets or sets a selected theme of the application
    /// </summary>
    ThemeItem SelectedTheme { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the suggestion
    ///     bar on the start page is enabled
    /// </summary>
    bool IsSuggestionBarEnabled { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the animation on
    ///     the start page is enabled
    /// </summary>

    bool IsAnimationEnabled { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating the orientation of the grid
    ///     tiles on the start page
    /// </summary>
    int StartPageGridViewOrientation { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the home button on
    ///     the address bar is enabled
    /// </summary>
    bool IsHomeButtonEnabled { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating the placement of the custom tabview
    /// </summary>
    TabViewPlacementMode TabViewPlacementMode { get; set; }
}