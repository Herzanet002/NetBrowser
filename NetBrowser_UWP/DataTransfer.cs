using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.System;
using NetBrowser_UWP.Annotations;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP
{
    public class DataTransfer
    {
        private const string SettingsFileName = "configs.xml";
        private const string BookmarksFileName = "bookmarks.xml";
        private const string HistoryFileName = "history.xml";

        private DataTransfer()
        {

        }
        private static DataTransfer _instance = null;
        private static readonly object Threadlock = new object();

        public static DataTransfer Source
        {
            get
            {

                lock (Threadlock)
                {
                    return _instance ?? (_instance = new DataTransfer());
                }
            }
        }
        public static async void SaveHistory(string title, string url, string hour, string date)
        {
            try
            {
                var doc = await DocumentLoad(HistoryFileName).AsAsyncOperation(); 

                var history = doc.GetElementsByTagName("history");

                XmlElement siteElement = doc.CreateElement("siteName");
                XmlElement siteUrl = doc.CreateElement("url");
                XmlElement timeElement = doc.CreateElement("hour");
                XmlElement dateElement = doc.CreateElement("date");

                var historyItem = history[0].AppendChild(doc.CreateElement("historyitem"));

                historyItem.AppendChild(siteElement);
                historyItem.AppendChild(siteUrl);
                historyItem.AppendChild(timeElement);
                historyItem.AppendChild(dateElement);

                siteElement.InnerText = title;
                siteUrl.InnerText = url;
                timeElement.InnerText = hour;
                dateElement.InnerText = date;

                SaveDoc(doc, HistoryFileName);
            }
            catch
            {
                // ignored
            }
        }

        public static async void SaveSearchTerm(string title)
        {
            try
            {
                if (title == string.Empty) return;
                var doc = await DocumentLoad(HistoryFileName).AsAsyncOperation();

                var history = doc.GetElementsByTagName("history");

                XmlElement elSiteName = doc.CreateElement("termName");

                var historyItem = history[0].AppendChild(doc.CreateElement("searchTerm"));

                historyItem.AppendChild(elSiteName);

                elSiteName.InnerText = title;

                SaveDoc(doc, HistoryFileName);

            }
            catch
            {
                // ignored
            }
        }

        public static async Task<XmlDocument> DocumentLoad(string configFileName)
        {
            XmlDocument result = null;

            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(configFileName);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);
                doc.Normalize();
                result = doc;
            });
            return result;
        }



        private static async void SaveDoc(XmlDocument doc, string configFileName)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(configFileName);
            await doc.SaveToFileAsync(file);
        }

        public static async Task<List<HistoryItemDetails>> GetHistory(string source)
        {
            List<HistoryItemDetails> listOfHistory = new List<HistoryItemDetails>();
            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(HistoryFileName);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);

                var historyItem = doc.GetElementsByTagName("historyitem");
                foreach (var item in historyItem)
                {
                    var historyItemChild = item.ChildNodes;
                    foreach (var child in historyItemChild)
                    {
                        if (child.NodeName == source)
                        {

                            listOfHistory.Add(new HistoryItemDetails
                            {
                                Url = child.InnerText,
                                Title = child.PreviousSibling.InnerText,
                                Time = child.NextSibling.InnerText,
                                Date = child.NextSibling.NextSibling.InnerText
                            });
                        }
                    }
                }

            });
            return listOfHistory;
        }

        public static async Task<List<string>> GetSearchTerm()
        {
            var listOfTerms = new List<string>();
            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(HistoryFileName);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);

                var historyItem = doc.GetElementsByTagName("searchTerm");
                listOfTerms.AddRange(from item in historyItem from child in item.ChildNodes where
                    child.NodeName == "termName" select child.InnerText);
            });
            return listOfTerms;
        }

        public static async void SaveBookmark(string title, string url)
        {
            var doc = await DocumentLoad(BookmarksFileName);

            var bookmarks = doc.GetElementsByTagName("bookmarks");

            var bookmark = bookmarks[0].AppendChild(doc.CreateElement("bookmark"));
            var bookmarkUrl = bookmark.AppendChild(doc.CreateElement("url"));
            var bookmarkTitle = bookmark.AppendChild(doc.CreateElement("title"));
            var bookmarkIcon = bookmark.AppendChild(doc.CreateElement("icon"));

            bookmarkUrl.InnerText = url;
            bookmarkTitle.InnerText = title;
            bookmarkIcon.InnerText = "https://www.google.com/s2/favicons?domain=" + url;

            SaveDoc(doc, BookmarksFileName);
        }

        public static async void LoadXmlFile(string configFileName)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(configFileName);
            await Launcher.LaunchFileAsync(file);
        }

        public static async Task<int> GetCurrentTheme()
        {
            int mode = 1;
            await Task.Run(async () =>
            {
                var doc = await DocumentLoad(SettingsFileName);
                var theme = doc.GetElementsByTagName("CurrentThemeMode");

                foreach (var item in theme)
                {
                    mode = Convert.ToInt32(item.Attributes[0].InnerText);
                }

            });
            return mode;
        }

        public static async void SaveCurrentTheme(string mode)
        {
            var doc = await DocumentLoad(SettingsFileName);
            var theme = doc.GetElementsByTagName("CurrentThemeMode");

            foreach (var item in theme)
            {
                item.Attributes[0].InnerText = mode;
            }

            SaveDoc(doc, SettingsFileName);
        }
        public static async Task<List<BookmarkDetails>> GetBookmarkList()
        {
            var list = new List<BookmarkDetails>();

            await Task.Run(async () =>
            {
                var doc = await DocumentLoad(BookmarksFileName);

                var bookmark = doc.GetElementsByTagName("bookmark");
                for (int i = 0; i < bookmark.Count; i++)
                {
                    var children = bookmark[i].ChildNodes;

                    string returnUrl = string.Empty;
                    string returnTitle = string.Empty;
                    string returnIcon = string.Empty;

                    if (bookmark[i].NodeName == "bookmark")
                    {
                        for (int j = 0; j < children.Count; j++)
                        {
                            if (children[j].NodeName == "url")
                                returnUrl = children[j].InnerText;
                            if (children[j].NodeName == "title")
                                returnTitle = children[j].InnerText;
                            if (children[j].NodeName == "icon")
                                returnIcon = children[j].InnerText;
                        }
                    }

                    if (returnUrl != string.Empty && returnTitle != string.Empty)
                    {
                        list.Add(new BookmarkDetails { Title = returnTitle, Url = returnUrl, Icon = returnIcon });
                    }
                }
            });
            return list;
        }

        public static async Task<bool> RemoveBookmark(string url)
        {
            var doc = await DocumentLoad(BookmarksFileName);
            var result = false;
            var bookmark = doc.GetElementsByTagName("bookmark");
            for (int i = 0; i < bookmark.Count; i++)
            {
                var child = bookmark[i].ChildNodes;

                for (int j = 0; j < child.Count; j++)
                {
                    if (child[j].NodeName == "url")
                    {
                        if (child[j].InnerText == url)
                        {
                            child[j].ParentNode.ParentNode.RemoveChild(bookmark[i]);
                            result = true;
                        }
                    }
                }
            }
            SaveDoc(doc, BookmarksFileName);
            return result;
        }

        public static async Task<bool> ClearHistoryFile()
        {
            try
            {
                var doc = await DocumentLoad(HistoryFileName);
                var root = doc.DocumentElement;
                var isSuccess = false;
                while (root.ChildNodes.Count > 0)
                    root.RemoveChild(root.ChildNodes[0]);

                if (root.ChildNodes.Count == 0)
                    isSuccess = true;
                
                SaveDoc(doc, HistoryFileName);
                return isSuccess;
            }
            catch
            {
                throw new Exception("Clear history file error");
            }
        }

        public static async Task<bool> RemoveHistoryItem(string time)
        {
            var doc = await DocumentLoad(HistoryFileName);
            var root = doc.DocumentElement;
            var result = false;
            var history = doc.GetElementsByTagName("historyitem"); 
            for (int i = 0; i < history.Count; i++)
            {
                var child = history[i].ChildNodes;

                for (int j = 0; j < child.Count; j++)
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
            SaveDoc(doc, HistoryFileName);
            return result;
        }
        public static async void EditBookmark(string oldUrl, string newUrl, string newTitle)
        {
            var doc = await DocumentLoad(BookmarksFileName);
            var bookmark = doc.GetElementsByTagName("bookmark");
            for (int i = 0; i < bookmark.Count; i++)
            {
                var child = bookmark[i].ChildNodes;

                for (int j = 0; j < child.Count; j++)
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
            SaveDoc(doc, BookmarksFileName);
        }

        public static async Task<List<SearchEngineItem>> GetSearchEngineList()
        {
            List<SearchEngineItem> engineList = new List<SearchEngineItem>();
            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SettingsFileName);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);

                var searchEngine = doc.GetElementsByTagName("searchEngine");
                foreach (var t in searchEngine)
                {
                    var searchChild = t.ChildNodes;

                    foreach (var child in searchChild)
                    {
                        if (child.NodeName != "engine") continue;
                        SearchEngineItem engineItem = new SearchEngineItem
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

        public static async Task<SearchEngineItem> GetCurrentEngine()
        {

            SearchEngineItem current = new SearchEngineItem();
            await Task.Run(async () =>
            {
                var engines = await GetSearchEngineList();
                foreach (var engine in engines)
                {
                    if (engine.Mode == "1")
                        current = engine;
                }
            });
            return current;


        }

        public static async void ChangeSearchEngine([CanBeNull] string newEngine)
        {
            if (newEngine == null) return;
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SettingsFileName);
            XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);

            var searchEngine = doc.GetElementsByTagName("searchEngine");
            foreach (var engine in searchEngine)
            {
                var searchChild = engine.ChildNodes;

                foreach (var child in searchChild)
                {
                    var attr = child.Attributes;
                    if (child.NodeName != "engine") continue;

                    attr.GetNamedItem("mode").InnerText = attr.GetNamedItem("name")?.InnerText == newEngine ? "1" : "0";
                }
            }
            SaveDoc(doc, SettingsFileName);
        }
    }
}
