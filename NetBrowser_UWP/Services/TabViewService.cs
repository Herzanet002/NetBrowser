using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using Microsoft.UI.Xaml.Controls;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Services.PageService;
using NetBrowser_UWP.Views;
using NetBrowser_UWP.Views.News;
using NetBrowser_UWP.Views.Settings;
using FontIconSource = Microsoft.UI.Xaml.Controls.FontIconSource;
using IconSource = Microsoft.UI.Xaml.Controls.IconSource;
using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;

namespace NetBrowser_UWP.Services;

public class TabViewService : ObservableRecipient, ITabViewService
{
    private readonly IWebView2Service _webView2Service;
    private TabViewItem _selectedTabItem;
    private WebView2 _selectedWebView;
    private ObservableCollection<TabViewItem> _tabViewItemsList;

    public event SelectionChangedEventHandler SelectionChangedHandler;

    public TabViewService(IWebView2Service webView2Service)
    {
        _webView2Service = webView2Service;
        ItemsCollection = new ObservableCollection<TabViewItem>();
    }

    public ObservableCollection<TabViewItem> ItemsCollection
    {
        get => _tabViewItemsList;
        set => SetProperty(ref _tabViewItemsList, value);
    }

    public TabViewItem SelectedTabItem
    {
        get => _selectedTabItem;
        set
        {
            SetProperty(ref _selectedTabItem, value);
            SelectionChangedHandler?.Invoke(this, null);
        }
    }

    public WebView2 SelectedWebView
    {
        get => _selectedWebView;
        set => SetProperty(ref _selectedWebView, value);
    }

    public void AddTabItem(TabViewItem item)
    {
        if (item is null) return;
        ItemsCollection.Add(item);
    }


    public TabViewItem GetTabItemByFilter(Func<TabViewItem, bool> filter)
    {
        return ItemsCollection.SingleOrDefault(filter);
    }

    public void ChangeSelectedTabItem(TabViewItem newItem)
    {
        if (ItemsCollection.Contains(newItem))
            SelectedTabItem = newItem;
        if (newItem is null)
            SelectedTabItem = null;
    }

    public void RemoveTabItem(TabViewItem item)
    {
        if (ItemsCollection.Contains(item))
            ItemsCollection.Remove(item);
    }

    public void ChangeTabItem(TabViewItem oldTabItem, TabViewItem newTabItem)
    {
        var index = ItemsCollection.IndexOf(oldTabItem);
        ItemsCollection[index] = newTabItem;
    }

    public void CloseTabItemRequested(TabViewItem tab)
    {
        if (tab.Content is WebView2 webContent)
            webContent.Close();

        RemoveTabItem(tab);
        if (!ItemsCollection.Any())
            SelectedWebView = null;
    }

    public TabViewItem CreateTabViewItemInstance<T>(string header, T content, IconSource icon)
    {
        var newTab = new TabViewItem
        {
            Header = string.IsNullOrWhiteSpace(header) ? "LoadingString".GetLocalized() : header,
            Content = content,
            IconSource = icon,
            IsRightTapEnabled = true
        };
        return newTab;
    }

    public async Task CreateNewWebTab(string url = null, bool isNavigated = true, bool isReplaced = false)
    {
        var newWebView = await _webView2Service.InstantiateWebView2(url);

        var newTab = CreateTabViewItemInstance(
            newWebView.CoreWebView2.DocumentTitle,
            newWebView,
            new SymbolIconSource { Symbol = Symbol.More });

        if (isReplaced)
            ChangeTabItem(SelectedTabItem, newTab);
        else
            AddTabItem(newTab);
        if (isNavigated) ChangeSelectedTabItem(newTab);
    }

    public void CreateStartPageTab()
    {
        var startPageTab = CreateTabViewItemInstance(
            "NewTab".GetLocalized(),
            new StartPage(),
            new FontIconSource
            {
                Glyph = "\xE737"
            });

        AddTabItem(startPageTab);
        ChangeSelectedTabItem(startPageTab);
    }

    public void CreateSettingsTab(Type innerPageType = default)
    {
        var settingsTabItem = GetTabItemByFilter(tab => tab.Content is SettingsPage);

        if (settingsTabItem != null)
        {
            ChangeSelectedTabItem(settingsTabItem);
            if (settingsTabItem.Content is SettingsPage page)
            {
                page.ViewModel.NavigateToPageType(innerPageType);
            }

            return;
        }

        var settingsTab = CreateTabViewItemInstance(
            "Settings".GetLocalized(),
            new SettingsPage(innerPageType ?? typeof(MainItemPageSettings)),
            new SymbolIconSource { Symbol = Symbol.Setting });

        AddTabItem(settingsTab);
        ChangeSelectedTabItem(settingsTab);
    }

    public void CreateNewsTab(Type innerPageType = default)
    {
        var newsTabItem = GetTabItemByFilter(tab => tab.Content is NewsShellPage);

        if (newsTabItem != null)
        {
            ChangeSelectedTabItem(newsTabItem);
            return;
        }

        var newsTab = CreateTabViewItemInstance(
            "News".GetLocalized(),
            new NewsShellPage(innerPageType ?? typeof(AllNewsPage)),
            new FontIconSource
            {
                Glyph = "\xE8A1"
            });

        AddTabItem(newsTab);
        ChangeSelectedTabItem(newsTab);
    }

    public void CreateTabByPageInfo(PageInfo pageInfo)
    {
        if (pageInfo.ParentType == typeof(StartPage))
        {
            CreateStartPageTab();
        }

        if (pageInfo.ParentType == typeof(SettingsPage))
        {
            CreateSettingsTab(pageInfo.Type);
        }

        if (pageInfo.ParentType == typeof(NewsShellPage))
        {
            CreateNewsTab(pageInfo.Type);
        }
    }
}