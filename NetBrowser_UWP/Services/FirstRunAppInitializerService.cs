using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser.Utils;
using NetBrowser_UWP.Contracts.Services.Settings;

namespace NetBrowser_UWP.Services;

public class FirstRunAppInitializerService : IFirstRunAppInitializerService
{
    private readonly IDataService _dataService;
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly AppConfigService _appConfigService;

    public FirstRunAppInitializerService(IDataService dataService,
        IGeneralSettingsService generalSettingsService,
        AppConfigService appConfigService)
    {
        _dataService = dataService;
        _generalSettingsService = generalSettingsService;
        _appConfigService = appConfigService;
    }

    public async Task InitializeAppStorageAsync()
    {
        try
        {
            await InitializeSearchEngineStorageAsync();
            await InitializeStartPageStorageAsync();
            await InitializeRssFeedersStorageAsync();
            _generalSettingsService.IsFirstRunInitResultSuccessful = true;
        }
        catch
        {
            _generalSettingsService.IsFirstRunInitResultSuccessful = false;
        }
    }

    private async Task InitializeStartPageStorageAsync()
    {
        var defaultStartPageItems = _appConfigService.GetSection<IEnumerable<SiteItem>>("DefaultStartPageItems");
        foreach (var startPageItem in defaultStartPageItems)
        {
            await _dataService.AddNewSiteOnStartPageAsync(startPageItem);
        }
    }

    private async Task InitializeSearchEngineStorageAsync()
    {
        var defaultEngineItems = _appConfigService.GetSection<IEnumerable<SearchEngineItem>>("DefaultSearchEngines");
        foreach (var searchEngine in defaultEngineItems)
        {
            await _dataService.AddSearchEngineAsync(searchEngine);
        }
    }

    private async Task InitializeRssFeedersStorageAsync()
    {
        var defaultFeedResources = _appConfigService.GetSection<IEnumerable<RssFeeder>>("FeedResources");
        await _dataService.AddRssFeedersAsync(defaultFeedResources);
    }
}