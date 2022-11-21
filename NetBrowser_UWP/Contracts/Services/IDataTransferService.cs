using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Contracts.Services;

public interface IDataTransferService
{
    Task SaveHistoryAsync(HistoryItemDetails historyItemDetail);
    Task SaveSearchTermAsync(SiteItem siteItem);
    Task<IList<HistoryItemDetails>> GetHistoryAsync();
    Task<IList<SiteItem>> GetSearchTermAsync();
    Task SaveBookmarkAsync(BookmarkDetails bookmarkDetails);
    Task<IList<BookmarkDetails>> GetBookmarksListAsync();
    Task RemoveBookmarkAsync(BookmarkDetails bookmarkDetails);
    Task SaveNewsContentToFavoriteAsync(ContentModel contentModel, CancellationToken ct);
    Task ClearHistoryFileAsync();
    Task RemoveHistoryItemAsync(HistoryItemDetails historyItemDetails);
    Task EditBookmarkAsync(BookmarkDetails oldBookmark, BookmarkDetails newBookmark);
    Task<IList<SearchEngineItem>> GetSearchEngineListAsync();
    Task<SearchEngineItem> GetCurrentSearchEngineAsync();
    Task<IList<StartPageItem>> GetStartPageElementsAsync();
    Task EditStartPageItemAsync(StartPageItem oldItem, StartPageItem newItem);
    Task AddNewSiteOnStartPageAsync(StartPageItem siteItem);
    Task RemoveSiteOnStartPageAsync(StartPageItem siteItem);
    Task ChangeSearchEngineAsync(string newEngine);
}