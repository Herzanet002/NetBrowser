using System;
using System.Runtime.CompilerServices;
using NetBrowser_UWP.Contracts.Services.Settings;
using NetBrowser_UWP.Enums;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Services.Settings;

public class AppearanceSettingsService : IAppearanceSettingsService
{
    public event EventHandler<SettingChangedEventArgs> SettingChanged;

    private readonly ILocalSettingsService _localSettingsService;

    public AppearanceSettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public ThemeItem SelectedTheme
    {
        get => App.ThemeManager.GetRequestedTheme(_localSettingsService
            .ReadSetting<string>(nameof(SelectedTheme)));

        set
        {
            _localSettingsService.SaveSetting(nameof(SelectedTheme), value?.Name);
            OnSettingsChanged(value);
        }
    }

    public bool IsSuggestionBarEnabled
    {
        get => _localSettingsService.ReadSetting<bool>(nameof(IsSuggestionBarEnabled));
        set
        {
            _localSettingsService.SaveSetting(nameof(IsSuggestionBarEnabled), value);
            OnSettingsChanged(value);
        }
    }

    public bool IsAnimationEnabled
    {
        get => _localSettingsService.ReadSetting<bool>(nameof(IsAnimationEnabled));
        set
        {
            _localSettingsService.SaveSetting(nameof(IsAnimationEnabled), value);
            OnSettingsChanged(value);
        }
    }

    public int StartPageGridViewOrientation
    {
        get => _localSettingsService.ReadSetting<int>(nameof(StartPageGridViewOrientation));
        set
        {
            _localSettingsService.SaveSetting(nameof(StartPageGridViewOrientation), value);
            OnSettingsChanged(value);
        }
    }

    public bool IsHomeButtonEnabled
    {
        get => _localSettingsService.ReadSetting<bool>(nameof(IsHomeButtonEnabled));
        set
        {
            _localSettingsService.SaveSetting(nameof(IsHomeButtonEnabled), value);
            OnSettingsChanged(value);
        }
    }

    public TabViewPlacementMode TabViewPlacementMode
    {
        get => (TabViewPlacementMode)_localSettingsService.ReadSetting<int>(nameof(TabViewPlacementMode));
        set
        {
            _localSettingsService.SaveSetting(nameof(TabViewPlacementMode), (int)value);
            OnSettingsChanged(value);
        }
    }

    private void OnSettingsChanged<T>(T newValue, [CallerMemberName] string propertyName = "")
        => SettingChanged?.Invoke(this, new SettingChangedEventArgs(propertyName, newValue));
}