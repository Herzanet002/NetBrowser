using System;
using NetBrowser.Core.EventArguments;

namespace NetBrowser_UWP.Contracts.Services.Settings;

public interface IBaseSettingsService
{
    event EventHandler<SettingChangedEventArgs> SettingChanged;
}