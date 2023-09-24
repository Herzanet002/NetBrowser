using System.Collections.Generic;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser.Core.Models;

namespace NetBrowser_UWP.IncrementalSources;

public class NewsIncrementalSourceFactory : INewsIncrementalSourceFactory
{
    private readonly INewsApiClientService _newsApiClientService;

    public NewsIncrementalSourceFactory(INewsApiClientService newsApiClientService)
    {
        _newsApiClientService = newsApiClientService;
    }

    public NewsIncrementalSource CreateNewsIncrementalSource()
        => new NewsIncrementalSource(_newsApiClientService);

    public NewsIncrementalSource CreateNewsByProvidersIncrementalSource(
        IEnumerable<NewsProvider> newsProviders)
        => new NewsIncrementalSource(_newsApiClientService, newsProviders);
}