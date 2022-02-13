using NetBrowser_UWP.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.System;
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

        private string _appTitleText;
        private string _searchBoxText;

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

        public string AppTitleText
        {

            get => _appTitleText;
            set
            {
                _appTitleText = value;
                OnPropertyChanged(nameof(AppTitleText));
            }
        }

        public MainPage()
        {
            InitializeComponent();
            GetBookmarks();
            ThemeManager.SetRequestedTheme();
            SetCurrentEngine();
            DataContext = this;
            Browser = new WebView(WebViewExecutionMode.SeparateProcess)
            {
                Source = new Uri(App.CurrentWebEngine.homePage)
            };
            Browser.NavigationCompleted += browser_NavigationCompleted;
            Browser.NewWindowRequested += browser_NewWindowRequested;
            Browser.NavigationStarting += browser_NavigationStarting;
            Browser.ContainsFullScreenElementChanged += webView_ContainsFullScreenElementChanged;
            DefaultTab.Content = Browser;
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
        private void searchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) SearchWeb();
        }

        //Browser search button functionality
        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchWeb();
        }

        //Event handler for the webpage start loading event
        private void browser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            SetSearchBoxStatus(_currentSelectedWeb.Source.ToString());
            BrowserProgress.IsEnabled = true;
            BrowserProgress.Visibility = Visibility.Visible;
        }

        //Event handler when the web page is loaded
        private void browser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            BrowserProgress.IsEnabled = false;
            BrowserProgress.Visibility = Visibility.Collapsed;
            try
            {
                AppTitleText = "NetBrowser" + " | " + sender.DocumentTitle;
                var icoUri = new Uri("https://www.google.com/s2/favicons?domain=" + sender.Source);
                _currentSelectedTab.Header = sender.DocumentTitle;
                _currentSelectedTab.IconSource = new muxc.BitmapIconSource
                { UriSource = icoUri, ShowAsMonochrome = false };
                SetSearchBoxStatus(sender.Source.AbsoluteUri);
                if (!string.IsNullOrEmpty(SearchBoxText))
                    DataTransfer.SaveHistory(_currentSelectedWeb.DocumentTitle, _currentSelectedWeb.Source.AbsoluteUri);
                SetBookmarkButtonAppearance();
            }
            catch
            {
                // ignored
            }
        }

        //Event handler for opening a new page in a new tab
        private void browser_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            CreateNewWebTab();
            SearchWeb(args.Uri);
        }


        public void CreateNewWebTab()
        {
            var wb = new WebView(WebViewExecutionMode.SeparateProcess);
            wb.Navigate(new Uri(App.CurrentWebEngine.homePage));
            wb.NavigationCompleted += browser_NavigationCompleted;
            wb.NewWindowRequested += browser_NewWindowRequested;
            wb.NavigationStarting += browser_NavigationStarting;
            wb.ContainsFullScreenElementChanged += webView_ContainsFullScreenElementChanged;
            var newTab = new muxc.TabViewItem
            {
                Header = wb.DocumentTitle,
                Content = wb
            };

            TabsControl.TabItems.Add(newTab);
            TabsControl.SelectedItem = newTab;
        }


        private void tabView_AddTabButtonClick(muxc.TabView sender, object args)
        {
            CreateNewWebTab();
        }


        private void tabView_TabCloseRequested(muxc.TabView sender, muxc.TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }


        private static void webView_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            if (sender.ContainsFullScreenElement)
                applicationView.TryEnterFullScreenMode();
            else if (applicationView.IsFullScreenMode) applicationView.ExitFullScreenMode();
        }

        public void SearchWeb()
        {
            if (SearchBoxText.Contains("https://"))
                _currentSelectedWeb.Source = new Uri(SearchBoxText);
            else if (_currentSelectedWeb == null)
            {
                if (_currentSelectedWeb != null)
                    _currentSelectedWeb.Source = new Uri(App.CurrentWebEngine.Prefix+ SearchBoxText); //изменять динамически движок
            }

            else if (SearchBoxText == "app://settings")
                AddSettingsTab(0);

            else
                _currentSelectedWeb.Source = new Uri(App.CurrentWebEngine.Prefix + SearchBoxText);
        }

        public void SearchWeb(Uri uri)
        {
            _currentSelectedWeb.Navigate(uri);
        }

        public async void GetBookmarks()
        {
            _bookmarksList = await DataTransfer.GetBookmarkList();
            _bookmarksList.Reverse();
            BookmarksFlyoutListView.ItemsSource = _bookmarksList;
        }

        private void SetBookmarkButtonAppearance()
        {
            GetBookmarks();
            if (_bookmarksList == null) return;
            var isExistsBookmark = false;


            _bookmarksList.ForEach(bookmark =>
            {
                if (bookmark != null)
                    if (bookmark.Url == _currentSelectedWeb.Source.AbsoluteUri)
                        isExistsBookmark = true;
            });
            if (isExistsBookmark)
            {
                AddBookmarksButton.Content = Constants.Constants.ActiveIcon;
                DeleteBookmarkBtn.Visibility = Visibility.Visible;
            }

            else
            {
                AddBookmarksButton.Content = Constants.Constants.UnactiveIcon;
                DeleteBookmarkBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelBookmarksBtn_Click(object sender, RoutedEventArgs e)
        {
            BookmarkFlyout.Hide();
        }

        private async void SaveBookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BookmarkTitle.Text != string.Empty && BookmarkUrl.Text != string.Empty &&
                Uri.IsWellFormedUriString(BookmarkUrl.Text, UriKind.Absolute))
            {
                DataTransfer.SaveBookmark(BookmarkTitle.Text, BookmarkUrl.Text);
                BookmarkFlyout.Hide();
                AddBookmarksButton.Content = Constants.Constants.ActiveIcon;
                DeleteBookmarkBtn.Visibility = Visibility.Visible;
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
            AddBookmarksButton.Content = Constants.Constants.UnactiveIcon;
            DeleteBookmarkBtn.Visibility = Visibility.Collapsed;
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
                IconSource = new muxc.SymbolIconSource { Symbol = Symbol.Setting }
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
            var historyList = await DataTransfer.GetHistory("url");
            historyList.Reverse();
            HistoryListView.ItemsSource = historyList;
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
                if (url != null) SearchWeb(new Uri(url));
                FlyoutHistory.Hide();
            }
            else
            {
                CreateErrorLoadPage(url);
                FlyoutHistory.Hide();
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
            BookmarkTitle.Text = _currentSelectedWeb.DocumentTitle;
            BookmarkUrl.Text = _currentSelectedWeb.Source.AbsoluteUri;
        }

        private void bookmarksBtn_Click(object sender, RoutedEventArgs e)
        {
            GetBookmarks();
        }

        private void bookmarkSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            AddSettingsTab(3);

            FlyoutBookmarks.Hide();
        }

        private void bookmarksFlyoutListView_ItemClick(object sender, [NotNull] ItemClickEventArgs e)
        {
            var a = (BookmarkDetails)e.ClickedItem;
            CreateNewWebTab();
            SearchWeb(new Uri(a.Url));
            FlyoutBookmarks.Hide();
        }


    }
}