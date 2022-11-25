using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using Microsoft.UI.Xaml.Controls;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Views;
using NetBrowser_UWP.Views.News;
using NetBrowser_UWP.Views.Settings;
using FontIconSource = Microsoft.UI.Xaml.Controls.FontIconSource;
using IconSource = Microsoft.UI.Xaml.Controls.IconSource;
using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;

namespace NetBrowser_UWP.Services;

public class TabViewService : ObservableObject, ITabViewService
{
    private readonly IWebView2Service _webView2Service;
    private TabViewItem _selectedTabItem;

    private WebView2 _selectedWebView2;
    private ObservableCollection<TabViewItem> _tabViewItemsList;

    public TabViewService(IWebView2Service webView2Service)
    {
        _webView2Service = webView2Service;
        TabViewItemsList = new ObservableCollection<TabViewItem>();
    }

    public ObservableCollection<TabViewItem> TabViewItemsList
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
            SelectionChangedHandler?.Invoke(_selectedTabItem, E);
        }
    }

    public WebView2 SelectedWebView2
    {
        get => _selectedWebView2;
        set => SetProperty(ref _selectedWebView2, value);
    }

    public void AddTabItem(TabViewItem item)
    {
        if (item is null) return;
        TabViewItemsList.Add(item);
    }

    public bool ContainsTab(TabViewItem item)
    {
        return TabViewItemsList.Contains(item);
    }

    public TabViewItem GetTabItemByFilter(Func<TabViewItem, bool> filter)
    {
        return TabViewItemsList.SingleOrDefault(filter);
    }

    public int GetTabItemIndex(TabViewItem item)
    {
        return TabViewItemsList.IndexOf(item);
    }

    public void ChangeSelectedTabItem(TabViewItem newItem)
    {
        if (TabViewItemsList.Contains(newItem))
            SelectedTabItem = newItem;
        if (newItem is null)
            SelectedTabItem = null;
    }

    public TabViewItem GetSelectedTabItem()
    {
        return SelectedTabItem;
    }

    public void RemoveTabItem(TabViewItem item)
    {
        if (TabViewItemsList.Contains(item))
            TabViewItemsList.Remove(item);
    }

    public int GetTabItemsCount()
    {
        return TabViewItemsList.Count();
    }

    public ObservableCollection<TabViewItem> GetAllTabItems()
    {
        return TabViewItemsList;
    }

    public WebView2 GetSelectedWebView()
    {
        return SelectedWebView2;
    }

    public void ChangeSelectedWebView(WebView2 webView)
    {
        SelectedWebView2 = webView;
    }

    public void ChangeTabItem(TabViewItem oldTabItem, TabViewItem newTabItem)
    {
        var index = TabViewItemsList.IndexOf(oldTabItem);
        TabViewItemsList[index] = newTabItem;
    }

    public void CloseTabItemRequested(TabViewItem tab)
    {
        if (tab.Content is WebView2 webContent)
            webContent.Close();

        RemoveTabItem(tab);
        if (GetTabItemsCount() == 0)
            ChangeSelectedWebView(null);
    }

    public event EventHandler<SelectionChangedEventHandler> SelectionChangedHandler;

    private void E(object sender, SelectionChangedEventArgs e)
    {
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
            new SymbolIconSource {Symbol = Symbol.More});

        if (isReplaced)
            ChangeTabItem(GetSelectedTabItem(), newTab);
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

    public void CreateSettingsTab(Type pageType = default)
    {
        var alreadyExistsSettingsTab = GetTabItemByFilter(tab => tab.Content is SettingsPage);

        if (alreadyExistsSettingsTab != null)
        {
            ChangeSelectedTabItem(alreadyExistsSettingsTab);
            return;
        }

        var settingsTab = CreateTabViewItemInstance(
            "Settings".GetLocalized(),
            new SettingsPage(pageType ?? typeof(MainItemPageSettings)),
            new SymbolIconSource {Symbol = Symbol.Setting});

        AddTabItem(settingsTab);
        ChangeSelectedTabItem(settingsTab);
    }

    public void CreateNewsTab(Type pageType = default)
    {
        var alreadyExistsContentTab = GetTabItemByFilter(tab => tab.Content is NewsShellPage);

        if (alreadyExistsContentTab != null)
        {
            ChangeSelectedTabItem(alreadyExistsContentTab);
            return;
        }

        var newsTab = CreateTabViewItemInstance(
            "News".GetLocalized(),
            new NewsShellPage(pageType ?? typeof(AllNewsPage)),
            new FontIconSource
            {
                Glyph = "\xE8A1"
            });

        AddTabItem(newsTab);
        ChangeSelectedTabItem(newsTab);
    }
}