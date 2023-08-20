using NetBrowser_UWP.Contracts.Services.Settings;

namespace NetBrowser_UWP.Services.Settings;

public class GeneralSettingsService : IGeneralSettingsService
{
    private readonly ILocalSettingsService _localSettingsService;

    public GeneralSettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public SettingHolder<bool> IsFirstRunInitResultSuccessful => new(
        onGetAction: () => _localSettingsService.ReadSetting<bool>(nameof(IsFirstRunInitResultSuccessful)),
        onSetAction: setItem => _localSettingsService.SaveSetting(nameof(IsFirstRunInitResultSuccessful), setItem));
}