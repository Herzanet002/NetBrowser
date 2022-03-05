using NetBrowser_UWP.Annotations;
using NetBrowser_UWP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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

        private static muxc.TabViewItem _settingsTab;
        private static List<BookmarkDetails> _bookmarksList;
        private static IEnumerable<string> _searchTermList;
        private static List<HistoryItemDetails> _historyList;

        private static string _appTitleText;
        private static string _searchBoxText;
        private static string _bookmarkTitleForSave;
        private static string _bookmarkUrlForSave;

        private static Visibility _visibilityProgressBar;
        private static Visibility _visibilityDeleteBookmarkButton;
        private static FontIcon _addbookmarkIcon;
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
            get { return _isFlyoutClosed; }
            set
            {
                _isFlyoutClosed = value;
                OnPropertyChanged(nameof(IsFlyoutClosed));
                if (value)
                    IsFlyoutClosed = false;
            }
        }
        public MainPage()
        {
            InitializeComponent();
            GetBookmarks();
            ThemeManager.SetRequestedTheme();
            SetCurrentEngine();
            DataContext = this;
            GetSearchTermList();
            CreateNewWebTab();

        }

        public async void GetSearchTermList()
        {
            _searchTermList = await DataTransfer.GetSearchTerm();
            _searchTermList = new HashSet<string>(_searchTermList.Reverse());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName][NotNull] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //Setting an address in SearchBox Control
        private void SetSearchBoxStatus([NotNull] string message)
        {
            SearchBoxText = message;
        }

        public static async void SetCurrentEngine()
        {
            App.CurrentWebEngine = await DataTransfer.GetCurrentEngine();
        }

        //Browser back button functionality
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSelectedWeb.CanGoBack)
                _currentSelectedWeb.GoBack();
        }

        //Browser forward button functionality
        private void forwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSelectedWeb.CanGoForward)
                _currentSelectedWeb.GoForward();
        }

        //Browser refresh button functionality
        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentSelectedWeb.Refresh();
        }

        //Browser home button functionality
        private void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentSelectedWeb.Navigate(new Uri(App.CurrentWebEngine.homePage));
        }

        //Search in the browser using the enter key
        private void AutoSuggestBox_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSearchTermList();
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var queryForSearch = string.Empty;

            if (args.ChosenSuggestion != null)
                queryForSearch = args.ChosenSuggestion.ToString();

            else if (!string.IsNullOrEmpty(args.QueryText))
                queryForSearch = args.QueryText;

            SearchWeb(queryForSearch);
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
            SearchWeb(SearchBoxText);
        }

        private void SetProgressBarStatus(bool isEnabled)
        {
            ProgressBarVisibility = isEnabled ? Visibility.Visible : Visibility.Collapsed;
        }

        //Event handler for the webpage start loading event
        private void browser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            //Добавить иконку загрузки на вкладку
            SetProgressBarStatus(true);
            //_currentSelectedWeb.Focus(FocusState.Programmatic);
        }

        //Event handler when the web page is loaded
        private void browser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            foreach (muxc.TabViewItem tabItem in TabsControl.TabItems)
            {
                if (tabItem.Content == sender && sender != null && args.IsSuccess)
                {
                    SetProgressBarStatus(false);

                    var icoUri = new Uri("https://www.google.com/s2/favicons?domain=" + sender.Source);
                    tabItem.Header = sender.DocumentTitle;
                    tabItem.IconSource = new muxc.BitmapIconSource
                    { UriSource = icoUri, ShowAsMonochrome = false };
                    if (!string.IsNullOrEmpty(SearchBoxText) && tabItem.Header.ToString() != "Параметры")
                        DataTransfer.SaveHistory(sender.DocumentTitle, sender.Source.AbsoluteUri,
                            DateTime.Now.ToLongTimeString(), DateTime.Now.ToShortDateString());

                }

                if (_currentSelectedTab.Content == sender && sender != null)
                {
                    AppTitleText = Application.Current.Resources["AppName"] + " | " + sender?.DocumentTitle;
                    SetSearchBoxStatus(sender.Source.AbsoluteUri);
                    SetBookmarkButtonAppearance();
                }
            }
        }


        //Event handler for opening a new page in a new tab
        private void browser_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            CreateNewWebTab();
            SearchWeb(args.Uri.AbsoluteUri);
        }


        public void CreateNewWebTab()
        {
            var newWebView = new WebView(WebViewExecutionMode.SeparateProcess);
            newWebView.Navigate(new Uri(App.CurrentWebEngine.HomePage));
            newWebView.NavigationCompleted += browser_NavigationCompleted;
            newWebView.NewWindowRequested += browser_NewWindowRequested;
            newWebView.NavigationStarting += browser_NavigationStarting;
            //newWebView.Focus(FocusState.Programmatic);
            newWebView.ContainsFullScreenElementChanged += webView_ContainsFullScreenElementChanged;
            newWebView.Tapped += (_, _) => newWebView.Focus(FocusState.Programmatic);
            var newTab = new muxc.TabViewItem
            {
                Header = newWebView.DocumentTitle == string.Empty ? "Загрузка..." : newWebView.DocumentTitle,
                Content = newWebView,
                IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.More },
                Style = Application.Current.Resources["TabViewItemStyle"] as Style
            };

            TabsControl.TabItems.Add(newTab);
            TabsControl.SelectedItem = newTab;

        }
        private void tabView_AddTabButtonClick(muxc.TabView sender, object args)
        {
            CreateNewWebTab();
        }

        private void CloseWebViewItemRequested(ContentControl tab, muxc.TabView view = null)
        {
            if (tab.Content is WebView webContent)
            {
                webContent.Source = new Uri("about:blank");
                view?.TabItems.Remove(tab);
                tab.Content = null;
                SetProgressBarStatus(false);
            }
          

            if (_currentSelectedTab != null)
                view?.TabItems.Remove(tab);
            else
            {
                AppTitleText = Application.Current.Resources["AppName"].ToString();
                SearchBoxText = string.Empty;
            }
            

        }
        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            CreateNewWebTab();
        }

        private void tabView_TabCloseRequested(muxc.TabView sender, muxc.TabViewTabCloseRequestedEventArgs args)
        {
            CloseWebViewItemRequested(args.Tab, sender);
        }


        private void CloseTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            if (args.Element is not muxc.TabView invokedTabView) return;
            if (!((muxc.TabViewItem)invokedTabView.SelectedItem).IsClosable) return;
            if (invokedTabView.TabItems[invokedTabView.SelectedIndex] is muxc.TabViewItem tabItem)
                CloseWebViewItemRequested(tabItem, invokedTabView);
        }

        private static void webView_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                // The SizeChanged event will be raised when the exit from full-screen mode is complete.
            }
            else
            {
                if (view.TryEnterFullScreenMode())
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
                    // The SizeChanged event will be raised when the entry to full-screen mode is complete.
                }
            }
        }

        public void SearchWeb(string uri)
        {
            if (SearchBoxText.Contains("https://") || SearchBoxText.Contains("http://"))
                _currentSelectedWeb.Source = new Uri(uri);
            else if (_currentSelectedTab == null)
            {
                CreateNewWebTab();
            }

            else if (SearchBoxText == "app://settings")
                AddSettingsTab(0);

            else
                _currentSelectedWeb.Source = new Uri(App.CurrentWebEngine.Prefix + uri);
        }

        public async void GetBookmarks()
        {
            var bookmarksListTransfer = await DataTransfer.GetBookmarkList();
            bookmarksListTransfer.Reverse();
            BookmarksList = bookmarksListTransfer;
        }

        private void SetBookmarkButtonAppearance()
        {
            GetBookmarks();
            if (_bookmarksList == null) return;
            var isExistsBookmark = false;
            _bookmarksList.ForEach(bookmark =>
            {
                if (bookmark == null) return;
                if (bookmark.Url == _currentSelectedWeb.Source.AbsoluteUri)
                    isExistsBookmark = true;
            });
            if (isExistsBookmark)
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

        private void CancelBookmarksBtn_Click(object sender, RoutedEventArgs e)
        {
            BookmarkFlyout.Hide();
        }

        private async void SaveBookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BookmarkTitleForSave != string.Empty && BookmarkUrlForSave != string.Empty &&
                Uri.IsWellFormedUriString(BookmarkUrlForSave, UriKind.Absolute))
            {
                DataTransfer.SaveBookmark(BookmarkTitleForSave, BookmarkUrlForSave);
                BookmarkFlyout.Hide();
                AddBookmarkIcon = Constants.Constants.ActiveIcon;
                DeleteBookmarkButtonVisibility = Visibility.Visible;
            }
            else
            {
                var dialogError = new ContentDialog
                {
                    Title = "Неверные данные",
                    Content = "Введите верные параметры",
                    CloseButtonText = "Закрыть"
                };

                await dialogError.ShowAsync();
            }
        }

        private async void DeleteBookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await DataTransfer.RemoveBookmark(_currentSelectedWeb.Source.AbsoluteUri);

            if (!result) return;
            AddBookmarkIcon = Constants.Constants.UnactiveIcon;
            DeleteBookmarkButtonVisibility = Visibility.Collapsed;
            BookmarkFlyout.Hide();
        }

        private void tabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _currentSelectedTab = TabsControl.SelectedItem as muxc.TabViewItem;
                if (_currentSelectedTab == null) return;
                _currentSelectedWeb = _currentSelectedTab.Content as WebView;
                if (_currentSelectedTab == _settingsTab)
                {
                    AppTitleText = "NetBrowser | Настройки";
                    SetSearchBoxStatus("app://settings");
                }
                else if (_currentSelectedWeb != null && _currentSelectedTab != null)
                {
                    AppTitleText = "NetBrowser" + " | " + _currentSelectedWeb.DocumentTitle;
                    SetSearchBoxStatus(_currentSelectedWeb.Source.AbsoluteUri);
                    SetBookmarkButtonAppearance();
                }
            }
            catch
            {
                // ignored
            }
        }

        private void AddSettingsTab(int mode)
        {
            _settingsTab = new muxc.TabViewItem
            {
                Header = "Настройки",
                IconSource = new muxc.SymbolIconSource { Symbol = Symbol.Setting },
                Style = Application.Current.Resources["TabViewItemStyle"] as Style
            };
            var setFrame = new Frame();
            _settingsTab.Content = setFrame;
            setFrame.Navigate(typeof(SettingsPage));
            SettingsPage.CurrentMode = mode;

            TabsControl.TabItems.Add(_settingsTab);
            TabsControl.SelectedItem = _settingsTab;
        }

        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!TabsControl.TabItems.Contains(_settingsTab)) AddSettingsTab(0);
        }

        private async void historyBtn_Click(object sender, RoutedEventArgs e)
        {
            var historyListTransfer = await DataTransfer.GetHistory("url");
            historyListTransfer.Reverse();
            HistoryList = historyListTransfer.Count <= 100 ? historyListTransfer : historyListTransfer.GetRange(0, 100);
        }

        private void CreateErrorLoadPage(string url)
        {
            var webView = new WebView();
            try
            {
                webView.Navigate(new Uri(url));
            }
            catch (Exception ex)
            {
                ErrorLoadPage.error = ex.Message;
            }

            var errorTab = new muxc.TabViewItem
            {
                Header = "Ошибка",
                IconSource = new muxc.SymbolIconSource { Symbol = Symbol.Cancel }
            };
            var setFrame = new Frame();
            errorTab.Content = setFrame;
            setFrame.Navigate(typeof(ErrorLoadPage));
            TabsControl.TabItems.Add(errorTab);
            TabsControl.SelectedItem = errorTab;
        }

        private async void historyListView_ItemClick(object sender, [NotNull] ItemClickEventArgs e)
        {
            var historyItem = e.ClickedItem as HistoryItemDetails;
            var url = historyItem?.Url;
            var isUri = Uri.IsWellFormedUriString(url, UriKind.Absolute);
            if (isUri)
            {
                CreateNewWebTab();
                if (url != null) SearchWeb(url);
                //FlyoutHistory.Hide();
                IsFlyoutClosed = true;
            }
            else
            {
                CreateErrorLoadPage(url);
                //FlyoutHistory.Hide();
                IsFlyoutClosed = true;

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
            AddSettingsTab(3);
            IsFlyoutClosed = true;
            //FlyoutBookmarks.Hide();
        }

        private void bookmarksFlyoutListView_ItemClick(object sender, [NotNull] ItemClickEventArgs e)
        {
            var a = (BookmarkDetails)e.ClickedItem;
            CreateNewWebTab();
            SearchWeb(a.Url);
            IsFlyoutClosed = true;
            //FlyoutBookmarks.Hide();
        }
        private void HistorySettingsButton_Click(object sender, RoutedEventArgs e)
        {
            AddSettingsTab(5);
            //FlyoutHistory.Hide();
            IsFlyoutClosed = true;
        }


    }

}