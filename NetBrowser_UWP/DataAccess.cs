using System;
using System.IO;
using System.Xml;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NetBrowser_UWP
{
    public class DataAccess
    {
        public async void CreateSettingsFile()
        {
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("settings.xml");
                using (IRandomAccessStream writeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Stream stream = writeStream.AsStreamForWrite();
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Async = true;
                    settings.Indent = true;

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("settings");
                        writer.WriteStartElement("history");
                        writer.WriteEndElement();
                        writer.WriteStartElement("bookmarks");
                        writer.WriteEndElement();
                        writer.WriteStartElement("searchEngine");
                        writer.WriteStartElement("google");
                        writer.WriteAttributeString("prefix", "https://google.com/search?q=");
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Flush();
                        await writer.FlushAsync();


                    }
                }
                await Windows.System.Launcher.LaunchFileAsync(storageFile);
            }
            catch
            {

            }
        }
    }
}
