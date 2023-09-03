using System;
using Windows.UI.Xaml.Controls;

namespace NetBrowser_UWP.Views;

public abstract class NavigationViewContentPage : Page
{
    public abstract Type GetInnerPageType();
}