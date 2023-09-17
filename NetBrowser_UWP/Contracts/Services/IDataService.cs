using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Contracts.Services;

public interface IDataService
{
    Task SaveHistoryAsync(HistoryItem historyItem);

    Task<List<HistoryItem>> GetHistoryAsync();

    Task ClearAllHistoryAsync();

    Task RemoveHistoryItemAsync(HistoryItem historyItem);

    Task AddOrReplaceSearchTermAsync(SearchTermItem searchTermItem);

    Task<List<SearchTermItem>> GetSearchTermsAsync();

    Task SaveBookmarkAsync(BookmarkItem bookmarkItem);

    Task EditBookmarkAsync(BookmarkItem oldBookmark, BookmarkItem newBookmark);

    Task<List<BookmarkItem>> GetBookmarksAsync();

    Task RemoveBookmarkAsync(BookmarkItem bookmarkItem);

    Task ClearLikedNewsProvidersAsync();

    Task<List<NewsProvider>> GetLikedNewsProvidersAsync();

    Task AddLikedNewsProvidersAsync(IEnumerable<NewsProvider> feeders);

    Task SaveNewsContentToFavoriteAsync(ContentModel content);

    Task<List<ContentModel>> GetAllFavoritesNewsContentAsync();

    Task RemoveNewsContentFromFavoritesAsync(ContentModel content);

    Task SetDefaultSearchEngineAsync(SearchEngineItem searchEngine);

    Task<List<SearchEngineItem>> GetSearchEngineListAsync();

    Task<SearchEngineItem> GetCurrentSearchEngineAsync();

    Task AddSearchEngineAsync(SearchEngineItem engineItem);

    Task<List<SiteItem>> GetStartPageElementsAsync();

    Task EditStartPageItemAsync(SiteItem oldItem, SiteItem newItem);

    Task AddNewSiteOnStartPageAsync(SiteItem siteItem);

    Task RemoveSiteOnStartPageAsync(SiteItem siteItem);
}