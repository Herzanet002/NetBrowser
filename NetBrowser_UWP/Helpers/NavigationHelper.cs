using System;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NetBrowser_UWP.Helpers;

public class NavigationHelper
{
    public static readonly DependencyProperty NavigateToProperty =
        DependencyProperty.RegisterAttached("NavigateTo", typeof(Type), typeof(NavigationHelper),
            new PropertyMetadata(null));

    // This helper class allows to specify the page that will be shown when you click on a NavigationViewItem
    //
    // Usage in xaml:
    // <winui:NavigationViewItem x:Uid="Shell_Main" Icon="Document" helpers:NavHelper.NavigateTo="views:ShellPage" />
    //
    // Usage in code:
    // NavHelper.SetNavigateTo(navigationViewItem, typeof(ShellPage));
    public static Type GetNavigateTo(NavigationViewItem item)
    {
        return (Type)item.GetValue(NavigateToProperty);
    }

    public static void SetNavigateTo(NavigationViewItem item, Type value)
    {
        item.SetValue(NavigateToProperty, value);
    }
}