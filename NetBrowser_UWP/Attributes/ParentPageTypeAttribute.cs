using System;

namespace NetBrowser_UWP.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ParentPageTypeAttribute : Attribute
{
    public Type ParentPageType { get; }

    public ParentPageTypeAttribute(Type parentPageType)
    {
        ParentPageType = parentPageType;
    }
}