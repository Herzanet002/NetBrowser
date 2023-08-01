using System;
using LiteDB;

namespace NetBrowser_UWP.Models;

public class SiteItem
{
    [BsonId] public ObjectId Id { get; set; }

    public string Name { get; set; }

    public string Url { get; set; }
}