using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;

namespace NetBrowser_UWP.Contracts.Services;

public interface ITabViewService 
{
    void AddTabItem(TabViewItem item);
    bool ContainsTab(TabViewItem item);
    TabViewItem GetTabItemByFilter(Func<TabViewItem, bool> filter);
    int GetTabItemIndex(TabViewItem item);
    void ChangeSelectedTabItem(TabViewItem newItem);
    TabViewItem GetSelectedTabItem();
    void RemoveTabItem(TabViewItem item);
    int GetTabItemsCount();
    ObservableCollection<TabViewItem> GetAllTabItems();
    WebView2 GetSelectedWebView();
    void ChangeSelectedWebView(WebView2 webView);
    void ChangeTabItem(TabViewItem oldTabItem, TabViewItem newTabItem);
}