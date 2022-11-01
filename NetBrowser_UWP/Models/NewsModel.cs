using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetBrowser_UWP.Models;
public class NewsModel
{
    [JsonPropertyName("status")]
    public string Status
    {
        get; set;
    }
    [JsonPropertyName("totalResults")]
    public int TotalResults
    {
        get;
        set;
    }
    [JsonPropertyName("results")]
    public List<ContentModel> Content
    {
        get;
        set;
    }


}
