using NetBrowser_UWP.Contracts.Services.Settings;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Services.Settings;

public class AppearanceSettingsService : IAppearanceSettingsService
{
    private readonly ILocalSettingsService _localSettingsService;

    public AppearanceSettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public SettingHolder<ThemeItem> SelectedTheme => new(
        onGetAction: () => App.ThemeManager.GetRequestedTheme(_localSettingsService
            .ReadSetting<string>(nameof(SelectedTheme))),
        onSetAction: setItem => _localSettingsService.SaveSetting(nameof(SelectedTheme), setItem?.Name));

    public SettingHolder<bool> IsSuggestionBarEnabled => new(
        onGetAction: () => _localSettingsService.ReadSetting<bool>(nameof(IsSuggestionBarEnabled)),
        onSetAction: setItem => _localSettingsService.SaveSetting(nameof(IsSuggestionBarEnabled), setItem));

    public SettingHolder<bool> IsAnimationEnabled => new(
        onGetAction: () => _localSettingsService.ReadSetting<bool>(nameof(IsAnimationEnabled)),
        onSetAction: setItem => _localSettingsService.SaveSetting(nameof(IsAnimationEnabled), setItem));

    public SettingHolder<int> StartPageGridViewOrientation => new(
        onGetAction: () => _localSettingsService.ReadSetting<int>(nameof(StartPageGridViewOrientation)),
        onSetAction: setItem => _localSettingsService.SaveSetting(nameof(StartPageGridViewOrientation), setItem));

    public SettingHolder<bool> IsHomeButtonEnabled => new(
        onGetAction: () => _localSettingsService.ReadSetting<bool>(nameof(IsHomeButtonEnabled)),
        onSetAction: setItem => _localSettingsService.SaveSetting(nameof(IsHomeButtonEnabled), setItem));
}