using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace NetBrowser.Utils;

public class RssWorkerService : IRssWorkerService
{
    public async IAsyncEnumerable<ContentModel> GetFeeds(IEnumerable<RssFeeder> rssFeeders,
        IEnumerable<ContentModel> favoriteItems, int? limit)
    {
        var syndicationFeeds = new List<SyndicationFeed?>();
        await Task.Run(async () =>
        {
            foreach (var source in rssFeeders)
                syndicationFeeds.Add(await GetSyndicationFeedAsync(source));
        });

        foreach (var syndicationFeed in syndicationFeeds)
        {
            if (syndicationFeed is null) continue;
            var syndicationItems =
                limit == null ? syndicationFeed.Items : syndicationFeed.Items.Take(limit ?? 0);

            foreach (var element in syndicationItems)
            {
                if (element is null) continue;
                var commonFavorite = favoriteItems?.FirstOrDefault(x => x.Link == element.Links[0].Uri.ToString());
                yield return new ContentModel
                {
                    Title = element.Title.Text,
                    Description = element.Summary?.Text.StripHtml().TrimStart().Replace("\n", string.Empty).TrimEnd(),
                    PubDate = element.PublishDate.LocalDateTime.ToString("g"),
                    Link = element.Links[0].Uri.ToString(),
                    ImageUrl = element.Links.Count == 2 ? element.Links[1].Uri.ToString() : null,
                    Feeder = rssFeeders.FirstOrDefault(x => x.Link == syndicationFeed.Links[0].Uri ||
                                                            x.RssUrl == syndicationFeed.Links[0].Uri.OriginalString),
                    Categories = element.Categories,
                    IsFavorite = commonFavorite is not null
                };
            }
        }
    }

    private static async Task<SyndicationFeed?> GetSyndicationFeedAsync(RssFeeder rssFeeder)
    {
        try
        {
            var client = new HttpClient {Timeout = TimeSpan.FromMilliseconds(500)};
            using var content = await client.GetStreamAsync(rssFeeder.RssUrl);
            var settings = new XmlReaderSettings
            {
                Async = true
            };

            using var reader = XmlReader.Create(content, settings);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();
            return feed;
        }
        catch (Exception)
        {
            return null;
        }
    }
}