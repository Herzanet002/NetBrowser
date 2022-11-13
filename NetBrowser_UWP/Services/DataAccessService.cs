using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.System;
using NetBrowser_UWP.Contracts.Services;
using static NetBrowser_UWP.Constants.Constants;

namespace NetBrowser_UWP.Services;

public class DataAccessService : IDataAccessService
{
    public async Task InitializeHistoryFile()
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

            await Launcher.LaunchFileAsync(storageFile);
        }
        catch
        {
            // ignored
        }
    }

    public async Task InitializeBookmarksFile()
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
            }

            await Launcher.LaunchFileAsync(storageFile);
        }
        catch
        {
            // ignored
        }
    }

    public async Task InitializeConfigFile()
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
                writer.WriteStartElement("CurrentTheme");
                writer.WriteAttributeString("Name", "Light");

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

            await Launcher.LaunchFileAsync(storageFile);
        }
        catch
        {
            // ignored
        }
    }

    public async Task InitializeStartPageFile()
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
                    Indent = false
                };


                using var writer = XmlWriter.Create(stream, settings);
                await writer.WriteStartDocumentAsync();
                writer.WriteStartElement("startPageElements");

                writer.WriteStartElement("element");

                writer.WriteElementString("name", "Yandex");
                writer.WriteElementString("url", "https://yandex.com/");
                await writer.WriteEndElementAsync();

                writer.WriteStartElement("element");
                writer.WriteElementString("name", "Google");
                writer.WriteElementString("url", "https://google.com/");
                await writer.WriteEndElementAsync();

                writer.WriteStartElement("element");
                writer.WriteElementString("name", "Bing");
                writer.WriteElementString("url", "https://bing.com/");
                await writer.WriteEndElementAsync();

                writer.WriteStartElement("element");
                writer.WriteElementString("name", "Gmail");
                writer.WriteElementString("url", "https://gmail.com/");
                await writer.WriteEndElementAsync();


                await writer.WriteEndDocumentAsync();
                await writer.FlushAsync();
            }

            await Launcher.LaunchFileAsync(storageFile);
        }
        catch
        {
            // ignored
        }
    }
}