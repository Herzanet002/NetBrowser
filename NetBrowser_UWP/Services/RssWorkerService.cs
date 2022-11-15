using NetBrowser_UWP.Models;
using System.Collections.Generic;
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

    public async Task<SyndicationFeed> GetSyndicationFeedAsync(string rssSource)
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

    public IEnumerable<ContentModel> GetFeeds(List<SyndicationFeed> syndicationFeeds)
    {
        foreach (var syndicationFeed in syndicationFeeds)
        {
            if (syndicationFeed is null) continue;
            foreach (var element in syndicationFeed.Items)
            {
                if (element is null || element.Links.Count != 2) continue;
                yield return new ContentModel
                {
                    Title = element.Title.Text,
                    Description = element.Summary.Text.Trim().Replace("\n", string.Empty),
                    PubDate = element.PublishDate.LocalDateTime.ToString("g"),
                    Link = element.Links[0].Uri.ToString(),
                    ImageUrl = element.Links[1].Uri.ToString(),
                    FeederImageLink = syndicationFeed.ImageUrl.ToString(),
                    Feeder = syndicationFeed.Title.Text
                };
            }
        }
    }
}

public interface IRssWorkerService
{
    Task<SyndicationFeed> GetSyndicationFeedAsync(string rssSource);
    abstract IEnumerable<ContentModel> GetFeeds(List<SyndicationFeed> syndicationFeeds);
}