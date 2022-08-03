using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.System;
using static NetBrowser_UWP.Constants.Constants;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

namespace NetBrowser_UWP.Services
{
    public class DataTransferService : IDataTransferService
    {
        public async void SaveHistory(HistoryItemDetails historyItemDetail)
        {
            try
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

                SaveDoc(doc, HISTORY_FILE_NAME);
            }
            catch
            {
                // ignored
            }
        }

        public async void SaveSearchTerm(string title)
        {
            try
            {
                if (title == string.Empty) return;
                var doc = await DocumentLoad(HISTORY_FILE_NAME).AsAsyncOperation();

                var history = doc.GetElementsByTagName("history");

                var elSiteName = doc.CreateElement("termName");

                var historyItem = history[0].AppendChild(doc.CreateElement("searchTerm"));

                historyItem.AppendChild(elSiteName);

                elSiteName.InnerText = title;

                SaveDoc(doc, HISTORY_FILE_NAME);

            }
            catch
            {
                // ignored
            }
        }

        public async Task<XmlDocument> DocumentLoad(string configFileName)
        {
            XmlDocument result = null;

            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(configFileName);
                var doc = await XmlDocument.LoadFromFileAsync(file);
                doc.Normalize();
                result = doc;
            });
            return result;
        }

        public async void SaveDoc(XmlDocument doc, string fileName)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            await doc.SaveToFileAsync(file);
        }

        public async Task<List<HistoryItemDetails>> GetHistory()
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
                    Date = historyItemChild[3].InnerText,
                });
            }


            return listOfHistory;
        }

        public async Task<List<string>> GetSearchTerm()
        {
            var listOfTerms = new List<string>();
            var doc = await DocumentLoad(HISTORY_FILE_NAME);

            var historyItem = doc.GetElementsByTagName("searchTerm");
            listOfTerms.AddRange(from item in historyItem
                                 from child in item.ChildNodes
                                 where child.NodeName == "termName"
                                 select child.InnerText);

            return listOfTerms;
        }

        public async void SaveBookmark(BookmarkDetails bookmarkDetails)
        {
            var doc = await DocumentLoad(BOOKMARKS_FILE_NAME);

            var bookmarks = doc.GetElementsByTagName("bookmarks");

            var bookmark = bookmarks[0].AppendChild(doc.CreateElement("bookmark"));
            var bookmarkUrl = bookmark.AppendChild(doc.CreateElement("url"));
            var bookmarkTitle = bookmark.AppendChild(doc.CreateElement("title"));
            var bookmarkIcon = bookmark.AppendChild(doc.CreateElement("icon"));

            bookmarkUrl.InnerText = bookmarkDetails.Url;
            bookmarkTitle.InnerText = bookmarkDetails.Name;
            bookmarkIcon.InnerText = "https://www.google.com/s2/favicons?sz=32&domain_url=" + bookmarkDetails.Url;

            SaveDoc(doc, BOOKMARKS_FILE_NAME);
        }

        public static async void LoadXmlFile(string configFileName)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(configFileName);
            await Launcher.LaunchFileAsync(file);
        }

        public async Task<string> GetCurrentTheme()
        {

            var name = string.Empty;
            var doc = await DocumentLoad(SETTINGS_FILE_NAME);
            var theme = doc.GetElementsByTagName("CurrentTheme");

            foreach (var item in theme)
            {
                name = item.Attributes[0].InnerText;
            }


            return name;
        }

        public async void SaveCurrentTheme(string themeName)
        {
            var doc = await DocumentLoad(SETTINGS_FILE_NAME);
            var theme = doc.GetElementsByTagName("CurrentTheme");

            foreach (var item in theme)
            {
                item.Attributes[0].InnerText = themeName;
            }

            SaveDoc(doc, SETTINGS_FILE_NAME);
        }
        public async Task<List<BookmarkDetails>> GetBookmarkList()
        {
            var doc = await DocumentLoad(BOOKMARKS_FILE_NAME);
            var bookmarks = doc.GetElementsByTagName("bookmark");
            return bookmarks.Select(item => item.ChildNodes)
                .Select(bookmark => new BookmarkDetails()
                {
                    Url = bookmark[0].InnerText,
                    Name = bookmark[1].InnerText,
                    Icon = bookmark[2].InnerText
                })
                .ToList();
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
                if (child[0].InnerText != bookmarkDetails.Url || child[1].InnerText != bookmarkDetails.Name) continue;
                found = item;
            }

            if (found == null) 
                return false;
            root.RemoveChild(found);
            SaveDoc(doc, BOOKMARKS_FILE_NAME);
            return true;
        }

        public async Task<bool> ClearHistoryFile()
        {
            try
            {
                var doc = await DocumentLoad(HISTORY_FILE_NAME);
                var root = doc.DocumentElement;
                var isSuccess = false;
                while (root.ChildNodes.Count > 0)
                    root.RemoveChild(root.ChildNodes[0]);

                if (root.ChildNodes.Count == 0)
                    isSuccess = true;

                SaveDoc(doc, HISTORY_FILE_NAME);
                return isSuccess;
            }
            catch
            {
                throw new Exception("Clear history file error");
            }
        }

        public async Task<bool> RemoveHistoryItem(string time)
        {
            var doc = await DocumentLoad(HISTORY_FILE_NAME);
            var root = doc.DocumentElement;
            var result = false;
            var history = doc.GetElementsByTagName("historyitem");
            for (var i = 0; i < history.Count; i++)
            {
                var child = history[i].ChildNodes;

                for (var j = 0; j < child.Count; j++)
                {
                    if (child[j].NodeName == "hour")
                    {
                        if (child[j].InnerText == time)
                        {
                            root.RemoveChild(history[i]);
                            result = true;
                        }
                    }

                }
            }
            SaveDoc(doc, HISTORY_FILE_NAME);
            return result;
        }

        public async void EditBookmark(string oldUrl, string newUrl, string newTitle)
        {
            var doc = await DocumentLoad(BOOKMARKS_FILE_NAME);
            var bookmark = doc.GetElementsByTagName("bookmark");
            for (var i = 0; i < bookmark.Count; i++)
            {
                var child = bookmark[i].ChildNodes;

                for (var j = 0; j < child.Count; j++)
                {
                    if (child[j].NodeName == "url")
                    {
                        if (child[j].InnerText == oldUrl)
                        {
                            try
                            {
                                var child0 = child[j]; //url                        
                                var child1 = child0.NextSibling; //title
                                var child2 = child1.NextSibling; //icon

                                child0.InnerText = newUrl;
                                child1.InnerText = newTitle;
                                child2.InnerText = "https://www.google.com/s2/favicons?domain=" + newUrl;
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                }
            }
            SaveDoc(doc, BOOKMARKS_FILE_NAME);
        }

        public async Task<List<SearchEngineItem>> GetSearchEngineList()
        {
            var engineList = new List<SearchEngineItem>();
            await Task.Run(async () =>
            {
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
                            Mode = child.Attributes.GetNamedItem("mode")?.InnerText,
                            HomePage = child.Attributes.GetNamedItem("homePage")?.InnerText
                        };
                        engineList.Add(engineItem);
                    }
                }
            });
            return engineList;
        }

        public async Task<SearchEngineItem> GetCurrentSearchEngine()
        {

            var current = new SearchEngineItem();
            await Task.Run(async () =>
            {
                var engines = await GetSearchEngineList();
                foreach (var engine in engines.Where(engine => engine.Mode == "1"))
                {
                    current = engine;
                }
            });
            return current;
        }

        public async Task<List<SiteItem>> GetStartPageElements()
        {
            var startPageElements = new List<SiteItem>();
            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(STARTPAGE_FILE_NAME);
                var doc = await XmlDocument.LoadFromFileAsync(file);

                var searchEngine = doc.GetElementsByTagName("elements");
                foreach (var t in searchEngine)
                {
                    var searchChild = t.ChildNodes;

                    foreach (var child in searchChild)
                    {
                        if (child.NodeName != "element") continue;
                        var item = new SiteItem
                        {
                            Name = child.Attributes.GetNamedItem("name")?.InnerText,
                            Url = child.Attributes.GetNamedItem("url")?.InnerText
                        };
                        startPageElements.Add(item);
                    }
                }
            });
            return startPageElements;
        }

        public async void AddNewSiteOnStartPage(SiteItem siteItem)
        {
            var doc = await DocumentLoad(STARTPAGE_FILE_NAME);

            var elements = doc.GetElementsByTagName("elements");
            var newElement = doc.CreateElement("element");
            newElement.SetAttribute("name", siteItem.Name);
            newElement.SetAttribute("url", siteItem.Url);

            elements[0].AppendChild(newElement);

            SaveDoc(doc, STARTPAGE_FILE_NAME);
        }

        public async void RemoveSiteOnStartPage(SiteItem siteItem)
        {
            var doc = await DocumentLoad(STARTPAGE_FILE_NAME);

            var elements = doc.GetElementsByTagName("elements");
            foreach (var elem in elements.ToList())
            {
                var searchChild = elem.ChildNodes;

                foreach (var child in searchChild.ToList())
                {
                    var attr = child.Attributes;
                    if (child.NodeName != "element") continue;

                    var name = attr.GetNamedItem("name")!.InnerText;
                    var url = attr.GetNamedItem("url")!.InnerText;
                    if (name != siteItem.Name || url != siteItem.Url) continue;
                    child.ParentNode.RemoveChild(child);
                }
            }

            SaveDoc(doc, STARTPAGE_FILE_NAME);
        }
        public async void ChangeSearchEngine([CanBeNull] string newEngine)
        {
            if (newEngine == null) return;
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
            SaveDoc(doc, SETTINGS_FILE_NAME);
        }
    }
}
