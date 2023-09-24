using System;

namespace NetBrowser.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PageMetadataAttribute : Attribute
{
    public bool PathIsVisible { get; }

    public PageMetadataAttribute(bool pathIsVisible)
    {
        PathIsVisible = pathIsVisible;
    }
}