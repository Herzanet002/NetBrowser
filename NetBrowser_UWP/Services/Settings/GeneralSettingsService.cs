using NetBrowser_UWP.Contracts.Services.Settings;

namespace NetBrowser_UWP.Services.Settings;

public class GeneralSettingsService : IGeneralSettingsService
{
    private readonly ILocalSettingsService _localSettingsService;

    public GeneralSettingsService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public bool IsFirstRunInitResultSuccessful
    {
        get => _localSettingsService.ReadSetting<bool>(nameof(IsFirstRunInitResultSuccessful));
        set => _localSettingsService.SaveSetting(nameof(IsFirstRunInitResultSuccessful), value);
    }
}