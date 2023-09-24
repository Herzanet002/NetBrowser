using System.Collections.Generic;
using NetBrowser.Core.Models;

namespace NetBrowser_UWP.IncrementalSources;

public interface INewsIncrementalSourceFactory
{
    NewsIncrementalSource CreateNewsIncrementalSource();

    NewsIncrementalSource CreateNewsByProvidersIncrementalSource(IEnumerable<NewsProvider> newsProviders);
}