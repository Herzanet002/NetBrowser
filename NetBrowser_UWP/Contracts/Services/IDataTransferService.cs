using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Contracts.Services;

public interface IDataTransferService
{
    Task<XmlDocument> DocumentLoad(string configFileName);
    Task SaveDoc(XmlDocument doc, string fileName);
    Task SaveHistory(HistoryItemDetails historyItemDetail);
    Task SaveSearchTerm(SiteItem siteItem);
    Task<IList<HistoryItemDetails>> GetHistory();
    Task<IList<SiteItem>> GetSearchTerm();

    Task SaveBookmark(BookmarkDetails bookmarkDetails);

    //Task<string> GetCurrentTheme();
    //void SaveCurrentTheme(string themeMode);
    Task<IList<BookmarkDetails>> GetBookmarksList();
    Task<bool> RemoveBookmark(BookmarkDetails bookmarkDetails);
    Task<bool> ClearHistoryFile();
    Task<bool> RemoveHistoryItem(HistoryItemDetails historyItemDetails);
    Task EditBookmark(BookmarkDetails oldBookmark, BookmarkDetails newBookmark);
    Task<IList<SearchEngineItem>> GetSearchEngineList();
    Task<SearchEngineItem> GetCurrentSearchEngine();
    Task<IList<SiteItem>> GetStartPageElements();
    Task EditStartPageItem(SiteItem oldItem, SiteItem newItem);
    Task AddNewSiteOnStartPage(SiteItem siteItem);
    Task<bool> RemoveSiteOnStartPage(SiteItem siteItem);
    Task ChangeSearchEngine(string newEngine);
}