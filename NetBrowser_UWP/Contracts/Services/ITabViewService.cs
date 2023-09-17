using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using NetBrowser_UWP.Services.PageService;

namespace NetBrowser_UWP.Contracts.Services;

public interface ITabViewService : INotifyPropertyChanged
{
    event SelectionChangedEventHandler SelectionChangedHandler;
    ObservableCollection<TabViewItem> ItemsCollection { get; set; }

    TabViewItem SelectedTabItem { get; set; }

    WebView2 SelectedWebView { get; set; }

    Task CreateNewWebTab(string url = null, bool isNavigated = true, bool isReplaced = false);

    void CreateNewsTab(Type innerPageType = default);

    void CreateSettingsTab(Type innerPageType = default);

    void CreateStartPageTab();

    void AddTabItem(TabViewItem item);

    TabViewItem GetTabItemByFilter(Func<TabViewItem, bool> filter);

    void ChangeSelectedTabItem(TabViewItem newItem);

    void RemoveTabItem(TabViewItem item);

    void ChangeTabItem(TabViewItem oldTabItem, TabViewItem newTabItem);

    void CloseTabItemRequested(TabViewItem tab);

    void CreateTabByPageInfo(PageInfo pageInfo);
}