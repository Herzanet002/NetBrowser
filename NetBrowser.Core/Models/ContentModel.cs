using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetBrowser.Core.Models;

public class ContentModel
{
    public Guid Id { get; set; }

    [JsonPropertyName("feederId")] public Guid FeederId { get; set; }

    public string FeederName { get; set; }

    public bool IsFavorite { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("link")] public string? Link { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("pubDate")] public DateTime? PubDate { get; set; }

    [JsonPropertyName("imageUrl")] public string? ImageUrl { get; set; }

    [JsonPropertyName("categories")] public ICollection<string>? Categories { get; set; }
}