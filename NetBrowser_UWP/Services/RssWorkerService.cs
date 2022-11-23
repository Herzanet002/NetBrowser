using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using SyndicationFeed = System.ServiceModel.Syndication.SyndicationFeed;

namespace NetBrowser_UWP.Services;

public class RssWorkerService : IRssWorkerService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RssWorkerService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private async Task<SyndicationFeed> GetSyndicationFeedAsync(RssFeeder rssFeeder)
    {
        var settings = new XmlReaderSettings { Async = true };
        try
        {
            var client = _httpClientFactory.CreateClient();
            var content = await client.GetStreamAsync(rssFeeder.ApiUrl);
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

    public async IAsyncEnumerable<ContentModel> GetFeeds(IEnumerable<RssFeeder> rssFeeders, IEnumerable<ContentModel> favoriteItems = null)
    {
        var syndicationFeeds = new List<SyndicationFeed>();
        await Task.Run(async () =>
        {
            foreach (var source in rssFeeders)
                syndicationFeeds.Add(await GetSyndicationFeedAsync(source));
        });

        foreach (var syndicationFeed in syndicationFeeds)
        {
            if (syndicationFeed is null) continue;
            foreach (var element in syndicationFeed.Items)
            {
                if (element is null) continue;
                var commonFavorite = favoriteItems?.FirstOrDefault(x => x.Link == element.Links[0].Uri.ToString());
                yield return new ContentModel
                {
                    Title = element.Title.Text,
                    Description = element.Summary.Text.StripHtml().TrimStart().Replace("\n", string.Empty).TrimEnd(),
                    PubDate = element.PublishDate.LocalDateTime.ToString("g"),
                    Link = element.Links[0].Uri.ToString(),
                    ImageUrl = element.Links.Count == 2 ? element.Links[1].Uri.ToString() : null,
                    Feeder = rssFeeders.FirstOrDefault(x => x.Link == syndicationFeed.Links[0].Uri ||
                                                            x.ApiUrl == syndicationFeed.Links[0].Uri.OriginalString),
                    Categories = element.Categories,
                    IsFavorite = commonFavorite is not null
                };
            }
        }
    }
}

public interface IRssWorkerService
{
    abstract IAsyncEnumerable<ContentModel> GetFeeds(IEnumerable<RssFeeder> rssFeeders,
        IEnumerable<ContentModel> favoriteItems = null);
}