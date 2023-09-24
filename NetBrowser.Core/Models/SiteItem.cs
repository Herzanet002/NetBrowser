using LiteDB;

namespace NetBrowser.Core.Models;

public class SiteItem
{
    [BsonId] public ObjectId Id { get; set; }

    public string Name { get; set; }

    public string Url { get; set; }
}