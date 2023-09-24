using System;
using System.Text.Json.Serialization;

namespace NetBrowser.Core.Models;

public class NewsProvider
{
    [JsonPropertyName("id")] public Guid Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; } = null!;

    [JsonPropertyName("link")] public string Link { get; set; } = null!;

    [JsonPropertyName("rssUrl")] public string RssUrl { get; set; } = null!;

    [JsonPropertyName("feederImageLink")] public string? ImageLink { get; set; }
}