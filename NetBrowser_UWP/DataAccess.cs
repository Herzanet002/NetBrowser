using System;
using System.IO;
using System.Xml;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NetBrowser_UWP
{
    public class DataAccess
    {
        public static async void CreateHistoryFile()
        {
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("history.xml");
                using (IRandomAccessStream writeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Stream stream = writeStream.AsStreamForWrite();
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Async = true,
                        Indent = true
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        await writer.WriteStartDocumentAsync();
                        writer.WriteStartElement("history");
                        await writer.WriteEndElementAsync();
                        writer.WriteStartElement("bookmarks");
                        await writer.WriteEndElementAsync();

                        await writer.WriteEndDocumentAsync();
                        await writer.FlushAsync();
                        await writer.FlushAsync();


                    }
                }
                await Windows.System.Launcher.LaunchFileAsync(storageFile);
            }
            catch
            {
                // ignored
            }
        }

        public static async void CreateBookmarksFile()
        {
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("bookmarks.xml");
                using (IRandomAccessStream writeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Stream stream = writeStream.AsStreamForWrite();
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Async = true,
                        Indent = true
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        await writer.WriteStartDocumentAsync();
                        writer.WriteStartElement("bookmarks");
                        await writer.WriteEndDocumentAsync();
                        await writer.FlushAsync();
                        await writer.FlushAsync();


                    }
                }
                await Windows.System.Launcher.LaunchFileAsync(storageFile);
            }
            catch
            {
                // ignored
            }
        }
        public static async void CreateConfigFile()
        {
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("configs.xml");
                using (IRandomAccessStream writeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Stream stream = writeStream.AsStreamForWrite();
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Async = true,
                        Indent = true
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        await writer.WriteStartDocumentAsync();
                        writer.WriteStartElement("Configurations");
                        writer.WriteStartElement("CurrentThemeMode");
                        writer.WriteAttributeString("Mode", "1");
                        await writer.WriteEndElementAsync();
                        writer.WriteStartElement("searchEngine");
                        writer.WriteStartElement("engine");
                        writer.WriteAttributeString("prefix", "https://google.com/search?q=");
                        writer.WriteAttributeString("name", "Google");
                        writer.WriteAttributeString("mode", "1");
                        writer.WriteAttributeString("homePage", "https://www.google.ru/");
                        await writer.WriteEndElementAsync();
                        writer.WriteStartElement("engine");
                        writer.WriteAttributeString("prefix", "https://yandex.ru/search/?text=");
                        writer.WriteAttributeString("name", "Яндекс");
                        writer.WriteAttributeString("mode", "0");
                        writer.WriteAttributeString("homePage", "https://yandex.ru/");
                        await writer.WriteEndElementAsync();
                        writer.WriteStartElement("engine");
                        writer.WriteAttributeString("prefix", "https://bing.com/search?q=");
                        writer.WriteAttributeString("name", "Bing");
                        writer.WriteAttributeString("mode", "0");
                        writer.WriteAttributeString("homePage", "https://www.bing.ru/");
                        await writer.WriteEndElementAsync();
                        await writer.WriteEndElementAsync();
                        await writer.WriteEndDocumentAsync();
                        await writer.FlushAsync();


                    }
                }
                await Windows.System.Launcher.LaunchFileAsync(storageFile);
            }
            catch
            {
                // ignored
            }
        }
    }
}
