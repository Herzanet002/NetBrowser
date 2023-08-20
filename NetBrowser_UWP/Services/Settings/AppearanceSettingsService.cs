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

    public ThemeItem SelectedTheme
    {
        get => App.ThemeManager.GetRequestedTheme(_localSettingsService
            .ReadSetting<string>(nameof(SelectedTheme)));

        set => _localSettingsService.SaveSetting(nameof(SelectedTheme), value?.Name);
    }

    public bool IsSuggestionBarEnabled
    {
        get => _localSettingsService.ReadSetting<bool>(nameof(IsSuggestionBarEnabled));
        set => _localSettingsService.SaveSetting(nameof(IsSuggestionBarEnabled), value);
    }

    public bool IsAnimationEnabled
    {
        get => _localSettingsService.ReadSetting<bool>(nameof(IsAnimationEnabled));
        set => _localSettingsService.SaveSetting(nameof(IsAnimationEnabled), value);
    }

    public int StartPageGridViewOrientation
    {
        get => _localSettingsService.ReadSetting<int>(nameof(StartPageGridViewOrientation));
        set => _localSettingsService.SaveSetting(nameof(StartPageGridViewOrientation), value);
    }

    public bool IsHomeButtonEnabled
    {
        get => _localSettingsService.ReadSetting<bool>(nameof(IsHomeButtonEnabled));
        set => _localSettingsService.SaveSetting(nameof(IsHomeButtonEnabled), value);
    }
}