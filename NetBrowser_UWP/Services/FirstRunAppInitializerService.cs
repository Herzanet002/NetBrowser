using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser.Utils;

namespace NetBrowser_UWP.Services;

public class FirstRunAppInitializerService : IFirstRunAppInitializerService
{
    private readonly IDataService _dataService;
    private readonly AppConfigService _appConfigService;

    public FirstRunAppInitializerService(IDataService dataService,
        AppConfigService appConfigService)
    {
        _dataService = dataService;
        _appConfigService = appConfigService;
    }

    public Task InitializeConfigStorageAsync()
    {
        throw new NotImplementedException();
    }

    public async Task InitializeStartPageStorageAsync()
    {
        var defaultStartPageItems = _appConfigService.GetSection<IEnumerable<SiteItem>>("DefaultStartPageItems");
        foreach (var startPageItem in defaultStartPageItems)
        {
            await _dataService.AddNewSiteOnStartPageAsync(startPageItem);
        }
    }

    public async Task InitializeSearchEngineStorageAsync()
    {
        var defaultEngineItems = _appConfigService.GetSection<IEnumerable<SearchEngineItem>>("DefaultSearchEngines");
        foreach (var searchEngine in defaultEngineItems)
        {
            await _dataService.AddSearchEngineAsync(searchEngine);
        }
    }

    public async Task InitializeRssFeeders()
    {
        var defaultFeedResources = _appConfigService.GetSection<IEnumerable<RssFeeder>>("FeedResources");
        await _dataService.AddRssFeedersAsync(defaultFeedResources);
    }
}