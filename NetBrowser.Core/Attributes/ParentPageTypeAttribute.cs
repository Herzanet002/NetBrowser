using System;

namespace NetBrowser.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ParentPageTypeAttribute : Attribute
{
    public Type ParentPageType { get; }

    public ParentPageTypeAttribute(Type parentPageType)
    {
        ParentPageType = parentPageType;
    }
}