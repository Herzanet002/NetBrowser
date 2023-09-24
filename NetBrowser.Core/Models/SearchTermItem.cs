using System;
using LiteDB;

namespace NetBrowser.Core.Models;

public class SearchTermItem
{
    public string? Query { get; set; }

    [BsonIgnore] public bool IsNewSuggestedSearchQuery { get; set; }

    public DateTime? LastTimeAccess { get; set; }

    public override string ToString()
        => Query!;
}