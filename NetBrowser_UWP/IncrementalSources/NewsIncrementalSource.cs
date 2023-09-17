using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Collections;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.IncrementalSources;

public class NewsIncrementalSource : IIncrementalSource<ContentModel>
{
    private readonly INewsApiClientService _newsApiClientService;
    private readonly IEnumerable<NewsProvider> _newsProviders;

    public NewsIncrementalSource(INewsApiClientService newsApiClientService, IEnumerable<NewsProvider> newsProviders)
    {
        _newsApiClientService = newsApiClientService;
        _newsProviders = newsProviders;
    }

    public NewsIncrementalSource(INewsApiClientService newsApiClientService)
        => _newsApiClientService = newsApiClientService;

    public async Task<IEnumerable<ContentModel>> GetPagedItemsAsync(int pageIndex, int pageSize,
        CancellationToken cancellationToken)
    {
        return _newsProviders == null
            ? await _newsApiClientService.GetNewsAsync(pageSize, pageIndex)
            : await _newsApiClientService.GetNewsByProvidersAsync(_newsProviders, pageSize, pageIndex);
    }
}