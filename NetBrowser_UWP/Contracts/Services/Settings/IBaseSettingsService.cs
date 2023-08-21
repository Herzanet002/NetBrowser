using NetBrowser_UWP.Services.Settings;
using System;

namespace NetBrowser_UWP.Contracts.Services.Settings;

public interface IBaseSettingsService
{
    event EventHandler<SettingChangedEventArgs> SettingChanged;
}