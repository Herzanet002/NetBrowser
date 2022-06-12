using System;
using System.IO;
using System.Xml;
using Windows.Storage;
using static NetBrowser_UWP.Constants.Constants;

namespace NetBrowser_UWP
{
    public static class DataAccess
    {
        public static async void CreateHistoryFile()
        {
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(HISTORY_FILE_NAME);
                using (var writeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var stream = writeStream.AsStreamForWrite();
                    var settings = new XmlWriterSettings
                    {
                        Async = true,
                        Indent = true
                    };

                    using var writer = XmlWriter.Create(stream, settings);
                    await writer.WriteStartDocumentAsync();
                    writer.WriteStartElement("history");
                    await writer.WriteEndElementAsync();
                    await writer.WriteEndDocumentAsync();
                    await writer.FlushAsync();
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
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(BOOKMARKS_FILE_NAME);
                using (var writeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var stream = writeStream.AsStreamForWrite();
                    var settings = new XmlWriterSettings
                    {
                        Async = true,
                        Indent = true
                    };

                    using var writer = XmlWriter.Create(stream, settings);
                    await writer.WriteStartDocumentAsync();
                    writer.WriteStartElement("bookmarks");
                    await writer.WriteEndDocumentAsync();
                    await writer.FlushAsync();
                    await writer.FlushAsync();
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
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(SETTINGS_FILE_NAME);
                using (var writeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var stream = writeStream.AsStreamForWrite();
                    var settings = new XmlWriterSettings
                    {
                        Async = true,
                        Indent = true
                    };

                    using var writer = XmlWriter.Create(stream, settings);
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
                    writer.WriteAttributeString("name", "Yandex");
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
                await Windows.System.Launcher.LaunchFileAsync(storageFile);
            }
            catch
            {
                // ignored
            }
        }

        public static async void CreateStartPageFile()
        {
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(STARTPAGE_FILE_NAME);
                using (var writeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var stream = writeStream.AsStreamForWrite();
                    var settings = new XmlWriterSettings
                    {
                        Async = true,
                        Indent = true
                    };

                    using var writer = XmlWriter.Create(stream, settings);
                    await writer.WriteStartDocumentAsync();
                    writer.WriteStartElement("startpage");
                    writer.WriteStartElement("elements");
                    writer.WriteStartElement("element");
                    writer.WriteAttributeString("name", "Yandex");
                    writer.WriteAttributeString("url", "https://yandex.com/");
                    await writer.WriteEndElementAsync();
                    writer.WriteStartElement("element");
                    writer.WriteAttributeString("name", "Google");
                    writer.WriteAttributeString("url", "https://google.com/");
                    await writer.WriteEndElementAsync();
                    writer.WriteStartElement("element");
                    writer.WriteAttributeString("name", "Bing");
                    writer.WriteAttributeString("url", "https://bing.com/");
                    await writer.WriteEndElementAsync();
                    writer.WriteStartElement("element");
                    writer.WriteAttributeString("name", "Gmail");
                    writer.WriteAttributeString("url", "https://gmail.com/");
                    await writer.WriteEndElementAsync();
                    await writer.WriteEndElementAsync();
                    await writer.WriteEndDocumentAsync();
                    await writer.FlushAsync();
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
