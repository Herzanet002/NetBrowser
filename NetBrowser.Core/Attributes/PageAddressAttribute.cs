using System;

namespace NetBrowser.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PageAddressAttribute : Attribute
{
    public string Address { get; }

    public PageAddressAttribute(string address)
    {
        Address = address;
    }
}