using System;
using Microsoft.UI.Xaml.Controls;

namespace NetBrowser_UWP.Contracts.Services;

public interface ITabViewService
{
    void AddTabItem(TabViewItem item);

    bool ContainsTab(TabViewItem item);

    TabViewItem GetTabItemByFilter(Func<TabViewItem, bool> filter);

    int GetTabItemIndex(TabViewItem item);

    void ChangeSelectedTabItem(TabViewItem newItem);

    void RemoveTabItem(TabViewItem item);

    void ChangeTabItem(TabViewItem oldTabItem, TabViewItem newTabItem);

    void CloseTabItemRequested(TabViewItem tab);
}