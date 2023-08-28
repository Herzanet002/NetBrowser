using System;

namespace NetBrowser_UWP.Services.PageService;

public class PageInfo
{
    public PageInfo(string path, Type type, Type? parentType = null)
    {
        Path = path;
        Type = type;
        ParentType = parentType;
    }

    public PageInfo()
    {
    }

    public string Path { get; set; }

    public Type Type { get; set; }

    public Type? ParentType { get; set; }
}