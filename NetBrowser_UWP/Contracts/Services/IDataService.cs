using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser_UWP.Models;
using NetBrowser.Utils;

namespace NetBrowser_UWP.Contracts.Services;

public interface IDataService
{
    Task SaveHistoryAsync(HistoryItem historyItem);

    Task<List<HistoryItem>> GetHistoryAsync();

    Task ClearAllHistoryAsync();

    Task RemoveHistoryItemAsync(HistoryItem historyItem);

    Task SaveSearchTermAsync(SiteItem siteItem);

    Task<List<SiteItem>> GetSearchTermsAsync();

    Task SaveBookmarkAsync(BookmarkItem bookmarkItem);

    Task EditBookmarkAsync(BookmarkItem oldBookmark, BookmarkItem newBookmark);

    Task<List<BookmarkItem>> GetBookmarksAsync();

    Task RemoveBookmarkAsync(BookmarkItem bookmarkItem);

    Task SaveNewsContentToFavoriteAsync(ContentModel contentItem);

    Task<List<ContentModel>> GetAllFavoriteNewsContentAsync();

    Task RemoveNewsContentFromFavoriteAsync(ContentModel contentModel);

    Task<List<RssFeeder>> GetRssFeedersListAsync();

    Task AddRssFeedersAsync(IEnumerable<RssFeeder> feeders);

    Task AddLikedRssFeedersAsync(IEnumerable<RssFeeder> feeders);

    Task<List<RssFeeder>> GetLikedRssFeedersAsync();

    Task ClearAllLikedRssFeedersAsync();
    
    Task SetDefaultSearchEngineAsync(SearchEngineItem searchEngine);

    Task<List<SearchEngineItem>> GetSearchEngineListAsync();

    Task<SearchEngineItem> GetCurrentSearchEngineAsync();

    Task AddSearchEngineAsync(SearchEngineItem engineItem);

    Task<List<SiteItem>> GetStartPageElementsAsync();

    Task EditStartPageItemAsync(SiteItem oldItem, SiteItem newItem);

    Task AddNewSiteOnStartPageAsync(SiteItem siteItem);

    Task RemoveSiteOnStartPageAsync(SiteItem siteItem);
}