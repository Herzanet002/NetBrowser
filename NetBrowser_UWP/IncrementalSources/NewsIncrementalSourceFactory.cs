using System.Collections.Generic;
using Microsoft.Toolkit.Collections;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;

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