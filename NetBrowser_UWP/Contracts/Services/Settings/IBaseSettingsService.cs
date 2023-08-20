using System;
using NetBrowser_UWP.Services.Settings;

namespace NetBrowser_UWP.Contracts.Services.Settings;

public interface IBaseSettingsService
{
    event EventHandler<SettingChangedEventArgs> SettingChanged;
}