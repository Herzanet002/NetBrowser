using System;

namespace NetBrowser_UWP.Contracts;

public interface INavigationViewContentPage
{
    Type InnerPageType { get; }
}