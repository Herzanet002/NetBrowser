using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace NetBrowser.Utils;

public record ContentModel
{ 
    public int Id { get; set; }
    
    public string? Title { get; set; }

    public string? Link { get; set; }

    public string? Description { get; set; }

    public string? Content { get; set; }

    public string? PubDate { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsFavorite { get; set; } 
    
    public RssFeeder Feeder { get; set; } = null!;
    
    public ICollection<SyndicationCategory> Categories { get; set; } = null!;
}