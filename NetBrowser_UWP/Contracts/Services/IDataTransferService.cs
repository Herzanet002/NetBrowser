using NetBrowser_UWP.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace NetBrowser_UWP.Contracts.Services
{
    public interface IDataTransferService
    {
        public Task<XmlDocument> DocumentLoad(string configFileName);
        public void SaveDoc(XmlDocument doc, string fileName);
        public void SaveHistory(HistoryItemDetails historyItemDetail);
        public void SaveSearchTerm(string title);
        public Task<List<HistoryItemDetails>> GetHistory();
        public Task<List<SiteItem>> GetSearchTerm();
        public void SaveBookmark(BookmarkDetails bookmarkDetails);
        public Task<string> GetCurrentTheme();
        public void SaveCurrentTheme(string themeMode);
        public Task<List<BookmarkDetails>> GetBookmarkList();
        public Task<bool> RemoveBookmark(BookmarkDetails bookmarkDetails);
        public Task<bool> ClearHistoryFile();
        public Task<bool> RemoveHistoryItem(HistoryItemDetails historyItemDetails);
        public void EditBookmark(string oldUrl, string newUrl, string newTitle);
        public Task<List<SearchEngineItem>> GetSearchEngineList();
        public Task<SearchEngineItem> GetCurrentSearchEngine();
        public Task<List<SiteItem>> GetStartPageElements();
        public void AddNewSiteOnStartPage(SiteItem siteItem);
        public void RemoveSiteOnStartPage(SiteItem siteItem);
        public void ChangeSearchEngine(string newEngine);





    }
}
