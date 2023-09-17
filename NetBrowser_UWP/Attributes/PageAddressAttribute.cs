using System;

namespace NetBrowser_UWP.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PageAddressAttribute : Attribute
{
    public string Address { get; }

    public PageAddressAttribute(string address)
    {
        Address = address;
    }
}