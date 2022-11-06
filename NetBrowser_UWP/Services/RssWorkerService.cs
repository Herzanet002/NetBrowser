using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace NetBrowser_UWP.Services;

public class RssWorkerService : IRssWorkerService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RssWorkerService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<SyndicationFeed> ParseRss(string rssSource)
    {
        var settings = new XmlReaderSettings { Async = true };
        try
        {
            var client = _httpClientFactory.CreateClient();
            var content = await client.GetStreamAsync(rssSource);
            using var reader = XmlReader.Create(content, settings);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();
            return feed;
        }
        catch
        {
            return null;
        }
    }
}

public interface IRssWorkerService
{
    Task<SyndicationFeed> ParseRss(string rssSource);
}