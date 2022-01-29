using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.System;

namespace NetBrowser_UWP
{
    public class DataTransfer
    {
        static string fileName = "settings.xml";

        public async void SaveHistory(string title, string url)
        {
            try
            {
                var doc = await DocumentLoad().AsAsyncOperation(); //Load the Xml file

                var history = doc.GetElementsByTagName("history");

                XmlElement elSiteName = doc.CreateElement("siteName");
                XmlElement elUrl = doc.CreateElement("url");

                var historyItem = history[0].AppendChild(doc.CreateElement("historyitem"));

                historyItem.AppendChild(elSiteName);
                historyItem.AppendChild(elUrl);

                elSiteName.InnerText = title;
                elUrl.InnerText = url;

                SaveDoc(doc);
            }
            catch { };
        }

        public async Task<XmlDocument> DocumentLoad()
        {
            XmlDocument result = null;

            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);
                doc.Normalize();
                result = doc;
            });
            return result;
        }

        private async void SaveDoc(XmlDocument doc)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            await doc.SaveToFileAsync(file);
        }

        public async Task<List<string>> GetHistory(string Source)
        {
            List<string> list_of_history = new List<string>();
            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);
                
                var historyItem = doc.GetElementsByTagName("historyitem");
                for (int i = 0; i < historyItem.Count; i++)
                {
                    var historyItemChild = historyItem[i].ChildNodes;
                    for (int j = 0; j < historyItemChild.Count; j++)
                    {
                        if (historyItemChild[j].NodeName == Source)
                        {
                            list_of_history.Add(historyItemChild[j].InnerText);
                        }
                    }
                }

            });
            return list_of_history;

        }

        public async void SaveBookmark(string title, string url)
        {
            var doc = await DocumentLoad();
           
            var bookmarks = doc.GetElementsByTagName("bookmarks");

            var bookmark = bookmarks[0].AppendChild(doc.CreateElement("bookmark"));
            var bookmarkUrl = bookmark.AppendChild(doc.CreateElement("url"));
            var bookmarkTitle = bookmark.AppendChild(doc.CreateElement("title"));
            var bookmarkIcon = bookmark.AppendChild(doc.CreateElement("icon"));

            bookmarkUrl.InnerText = url;
            bookmarkTitle.InnerText = title;
            bookmarkIcon.InnerText = "https://www.google.com/s2/favicons?domain=" + url;

            SaveDoc(doc);
        }

        public async static void LoadXmlFile()
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            await Launcher.LaunchFileAsync(file);
        }

        public async Task<List<BookmarkDetails>> GetBookmarkList()
        {
            List<BookmarkDetails> list = new List<BookmarkDetails>();

            await Task.Run(async () =>
            {
                var doc = await DocumentLoad();
               
                var bookmark = doc.GetElementsByTagName("bookmark");
                for(int i=0; i < bookmark.Count; i++)
                {
                    var children = bookmark[i].ChildNodes;

                    string returnUrl = string.Empty;
                    string returnTitle = string.Empty;
                    string returnIcon = string.Empty;

                    if(bookmark[i].NodeName == "bookmark")
                    {
                        for(int j = 0; j<children.Count; j++)
                        {
                            if(children[j].NodeName == "url")
                                returnUrl = children[j].InnerText;
                            if(children[j].NodeName == "title")
                                returnTitle = children[j].InnerText;
                            if(children[j].NodeName == "icon")
                                returnIcon = children[j].InnerText;
                        }
                    }

                    if(returnUrl != string.Empty && returnTitle!= string.Empty)
                    {
                        list.Add(new BookmarkDetails { Title = returnTitle, Url = returnUrl, Icon=returnIcon});
                    }
                }
            });
            return list;
        }

        public async void RemoveBookmark(string url)
        {
            var doc = await DocumentLoad();
           
            var bookmark = doc.GetElementsByTagName("bookmark");
            for(int i = 0; i < bookmark.Count; i++)
            {
                var child = bookmark[i].ChildNodes;

                for(int j = 0; j < child.Count; j++)
                {
                    if(child[j].NodeName == "url")
                    {
                        if(child[j].InnerText == url)
                        {
                            child[j].ParentNode.ParentNode.RemoveChild(bookmark[i]);
                            
                            
                        }
                        
                    }
                }
            }
            SaveDoc(doc);
        }
        
        public async void EditBookmark(string oldUrl, string newUrl, string newTitle)
        {
            var doc = await DocumentLoad();
            
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
                            var child1 = child[j].NextSibling.NextSibling;//title
                            var child2 = child1.NextSibling.NextSibling;//icon
                            
                            
                            child1.InnerText = newTitle;
                            child2.InnerText = "https://www.google.com/s2/favicons?domain=" + newUrl;
                        }

                    }
                }
            }
            SaveDoc(doc);
        }
        
    }

    public class BookmarkDetails
    {
        public string Title { get; set; }
        public string Url { get; set; }

        public string Icon { get; set; }
    }
}
