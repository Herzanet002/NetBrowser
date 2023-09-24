using System;

namespace NetBrowser_UWP.Services.PageService;

/// <summary>
///     Represents information about a page.
///     If the page has an NavigationView element, then the parent page is the root page
/// </summary>
public class PageInfo
{
    public PageInfo(string path, Type type, Type? parentType = null)
    {
        Path = path;
        Type = type;
        ParentType = parentType;
    }

    public PageInfo(string path, Type type, bool pathIsVisible, Type? parentType = null)
    {
        Path = path;
        Type = type;
        PathIsVisible = pathIsVisible;
        ParentType = parentType;
    }

    public PageInfo()
    {
    }

    /// <summary>
    ///     Absolute path to the page
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    ///     Page type
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    ///     Parent page type
    ///     If this field is null, it means that this page is the root of all descendants
    /// </summary>
    public Type? ParentType { get; set; }

    /// <summary>
    ///     Displays whether the page address should be displayed in the findbox bar 
    /// </summary>
    public bool PathIsVisible { get; set; } = true;
}