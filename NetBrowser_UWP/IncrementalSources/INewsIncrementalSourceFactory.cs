using System.Collections.Generic;
using Microsoft.Toolkit.Collections;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.IncrementalSources;

public interface INewsIncrementalSourceFactory
{
    NewsIncrementalSource CreateNewsIncrementalSource();

    NewsIncrementalSource CreateNewsByProvidersIncrementalSource(IEnumerable<NewsProvider> newsProviders);
}