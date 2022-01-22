using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
namespace NetBrowser_UWP
{
    public class DataTransfer
    {
        string fileName = "config.xml";

        public async void SaveHistory(string SearchTerm, string title, string url)
        {
            var doc = await DocumentLoad().AsAsyncOperation(); //Load the Xml file

            var history = doc.GetElementsByTagName("history");

            XmlElement elSearchTerm = doc.CreateElement("searchterm");
            XmlElement elSiteName = doc.CreateElement("siteName");
            XmlElement elUrl = doc.CreateElement("url");

            var historyItem = history[0].AppendChild(doc.CreateElement("historyitem"));

            historyItem.AppendChild(elSearchTerm);
            historyItem.AppendChild(elSiteName);
            historyItem.AppendChild(elUrl);

            elSearchTerm.InnerText = SearchTerm;
            elSiteName.InnerText = title;
            elUrl.InnerText = url;

            SaveDoc(doc);
        }

        public async Task<XmlDocument> DocumentLoad(){
            XmlDocument result = null;

            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);
                result = doc;
            });
            return result;
        }

        private async void SaveDoc(XmlDocument doc)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            await doc.SaveToFileAsync(file);
        }
    }
}
