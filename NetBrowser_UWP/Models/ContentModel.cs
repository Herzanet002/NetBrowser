using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ServiceModel.Syndication;

namespace NetBrowser_UWP.Models;

public class ContentModel : BaseEntity
{
    public string Title { get; set; }

    public string Link { get; set; }

    public string Description { get; set; }

    public string Content { get; set; }

    public string PubDate { get; set; }

    public string ImageUrl { get; set; }

    public RssFeeder Feeder { get; set; }

    public bool IsFavorite { get; set; }

    [NotMapped] public ICollection<SyndicationCategory> Categories { get; set; }
}