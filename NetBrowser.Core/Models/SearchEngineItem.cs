using LiteDB;

namespace NetBrowser.Core.Models;

public class SearchEngineItem
{
    public ObjectId Id { get; set; }

    public string Prefix { get; set; }

    public string Name { get; set; }

    public bool IsSelected { get; set; }

    public string HomePage { get; set; }
}