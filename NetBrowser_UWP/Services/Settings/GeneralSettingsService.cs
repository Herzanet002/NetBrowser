using System;
using System.Runtime.CompilerServices;
using NetBrowser_UWP.Contracts.Services.Settings;

namespace NetBrowser_UWP.Services.Settings;

public class GeneralSettingsService : IGeneralSettingsService
{
    public event EventHandler<SettingChangedEventArgs> SettingChanged;

    private readonly ILocalSettingsService _localSettingsService;

    public GeneralSettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public bool IsFirstRunInitResultSuccessful
    {
        get => _localSettingsService.ReadSetting<bool>(nameof(IsFirstRunInitResultSuccessful));
        set
        {
            _localSettingsService.SaveSetting(nameof(IsFirstRunInitResultSuccessful), value);
            OnSettingsChanged(value);
        }
    }

    private void OnSettingsChanged(object newValue, [CallerMemberName] string propertyName = "")
        => SettingChanged?.Invoke(this, new SettingChangedEventArgs(propertyName, newValue));
}