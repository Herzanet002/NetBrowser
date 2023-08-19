#nullable enable
using System;
using LiteDB;

namespace NetBrowser_UWP.Models;

public class SearchTermItem
{
    public string? Query { get; set; }

    [BsonIgnore] public bool IsNewSuggestedSearchQuery { get; set; }

    public DateTime? LastTimeAccess { get; set; }

    public override string ToString()
        => Query!;
}