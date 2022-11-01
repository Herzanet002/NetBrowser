using System.Text.Json.Serialization;

namespace NetBrowser_UWP.Models;
public class ContentModel
{
    [JsonPropertyName("title")]
    public string Title
    {
        get; set;
    }
    [JsonPropertyName("link")]
    public string Link
    {
        get; set;
    }
    [JsonPropertyName("description")]
    public string Description
    {
        get; set;
    }
    [JsonPropertyName("content")]
    public string Content
    {
        get; set;
    }
    [JsonPropertyName("pubDate")]
    public string PubDate
    {
        get;
        set;
    }
    [JsonPropertyName("image_url")]
    public string ImageUrl
    {
        get;
        set;
    }
}
