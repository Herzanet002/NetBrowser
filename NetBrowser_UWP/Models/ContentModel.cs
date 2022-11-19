using System.Xml.Serialization;

namespace NetBrowser_UWP.Models;

[XmlRoot(ElementName = nameof(ContentModel))]
public class ContentModel
{
    public string Title { get; set; }

    public string Link { get; set; }

    public string FeederImageLink { get; set; }

    public string Description { get; set; }

    public string Content { get; set; }

    public string PubDate { get; set; }

    public string ImageUrl { get; set; }

    public string Feeder { get; set; }
}