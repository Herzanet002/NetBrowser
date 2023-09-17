using System;

namespace NetBrowser.Utils;

public record RssFeeder
{
    //[BsonId] public ObjectId Id { get; set; }

    public string Name { get; set; } = null!;

    public Uri Link { get; set; } = null!;

    public string RssUrl { get; set; } = null!;

    public string? FeederImageLink { get; set; }

    public bool IsCategorized { get; set; }

    public bool RecommendedCategory { get; set; }
}