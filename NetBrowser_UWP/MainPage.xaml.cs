using NetBrowser_UWP.Models;
using NetBrowser_UWP.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using NetBrowser_UWP.Properties;
using NetBrowser_UWP.Views.Settings;
using muxc = Microsoft.UI.Xaml.Controls;


namespace NetBrowser_UWP
{
    /// <summary>
    ///     Главная страница браузера, в котором отображается весь контент
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        #region PrivateGlobalElementRegion

        private static muxc.TabViewItem _currentSelectedTab;
        private static WebView _currentSelectedWeb;

        private static List<BookmarkDetails> _bookmarksList;
        private static IEnumerable<string> _searchTermList;
        private static List<HistoryItemDetails> _historyList;
        private static readonly Dictionary<object, bool> WebViewStates = new();

        private static string _appTitleText;
        private static string _searchBoxText;
        private static string _bookmarkTitleForSave;
        private static string _bookmarkUrlForSave;

        private static Visibility _visibilityProgressBar;
        private static Visibility _visibilityDeleteBookmarkButton;
        private static FontIcon _addbookmarkIcon;
        private static FontIcon _refreshButtonIcon;
        private static bool _isFlyoutClosed;
        #endregion
        public string SearchBoxText
        {
            get => _searchBoxText;
            set
            {
                _searchBoxText = value;
                OnPropertyChanged(nameof(SearchBoxText));
            }
        }
        public List<BookmarkDetails> BookmarksList
        {
            get => _bookmarksList;
            set
            {
                _bookmarksList = value;
                OnPropertyChanged(nameof(BookmarksList));
            }
        }
        public List<HistoryItemDetails> HistoryList
        {
            get => _historyList;
            set
            {
                _historyList = value;
                OnPropertyChanged(nameof(HistoryList));
            }
        }
        public string AppTitleText
        {
            get => _appTitleText;
            set
            {
                _appTitleText = value;
                OnPropertyChanged(nameof(AppTitleText));
            }
        }

        public string BookmarkTitleForSave
        {
            get => _bookmarkTitleForSave;
            set
            {
                _bookmarkTitleForSave = value;
                OnPropertyChanged(nameof(BookmarkTitleForSave));
            }
        }

        public string BookmarkUrlForSave
        {
            get => _bookmarkUrlForSave;
            set
            {
                _bookmarkUrlForSave = value;
                OnPropertyChanged(nameof(BookmarkUrlForSave));
            }
        }
        public FontIcon AddBookmarkIcon
        {
            get => _addbookmarkIcon;
            set
            {
                _addbookmarkIcon = value;
                OnPropertyChanged(nameof(AddBookmarkIcon));
            }
        }

        public FontIcon RefreshButtonIcon
        {
            get => _refreshButtonIcon;
            set
            {
                _refreshButtonIcon = value;
                OnPropertyChanged(nameof(RefreshButtonIcon));
            }
        }

        public Visibility ProgressBarVisibility
        {
            get => _visibilityProgressBar;
            set
            {
                _visibilityProgressBar = value;
                OnPropertyChanged(nameof(ProgressBarVisibility));
            }
        }

        public Visibility DeleteBookmarkButtonVisibility
        {
            get => _visibilityDeleteBookmarkButton;
            set
            {
                _visibilityDeleteBookmarkButton = value;
                OnPropertyChanged(nameof(DeleteBookmarkButtonVisibility));
            }
        }

        public bool IsFlyoutClosed
        {
            get => _isFlyoutClosed;
            set
            {
                _isFlyoutClosed = value;
                OnPropertyChanged(nameof(IsFlyoutClosed));
                if (value)
                    IsFlyoutClosed = false;
            }
        }

        public muxc.TabViewItem CurrentSelectedTab
        {
            get => _currentSelectedTab;
            set
            {
                _currentSelectedTab = value;
                OnPropertyChanged(nameof(CurrentSelectedTab));
                SelectionChangedTabHandler();
            }
        }

        /// <summary>
        /// INotifyPropertyChanged realization interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainPage()
        {
            InitializeComponent();
            GetBookmarks();
            ThemeManager.SetRequestedTheme();
            GetSearchTermList();
            CreateNewWebTab();
        }

        private static async void GetSearchTermList()
        {
            var searchTermListTransfer = await DataTransfer.GetSearchTerm();
            if (searchTermListTransfer == null) return;
            searchTermListTransfer.Reverse();
            _searchTermList = new HashSet<string>(searchTermListTransfer);

        }

        //Browser back button functionality
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSelectedWeb is { CanGoBack: true })
                _currentSelectedWeb.GoBack();
        }

        //Browser forward button functionality
        private void forwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSelectedWeb is { CanGoForward: true })
                _currentSelectedWeb.GoForward();
        }

        //Browser refresh button functionality
        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSelectedWeb == null || !WebViewStates.ContainsKey(_currentSelectedWeb)) return;
            if (RefreshButtonIcon == Constants.Constants.RefreshButtonIcon)
            {
                WebViewStates[_currentSelectedWeb] = true;
                _currentSelectedWeb.Refresh();
            }
            else
            {
                WebViewStates[_currentSelectedWeb] = false;
                _currentSelectedWeb.Stop();
            }
            SetVisualUiElementStates(_currentSelectedWeb);
        }

        //Browser home button functionality
        private void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentWebEngine?.HomePage != null && _currentSelectedWeb != null)
                NavigateTo(App.CurrentWebEngine.HomePage, _currentSelectedWeb);

        }
        private void AutoSuggestBox_GotFocus(object sender, RoutedEventArgs e) => GetSearchTermList();

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var queryForSearch = string.Empty;
            if (args.ChosenSuggestion != null)
                queryForSearch = args.ChosenSuggestion.ToString();

            else if (!string.IsNullOrEmpty(args.QueryText))
                queryForSearch = args.QueryText;

            if (_currentSelectedWeb == null)
            {
                CreateNewWebTab();
            }
            NavigateTo(queryForSearch, _currentSelectedWeb);
            DataTransfer.SaveSearchTerm(queryForSearch);


        }
        private static List<string> AutoSuggestListFill(string suggestBoxText)
        {
            var suitableItems = from item in _searchTermList
                                where item.ToLower().Contains(suggestBoxText.ToLower())
                                select item;

            var enumerableList = suitableItems.ToList();
            if (enumerableList.Count == 0)
            {
                enumerableList.Add("Искать в " + App.CurrentWebEngine.Name + " " + suggestBoxText);
            }

            if (suggestBoxText.Length != 0) return enumerableList;

            var recentlySearch = new List<string>();
            if (_searchTermList.ToList().Count < 10)
            {
                recentlySearch = _searchTermList.ToList();
            }
            else
            {
                recentlySearch.AddRange(_searchTermList.ToList().GetRange(0, 8));
            }

            suitableItems = recentlySearch;

            return suitableItems.ToList();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = AutoSuggestListFill(sender.Text);
            }
        }
        //Browser search button functionality
        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBoxText == null) return;
            DataTransfer.SaveSearchTerm(SearchBoxText);
            NavigateTo(SearchBoxText, _currentSelectedWeb);
        }

        private void SetProgressBarStatus(bool isEnabled)
        {
            ProgressBarVisibility = isEnabled ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetRefreshButtonIconState(bool isLoading)
        {
            RefreshButtonIcon = isLoading ? Constants.Constants.StopLoadButtonIcon : Constants.Constants.RefreshButtonIcon;
        }
        private void SetVisualUiElementStates(object sender)
        {

            if (sender == null || !WebViewStates.ContainsKey(sender))
            {
                SetProgressBarStatus(false);
                SetRefreshButtonIconState(false);
            }
            else
            {
                SetProgressBarStatus(WebViewStates[sender]);
                SetRefreshButtonIconState(WebViewStates[sender]);
            }
            SetBookmarkButtonAppearance();
        }

        private void SetVisualUiLabels(string appTitleText, string searchBoxText = null)
        {
            AppTitleText = Application.Current.Resources["AppName"] + " | " + appTitleText;
            if (searchBoxText != null)
            {
                SearchBoxText = searchBoxText;
            }

        }

        //Event handler for the webpage start loading event
        private void browser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (!WebViewStates.ContainsKey(sender)) return;
            WebViewStates[sender] = true;
            SetVisualUiElementStates(sender);
            SetVisualUiLabels("Загрузка...");

            //Добавить иконку загрузки на вкладку

        }

        //Event handler when the web page is loaded
        private void browser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            foreach (muxc.TabViewItem tabItem in TabsControl.TabItems)
            {
                if (sender == null || sender.Source == null || tabItem.Content != sender || !args.IsSuccess) continue;
                var icoUri = new Uri("https://www.google.com/s2/favicons?domain=" + sender.Source);
                tabItem.Header = sender.DocumentTitle;
                tabItem.IconSource = new muxc.BitmapIconSource
                {
                    UriSource = icoUri,
                    ShowAsMonochrome = false
                };
                if (!string.IsNullOrEmpty(SearchBoxText) && tabItem.Header.ToString() != "Параметры" &&
                    tabItem.Header.ToString() != "Новая вкладка")
                    DataTransfer.SaveHistory(new HistoryItemDetails
                    {
                        Name = sender.DocumentTitle,
                        Url = sender.Source.AbsoluteUri,
                        Time = DateTime.Now.ToLongTimeString(),
                        Date = DateTime.Now.ToShortDateString()
                    });
                        
                WebViewStates[sender] = false;
            }

            if (sender == null || sender.Source == null || _currentSelectedWeb != sender) return;
            SetVisualUiLabels(sender.DocumentTitle, sender.Source?.AbsoluteUri);
            SetVisualUiElementStates(sender);

        }

        //Event handler for opening a new page in a new tab
        private void browser_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            CreateNewWebTab(args.Uri.AbsoluteUri);
        }

        private WebView CreateWebViewInstance(string urlToNavigate)
        {
            var newWebViewInstance = new WebView(WebViewExecutionMode.SeparateProcess);
            WebViewStates.Add(newWebViewInstance, true);
            newWebViewInstance.NavigationCompleted += browser_NavigationCompleted;
            newWebViewInstance.NewWindowRequested += browser_NewWindowRequested;
            newWebViewInstance.NavigationStarting += browser_NavigationStarting;
            newWebViewInstance.ContainsFullScreenElementChanged += webView_ContainsFullScreenElementChanged;
            newWebViewInstance.Navigate(new Uri(urlToNavigate));
            return newWebViewInstance;
        }

        private static muxc.TabViewItem CreateTabViewItemInstance(string header, object content, muxc.IconSource icon, Style style)
        {
            var newTab = new muxc.TabViewItem
            {
                Header = string.IsNullOrEmpty(header) ? "Загрузка..." : header,
                Content = content,
                IconSource = icon,
                Style = style
            };
            return newTab;
        }
        public void NavigateTo(string address, WebView webViewInstance)
        {
            if (address.Contains("https://") || address.Contains("http://"))
                if (webViewInstance != null)
                {
                    webViewInstance.Source = new Uri(address);
                    return;

                }

            switch (address)
            {
                case "app://settings":
                    CreateSettingsTab(0);
                    break;
                case "app://newtab":
                    CreateStartPageTab();
                    break;
                default:
                    if (webViewInstance != null)
                        webViewInstance.Source = new Uri(App.CurrentWebEngine?.Prefix + address);
                    break;
            }
        }
        public void CreateNewWebTab(string url = null)
        {
            var newWebView = CreateWebViewInstance(string.IsNullOrEmpty(url) ? App.CurrentWebEngine.HomePage : url);
            var newTab = CreateTabViewItemInstance(
                newWebView.DocumentTitle,
                newWebView,
                new muxc.SymbolIconSource() { Symbol = Symbol.More },
                Application.Current.Resources["TabViewItemStyle"] as Style);

            TabsControl.TabItems.Add(newTab);
            CurrentSelectedTab = newTab;
        }
        public void CreateStartPageTab()
        {
            var startPageTab = new muxc.TabViewItem
            {
                Header = Application.Current.Resources["NewTabTitle"],
                IconSource = new muxc.SymbolIconSource { Symbol = Symbol.NewWindow },
                Style = Application.Current.Resources["TabViewItemStyle"] as Style,
                Content = new StartPage()
            };
            TabsControl.TabItems.Add(startPageTab);
            CurrentSelectedTab = startPageTab;
        }
        public void CreateSettingsTab(int mode)
        {
            var settingsTab = new muxc.TabViewItem
            {
                Header = Application.Current.Resources["SettingsText"],
                IconSource = new muxc.SymbolIconSource { Symbol = Symbol.Setting },
                Style = Application.Current.Resources["TabViewItemStyle"] as Style,
                Content = new SettingsPage(mode)
            };
            TabsControl.TabItems.Add(settingsTab);
            CurrentSelectedTab = settingsTab;

        }

        public void SearchWebFromStartPage(string url)
        {
            var newWebView = CreateWebViewInstance(url);

            var newTab = CreateTabViewItemInstance(
                newWebView.DocumentTitle,
                newWebView,
                new muxc.SymbolIconSource() { Symbol = Symbol.More },
                Application.Current.Resources["TabViewItemStyle"] as Style);

            var previousTab = CurrentSelectedTab;
            TabsControl.TabItems.Add(newTab);
            CurrentSelectedTab = newTab;
            TabsControl.TabItems.Remove(previousTab);

        }

        private void tabView_AddTabButtonClick(muxc.TabView sender, object args)
        {
            CreateStartPageTab();
        }
        private void SelectionChangedTabHandler()
        {
            if (CurrentSelectedTab is null) return;

            _currentSelectedWeb = CurrentSelectedTab.Content as WebView;
            if (_currentSelectedWeb != null && _currentSelectedWeb.Source != null)
            {
                SetVisualUiLabels(_currentSelectedWeb.DocumentTitle, _currentSelectedWeb.Source?.AbsoluteUri);
            }
            else switch (CurrentSelectedTab.Content)
            {
                case SettingsPage:
                    SetVisualUiLabels("Настройки", "app://settings"); //?????????
                    break;
                case StartPage:
                    SetVisualUiLabels("Новая вкладка", string.Empty);
                    break;
            }
            SetVisualUiElementStates(_currentSelectedWeb);
        }
        private void CloseWebViewItemRequested(ContentControl tab)
        {
            if (tab.Content is WebView webContent)
            {
                WebViewStates.Remove(webContent);
                webContent.Source = new Uri("about:blank");
                TabsControl.TabItems.Remove(tab);
                tab.Content = null;
            }

            if (CurrentSelectedTab != null)
            {
                TabsControl.TabItems.Remove(tab);
                if (CurrentSelectedTab == null)
                    SetVisualUiLabels(string.Empty, string.Empty);
            }

            else
            {
                SetVisualUiLabels(string.Empty, string.Empty);
            }
        }
        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            CreateStartPageTab();
        }

        private void tabView_TabCloseRequested(muxc.TabView sender, muxc.TabViewTabCloseRequestedEventArgs args)
        {
            CloseWebViewItemRequested(args.Tab);
        }

        private void CloseTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            if (args.Element is not muxc.TabView invokedTabView) return;
            if (!((muxc.TabViewItem)invokedTabView.SelectedItem).IsClosable) return;
            if (invokedTabView.TabItems[invokedTabView.SelectedIndex] is muxc.TabViewItem tabItem)
                CloseWebViewItemRequested(tabItem);
        }

        private static void webView_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            }
            else if (view.TryEnterFullScreenMode())
            {
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            }
        }


        private async void GetBookmarks()
        {
            var bookmarksListTransfer = await DataTransfer.GetBookmarkList();
            bookmarksListTransfer.Reverse();
            BookmarksList = bookmarksListTransfer;
        }

        private void SetBookmarkIconState(bool isAccessable)
        {
            if (isAccessable)
            {
                AddBookmarkIcon = Constants.Constants.ActiveIcon;
                DeleteBookmarkButtonVisibility = Visibility.Visible;
            }
            else
            {
                AddBookmarkIcon = Constants.Constants.UnactiveIcon;
                DeleteBookmarkButtonVisibility = Visibility.Collapsed;
            }
        }
        private void SetBookmarkButtonAppearance()
        {
            if (_currentSelectedWeb == null)
            {
                SetBookmarkIconState(false);
                return;
            }
            GetBookmarks();
            if (BookmarksList == null) return;
            var isExistsBookmark = false;
            BookmarksList.ForEach(bookmark =>
            {
                if (bookmark == null || _currentSelectedWeb.Source == null) return;
                if (bookmark.Url == _currentSelectedWeb.Source.AbsoluteUri)
                    isExistsBookmark = true;
            });

            SetBookmarkIconState(isExistsBookmark);
        }

        private void CancelBookmarksBtn_Click(object sender, RoutedEventArgs e)
        {
            IsFlyoutClosed = true;
        }

        private async void SaveBookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BookmarkTitleForSave != string.Empty && BookmarkUrlForSave != string.Empty &&
                Uri.IsWellFormedUriString(BookmarkUrlForSave, UriKind.Absolute))
            {
                DataTransfer.SaveBookmark(
                    new BookmarkDetails()
                    {
                        Name = BookmarkTitleForSave, 
                        Url = BookmarkUrlForSave
                    });
                IsFlyoutClosed = true;
                SetBookmarkIconState(true);
            }
            else
            {
                var dialogError = new ContentDialog
                {
                    Title = "Неверные данные",
                    Content = "Проверьте правильность адреса",
                    CloseButtonText = "Закрыть"
                };

                await dialogError.ShowAsync();
            }
        }

        private async void DeleteBookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await DataTransfer.RemoveBookmark(_currentSelectedWeb?.Source.AbsoluteUri);
            if (!result) return;
            SetBookmarkIconState(false);
            IsFlyoutClosed = true;
        }


        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateSettingsTab(0);
        }

        private async void historyBtn_Click(object sender, RoutedEventArgs e)
        {
            var historyListTransfer = await DataTransfer.GetHistory("url");
            var historyListCount = historyListTransfer.Count;
            historyListTransfer = historyListCount <= 100 ? historyListTransfer : historyListTransfer.GetRange(historyListCount - 100, 100);
            historyListTransfer.Reverse();
            HistoryList = historyListTransfer;
        }

        private async void historyListView_ItemClick(object sender, [NotNull] ItemClickEventArgs e)
        {
            var historyItem = e.ClickedItem as HistoryItemDetails;
            var url = historyItem?.Url;
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                CreateNewWebTab();
                if (url != null)
                    NavigateTo(url, _currentSelectedWeb);
                IsFlyoutClosed = true;
            }
            else
            {
                var dialogError = new ContentDialog
                {
                    Title = "Неверная ссылка",
                    Content = "Ссылка " + url + " недействительна или неверна",
                    CloseButtonText = "Закрыть"
                };

                await dialogError.ShowAsync();
            }

        }

        private void addBookmarksBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSelectedWeb == null) return;
            BookmarkTitleForSave = _currentSelectedWeb.DocumentTitle;
            BookmarkUrlForSave = _currentSelectedWeb.Source.AbsoluteUri;
        }

        private void bookmarksBtn_Click(object sender, RoutedEventArgs e)
        {
            GetBookmarks();
        }

        private void bookmarkSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateSettingsTab(3);
            IsFlyoutClosed = true;
        }

        private void bookmarksFlyoutListView_ItemClick(object sender, [NotNull] ItemClickEventArgs e)
        {
            var clickedItem = (BookmarkDetails)e.ClickedItem;
            CreateNewWebTab(clickedItem.Url);
            IsFlyoutClosed = true;
        }
        private void HistorySettingsButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSettingsTab(5);
            IsFlyoutClosed = true;
        }



    }

}