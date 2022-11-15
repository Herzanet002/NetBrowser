using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using static NetBrowser_UWP.Constants.Constants;

namespace NetBrowser_UWP.Services;

public class DataTransferService : IDataTransferService
{
    public async Task SaveHistory(HistoryItemDetails historyItemDetail)
    {
        if (historyItemDetail.Url == "about:blank") return;
        var doc = await DocumentLoad(HISTORY_FILE_NAME).AsAsyncOperation();

        var history = doc.GetElementsByTagName("history");

        var siteElement = doc.CreateElement("siteName");
        var siteUrl = doc.CreateElement("url");
        var timeElement = doc.CreateElement("hour");
        var dateElement = doc.CreateElement("date");

        var historyItem = history[0].AppendChild(doc.CreateElement("historyitem"));

        historyItem.AppendChild(siteElement);
        historyItem.AppendChild(siteUrl);
        historyItem.AppendChild(timeElement);
        historyItem.AppendChild(dateElement);

        siteElement.InnerText = historyItemDetail.Name;
        siteUrl.InnerText = historyItemDetail.Url;
        timeElement.InnerText = historyItemDetail.Time;
        dateElement.InnerText = historyItemDetail.Date;

        await SaveDoc(doc, HISTORY_FILE_NAME).ConfigureAwait(false);
    }

    public async Task SaveSearchTerm(SiteItem siteItem)
    {
        if (siteItem == null) return;
        var doc = await DocumentLoad(HISTORY_FILE_NAME).AsAsyncOperation();

        var history = doc.GetElementsByTagName("history");

        var elSiteName = doc.CreateElement("termName");

        var historyItem = history[0].AppendChild(doc.CreateElement("searchTerm"));

        historyItem.AppendChild(elSiteName);

        elSiteName.InnerText = siteItem.Name;

        await SaveDoc(doc, HISTORY_FILE_NAME).ConfigureAwait(false);
    }

    public async Task<XmlDocument> DocumentLoad(string configFileName)
    {
        XmlDocument result = null;

        await Task.Run(async () =>
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(configFileName);
            result = await XmlDocument.LoadFromFileAsync(file);
        });
        return result;
    }

    public async Task SaveDoc(XmlDocument doc, string fileName)
    {
        var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
        await doc.SaveToFileAsync(file);
    }

    public async Task<IList<HistoryItemDetails>> GetHistory()
    {
        var listOfHistory = new List<HistoryItemDetails>();
        var doc = await DocumentLoad(HISTORY_FILE_NAME);

        var historyItem = doc.GetElementsByTagName("historyitem");

        foreach (var item in historyItem)
        {
            var historyItemChild = item.ChildNodes;
            listOfHistory.Add(new HistoryItemDetails
            {
                Name = historyItemChild[0].InnerText,
                Url = historyItemChild[1].InnerText,
                Time = historyItemChild[2].InnerText,
                Date = historyItemChild[3].InnerText
            });
        }

        return listOfHistory;
    }

    public async Task<IList<SiteItem>> GetSearchTerm()
    {
        var doc = await DocumentLoad(HISTORY_FILE_NAME);
        var historyItem = doc.GetElementsByTagName("searchTerm");
        return historyItem.Select(item => item.ChildNodes)
            .Select(history => new SiteItem
            {
                Name = history[0].InnerText
            })
            .ToList();
    }

    public async Task SaveBookmark(BookmarkDetails bookmarkDetails)
    {
        var doc = await DocumentLoad(BOOKMARKS_FILE_NAME);

        var bookmarks = doc.GetElementsByTagName("bookmarks");

        var bookmark = bookmarks[0].AppendChild(doc.CreateElement("bookmark"));
        var bookmarkUrl = bookmark.AppendChild(doc.CreateElement("url"));
        var bookmarkTitle = bookmark.AppendChild(doc.CreateElement("title"));
        var bookmarkIcon = bookmark.AppendChild(doc.CreateElement("icon"));

        bookmarkUrl.InnerText = bookmarkDetails.Url;
        bookmarkTitle.InnerText = bookmarkDetails.Name;
        bookmarkIcon.InnerText = bookmarkDetails.FaviconUrl;

        await SaveDoc(doc, BOOKMARKS_FILE_NAME).ConfigureAwait(false);
    }

    public async Task<IList<BookmarkDetails>> GetBookmarksList()
    {
        var doc = await DocumentLoad(BOOKMARKS_FILE_NAME);
        var bookmarks = doc.GetElementsByTagName("bookmark");

        return bookmarks.Select(item => item.ChildNodes)
            .Select(bookmark => new BookmarkDetails
            {
                Url = bookmark[0].InnerText,
                Name = bookmark[1].InnerText,
                FaviconUrl = bookmark[2].InnerText
            }).ToList();
    }

    public async Task<bool> RemoveBookmark(BookmarkDetails bookmarkDetails)
    {
        var doc = await DocumentLoad(BOOKMARKS_FILE_NAME);
        var bookmarks = doc.GetElementsByTagName("bookmark");
        var root = doc.DocumentElement;
        IXmlNode found = null;

        foreach (var item in bookmarks)
        {
            var child = item.ChildNodes;
            if (child[0].InnerText != bookmarkDetails.Url) continue;
            found = item;
        }

        if (found == null)
            return false;
        root.RemoveChild(found);
        await SaveDoc(doc, BOOKMARKS_FILE_NAME);
        return true;
    }

    public async Task<bool> ClearHistoryFile()
    {
        var doc = await DocumentLoad(HISTORY_FILE_NAME);
        var root = doc.DocumentElement;
        var isSuccess = false;
        while (root.ChildNodes.Count > 0)
            root.RemoveChild(root.ChildNodes[0]);

        if (root.ChildNodes.Count == 0)
            isSuccess = true;

        await SaveDoc(doc, HISTORY_FILE_NAME);
        return isSuccess;
    }

    public async Task<bool> RemoveHistoryItem(HistoryItemDetails historyItem)
    {
        var doc = await DocumentLoad(HISTORY_FILE_NAME);
        var root = doc.DocumentElement;
        var result = false;
        var history = doc.GetElementsByTagName("historyitem");
        for (var i = 0; i < history.Count; i++)
        {
            var child = history[i].ChildNodes;

            for (var j = 0; j < child.Count; j++)
                if (child[j].NodeName == "hour")
                    if (child[j].InnerText == historyItem.Time)
                    {
                        root.RemoveChild(history[i]);
                        result = true;
                        break;
                    }
        }

        await SaveDoc(doc, HISTORY_FILE_NAME).ConfigureAwait(false);
        return result;
    }

    public async Task EditBookmark(BookmarkDetails oldBookmark, BookmarkDetails newBookmark)
    {
        if (oldBookmark == null || newBookmark == null || oldBookmark == newBookmark) return;
        var doc = await DocumentLoad(BOOKMARKS_FILE_NAME);
        var bookmarks = doc.GetElementsByTagName("bookmark");
        foreach (var bookmark in bookmarks)
        {
            var child = bookmark.ChildNodes;
            if (child[0].InnerText != oldBookmark.Url || child[1].InnerText != oldBookmark.Name) continue;
            child[0].InnerText = newBookmark.Url;
            child[1].InnerText = newBookmark.Name;
            child[2].InnerText = newBookmark.FaviconUrl;
        }

        await SaveDoc(doc, BOOKMARKS_FILE_NAME).ConfigureAwait(false);
    }

    public async Task<IList<SearchEngineItem>> GetSearchEngineList()
    {
        var engineList = new List<SearchEngineItem>();

        var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILE_NAME);
        var doc = await XmlDocument.LoadFromFileAsync(file);

        var searchEngine = doc.GetElementsByTagName("searchEngine");
        foreach (var t in searchEngine)
        {
            var searchChild = t.ChildNodes;

            foreach (var child in searchChild)
            {
                if (child.NodeName != "engine") continue;
                var engineItem = new SearchEngineItem
                {
                    Prefix = child.Attributes.GetNamedItem("prefix")?.InnerText,
                    Name = child.Attributes.GetNamedItem("name")?.InnerText,
                    IsSelected = child.Attributes.GetNamedItem("mode")?.InnerText,
                    HomePage = child.Attributes.GetNamedItem("homePage")?.InnerText
                };
                engineList.Add(engineItem);
            }
        }

        return engineList;
    }

    public async Task<SearchEngineItem> GetCurrentSearchEngine()
    {
        var current = new SearchEngineItem();
        await Task.Run(async () =>
        {
            var engines = await GetSearchEngineList();
            foreach (var engine in engines.Where(engine => engine.IsSelected == "1")) current = engine;
        });
        return current;
    }

    public async Task<IList<SiteItem>> GetStartPageElements()
    {
        var doc = await DocumentLoad(STARTPAGE_FILE_NAME);
        var startPageElements = doc.GetElementsByTagName("element");

        return startPageElements.Select(item => item.ChildNodes)
            .Select(page => new SiteItem
            {
                Name = page[0].InnerText,
                Url = page[1].InnerText
            }).ToList();
    }

    public async Task EditStartPageItem(SiteItem oldItem, SiteItem newItem)
    {
        if (oldItem == null || newItem == null || oldItem == newItem) return;

        var doc = await DocumentLoad(STARTPAGE_FILE_NAME);
        var elements = doc.GetElementsByTagName("element");
        foreach (var element in elements)
        {
            var child = element.ChildNodes;
            if (child[0].InnerText != oldItem.Name || child[1].InnerText != oldItem.Url) continue;
            child[0].InnerText = newItem.Url;
            child[1].InnerText = newItem.Name;
        }

        await SaveDoc(doc, BOOKMARKS_FILE_NAME).ConfigureAwait(false);
    }

    public async Task AddNewSiteOnStartPage(SiteItem siteItem)
    {
        var doc = await DocumentLoad(STARTPAGE_FILE_NAME);

        var elements = doc.GetElementsByTagName("startPageElements");

        var page = elements[0].AppendChild(doc.CreateElement("element"));
        var pageTitle = page.AppendChild(doc.CreateElement("name"));
        var pageUrl = page.AppendChild(doc.CreateElement("url"));

        pageTitle.InnerText = siteItem.Name;
        pageUrl.InnerText = siteItem.Url;


        await SaveDoc(doc, STARTPAGE_FILE_NAME).ConfigureAwait(false);
    }

    public async Task<bool> RemoveSiteOnStartPage(SiteItem siteItem)
    {
        var doc = await DocumentLoad(STARTPAGE_FILE_NAME);
        var elements = doc.GetElementsByTagName("element");
        var root = doc.DocumentElement;
        IXmlNode found = null;
        foreach (var item in elements)
        {
            var child = item.ChildNodes;
            if (child[0].InnerText != siteItem.Name || child[1].InnerText != siteItem.Url) continue;
            found = item;
        }

        if (found == null)
            return false;
        root.RemoveChild(found);
        await SaveDoc(doc, STARTPAGE_FILE_NAME);
        return true;
    }

    public async Task ChangeSearchEngine(string newEngine)
    {
        if (string.IsNullOrWhiteSpace(newEngine)) return;
        var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILE_NAME);
        var doc = await XmlDocument.LoadFromFileAsync(file);

        var searchEngine = doc.GetElementsByTagName("searchEngine");
        foreach (var engine in searchEngine)
        {
            var searchChild = engine.ChildNodes;

            foreach (var child in searchChild)
            {
                var attr = child.Attributes;
                if (child.NodeName != "engine") continue;

                attr.GetNamedItem("mode")!.InnerText = attr.GetNamedItem("name")?.InnerText == newEngine ? "1" : "0";
            }
        }

        await SaveDoc(doc, SETTINGS_FILE_NAME).ConfigureAwait(false);
    }
}