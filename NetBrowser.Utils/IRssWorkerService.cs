using System.Collections.Generic;

namespace NetBrowser.Utils;

public interface IRssWorkerService
{
    IAsyncEnumerable<ContentModel> GetFeeds(IEnumerable<RssFeeder> rssFeeders,
        IEnumerable<ContentModel> favoriteItems, int? limit);
}