using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Contracts.Services;

public interface IDataTransferService
{
    Task<XmlDocument> DocumentLoad(string configFileName);
    Task SaveDocAsync(XmlDocument doc, string fileName);
    Task SaveHistoryAsync(HistoryItemDetails historyItemDetail);
    Task SaveSearchTermAsync(SiteItem siteItem);
    Task<IList<HistoryItemDetails>> GetHistoryAsync();
    Task<IList<SiteItem>> GetSearchTermAsync();
    Task SaveBookmarkAsync(BookmarkDetails bookmarkDetails);
    Task<IList<BookmarkDetails>> GetBookmarksListAsync();
    Task<bool> RemoveBookmarkAsync(BookmarkDetails bookmarkDetails);
    Task SaveNewsContentToFavoriteAsync(ContentModel contentModel, CancellationToken ct);
    Task<bool> ClearHistoryFileAsync();
    Task<bool> RemoveHistoryItemAsync(HistoryItemDetails historyItemDetails);
    Task EditBookmarkAsync(BookmarkDetails oldBookmark, BookmarkDetails newBookmark);
    Task<IList<SearchEngineItem>> GetSearchEngineListAsync();
    Task<SearchEngineItem> GetCurrentSearchEngineAsync();
    Task<IList<SiteItem>> GetStartPageElementsAsync();
    Task EditStartPageItemAsync(SiteItem oldItem, SiteItem newItem);
    Task AddNewSiteOnStartPageAsync(SiteItem siteItem);
    Task<bool> RemoveSiteOnStartPageAsync(SiteItem siteItem);
    Task ChangeSearchEngineAsync(string newEngine);
}