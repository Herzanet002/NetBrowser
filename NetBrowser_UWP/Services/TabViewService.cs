using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.Web.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using NetBrowser_UWP.Contracts.Services;
using Microsoft.Toolkit.Uwp;
using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;
using NetBrowser_UWP.Views;
using NetBrowser_UWP.Views.News;
using FontIconSource = Microsoft.UI.Xaml.Controls.FontIconSource;
using NetBrowser_UWP.Views.Settings;

namespace NetBrowser_UWP.Services
{

    public class TabViewService : ObservableObject, ITabViewService
    {
        private readonly IWebView2Service _webView2Service;
        private ObservableCollection<TabViewItem> _tabViewItemsList;
        private TabViewItem _selectedTabItem;

        public event EventHandler<SelectionChangedEventHandler> SelectionChangedHandler;

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

        private void E(object sender, SelectionChangedEventArgs e)
        {

        }

        private WebView2 _selectedWebView2;
        public WebView2 SelectedWebView2
        {
            get => _selectedWebView2;
            set => SetProperty(ref _selectedWebView2, value);
        }

        public TabViewService(IWebView2Service webView2Service)
        {
            _webView2Service = webView2Service;
            TabViewItemsList = new ObservableCollection<TabViewItem>();
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
        }

        public TabViewItem CreateTabViewItemInstance<T>(string header, T content, Microsoft.UI.Xaml.Controls.IconSource icon)
        {
            var newTab = new TabViewItem
            {
                Header = string.IsNullOrWhiteSpace(header) ? "LoadingString".GetLocalized() : header,
                Content = content,
                IconSource = icon,
                IsRightTapEnabled = true,
            };
            return newTab;
        }

        public async void CreateNewWebTab(string url = null)
        {
            var newWebView = await _webView2Service.InstantiateWebView2(string.IsNullOrWhiteSpace(url) ?
                App.CurrentWebEngine.HomePage :
                _webView2Service.ResolveUri(url).ToString());

            var newTab = CreateTabViewItemInstance(
                newWebView.CoreWebView2.DocumentTitle,
                newWebView,
                new SymbolIconSource { Symbol = Symbol.More });

            AddTabItem(newTab);
            ChangeSelectedTabItem(newTab);
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

        public void CreateSettingsTab(int mode = 0)
        {
            var alreadyExistsSettingsTab = GetTabItemByFilter(tab => tab.Content is SettingsPage);

            if (alreadyExistsSettingsTab != null)
            {
                ChangeSelectedTabItem(alreadyExistsSettingsTab);
                return;
            }

            var settingsTab = CreateTabViewItemInstance(
                "Settings".GetLocalized(),
                new SettingsPage(mode),
                new SymbolIconSource { Symbol = Symbol.Setting });

            AddTabItem(settingsTab);
            ChangeSelectedTabItem(settingsTab);
        }

        public void CreateNewContentTab()
        {
            var newsTab = CreateTabViewItemInstance(
                "News".GetLocalized(),
                new NewsPage(),
                new FontIconSource
                {
                    Glyph = "\xE8A1"
                });

            AddTabItem(newsTab);
            ChangeSelectedTabItem(newsTab);
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
    }


}
