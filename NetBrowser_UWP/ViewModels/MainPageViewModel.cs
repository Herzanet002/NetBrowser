using NetBrowser_UWP.BindingHelpers;
using NetBrowser_UWP.Commands;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser_UWP.Views;
using NetBrowser_UWP.Views.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using muxc = Microsoft.UI.Xaml.Controls;

namespace NetBrowser_UWP.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        #region PrivateGlobalElementRegion

        private const string FAVICONS_SERVICE = "https://www.google.com/s2/favicons?domain=";

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
        private ObservableCollection<object> _tabViewItemsList;
        private List<string> _searchBoxItemsCollection;
        private bool _isSearchBoxListOpen;

        #endregion

        #region Commands
        public Command BackButtonCommand => new Command(OnBackButtonCommandExecuted, _ => true);
        public Command ForwardButtonCommand => new Command(OnForwardButtonCommandExecuted, _ => true);
        public Command ReloadButtonCommand => new Command(OnReloadButtonCommandExecuted, _ => true);
        public Command HomeButtonCommand => new Command(OnHomeButtonCommandExecuted, _ => true);
        public Command SearchButtonCommand => new Command(OnSearchButtonCommandExecuted, _ => true);
        public Command AddBookmarkButtonCommand => new Command(OnAddBookmarkButtonCommandExecuted, _ => true);
        public Command SaveBookmarkButtonCommand => new Command(OnSaveBookmarkCommandExecuted, _ => true);
        public Command CancelSaveBookmarkButtonCommand => new Command(OnCancelSaveBookmarkCommandExecuted, _ => true);
        public Command DeleteBookmarkButtonCommand => new Command(OnDeleteBookmarkCommandExecuted, _ => true);
        public Command BookmarksButtonCommand => new Command(OnBookmarksButtonCommandExecuted, _ => true);
        public Command BookmarksSettingsButtonCommand => new Command(OnBookmarkSettingButtonExecuted, _ => true);
        public Command BookmarksItemClickCommand => new Command(OnBookmarksFlyoutListViewItemClickExecuted, _ => true);
        public Command SearchBoxTextChangedCommand => new Command(OnSearchBoxTextChangedCommandExecuted, _ => true);
        public Command SearchBoxQuerySubmittedCommand => new Command(OnSearchBoxQuerySubmittedCommandExecuted, _ => true);

        public ICommand HistoryButtonCommand => new Command(OnHistoryButtonCommandExecuted, _ => true);
        public ICommand HistorySettingsButtonCommand => new Command(OnHistorySettingsButtonExecuted, _ => true);
        public ICommand HistoryItemClickCommand => new Command(OnHistoryFlyoutItemClickCommandExecuted, _ => true);


        public ICommand SettingsButtonCommand => new Command(OnSettingsButtonCommandExecuted, _ => true);
        public ICommand AddTabButtonCommand => new Command(OnAddTabButtonCommandExecuted, _ => true);
        public ICommand CloseTabButtonCommand => new Command(OnCloseTabButtonCommandExecuted, _ => true);

        #endregion

        #region Global Properties
        public ObservableCollection<object> TabViewItemsList
        {
            get => _tabViewItemsList;
            set => Set(ref _tabViewItemsList, value);
        }

        public WebView CurrentSelectedWebView
        {
            get => _currentSelectedWeb;
            set => Set(ref _currentSelectedWeb, value);
        }
        public List<string> SearchBoxItemsCollection
        {
            get => _searchBoxItemsCollection;
            set => Set(ref _searchBoxItemsCollection, value);
        }
        public string SearchBoxText
        {
            get => _searchBoxText;
            set => Set(ref _searchBoxText, value);

        }
        public List<BookmarkDetails> BookmarksList
        {
            get => _bookmarksList;
            set => Set(ref _bookmarksList, value);
        }
        public List<HistoryItemDetails> HistoryList
        {
            get => _historyList;
            set => Set(ref _historyList, value);
        }
        public string AppTitleText
        {
            get => _appTitleText;
            set => Set(ref _appTitleText, value);
        }

        public string BookmarkTitleForSave
        {
            get => _bookmarkTitleForSave;
            set => Set(ref _bookmarkTitleForSave, value);
        }

        public string BookmarkUrlForSave
        {
            get => _bookmarkUrlForSave;
            set => Set(ref _bookmarkUrlForSave, value);
        }
        public FontIcon AddBookmarkIcon
        {
            get => _addbookmarkIcon;
            set => Set(ref _addbookmarkIcon, value);
        }

        public FontIcon RefreshButtonIcon
        {
            get => _refreshButtonIcon;
            set => Set(ref _refreshButtonIcon, value);
        }

        public Visibility ProgressBarVisibility
        {
            get => _visibilityProgressBar;
            set => Set(ref _visibilityProgressBar, value);
        }

        public Visibility DeleteBookmarkButtonVisibility
        {
            get => _visibilityDeleteBookmarkButton;
            set => Set(ref _visibilityDeleteBookmarkButton, value);
        }

        public bool IsFlyoutClosed
        {
            get => _isFlyoutClosed;
            set
            {
                Set(ref _isFlyoutClosed, value);
                if (value)
                    IsFlyoutClosed = false;
            }
        }

        public muxc.TabViewItem CurrentSelectedTab
        {
            get => _currentSelectedTab;
            set
            {
                Set(ref _currentSelectedTab, value);
                SelectionChangedTabHandler();
            }
        }

        public bool IsSearchBoxListOpen
        {
            get => _isSearchBoxListOpen;
            set => Set(ref _isSearchBoxListOpen, value);
        }

        #endregion

        #region OnCommandExecuted

        private void OnBackButtonCommandExecuted(object sender)
        {
            if (CurrentSelectedWebView is { CanGoBack: true })
                CurrentSelectedWebView.GoBack();
        }

        //Browser forward button functionality
        private void OnForwardButtonCommandExecuted(object sender)
        {
            if (CurrentSelectedWebView is { CanGoForward: true })
                CurrentSelectedWebView.GoForward();
        }

        //Browser refresh button functionality
        private void OnReloadButtonCommandExecuted(object sender)
        {
            if (CurrentSelectedWebView == null || !WebViewStates.ContainsKey(CurrentSelectedWebView)) return;
            if (RefreshButtonIcon == Constants.Constants.RefreshButtonIcon)
            {
                WebViewStates[CurrentSelectedWebView] = true;
                CurrentSelectedWebView.Refresh();
            }
            else
            {
                WebViewStates[CurrentSelectedWebView] = false;
                CurrentSelectedWebView.Stop();
            }
            SetVisualUiElementStates(CurrentSelectedWebView);
        }

        private void OnHomeButtonCommandExecuted(object sender)
        {
            if (App.CurrentWebEngine?.HomePage != null && CurrentSelectedWebView != null)
                NavigateTo(App.CurrentWebEngine.HomePage, CurrentSelectedWebView);

        }
        private void OnSearchButtonCommandExecuted(object sender)
        {
            if (SearchBoxText == null) return;
            _dataTransferService.SaveSearchTerm(SearchBoxText);
            NavigateTo(SearchBoxText, CurrentSelectedWebView);
        }

        private void OnSettingsButtonCommandExecuted(object sender)
        {
            CreateSettingsTab();
        }
        private async void OnHistoryButtonCommandExecuted(object sender)
        {
            var historyListTransfer = await _dataTransferService.GetHistory();
            var historyListCount = historyListTransfer.Count;
            historyListTransfer = historyListCount <= 100 ? historyListTransfer : historyListTransfer.GetRange(historyListCount - 100, 100);
            historyListTransfer.Reverse();
            HistoryList = historyListTransfer;
        }

        private void OnHistoryFlyoutItemClickCommandExecuted(object obj)
        {
            if (obj is not ItemClickEventArgs objArgs) return;
            if (objArgs.ClickedItem is HistoryItemDetails selectedHistoryItem)
            {
                var url = selectedHistoryItem.Url;
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) return;
                CreateNewWebTab();
                if (url != null)
                    NavigateTo(url, CurrentSelectedWebView);
            }

            IsFlyoutClosed = true;
        }
        private void OnBookmarksButtonCommandExecuted(object sender) => GetBookmarksAsync();

        private void OnCancelSaveBookmarkCommandExecuted(object sender)
        {
            IsFlyoutClosed = true;
        }

        private async void OnSaveBookmarkCommandExecuted(object sender)
        {
            if (BookmarkTitleForSave != string.Empty && BookmarkUrlForSave != string.Empty &&
                Uri.IsWellFormedUriString(BookmarkUrlForSave, UriKind.Absolute))
            {
                _dataTransferService.SaveBookmark(
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

        private async void OnDeleteBookmarkCommandExecuted(object sender)
        {
            var result = await _dataTransferService.RemoveBookmark(CurrentSelectedWebView?.Source.AbsoluteUri);
            if (!result) return;
            SetBookmarkIconState(false);
            IsFlyoutClosed = true;
        }

        private void OnAddBookmarkButtonCommandExecuted(object sender)
        {
            if (CurrentSelectedWebView == null) return;
            BookmarkTitleForSave = CurrentSelectedWebView.DocumentTitle;
            BookmarkUrlForSave = CurrentSelectedWebView.Source.AbsoluteUri;
        }

        private void OnBookmarkSettingButtonExecuted(object sender)
        {
            CreateSettingsTab(3);
            IsFlyoutClosed = true;
        }

        private void OnBookmarksFlyoutListViewItemClickExecuted(object sender)
        {
            if (sender is not ItemClickEventArgs objArgs) return;
            if (objArgs.ClickedItem is not BookmarkDetails selectedBookmarkItem) return;
            CreateNewWebTab(selectedBookmarkItem.Url);
            IsFlyoutClosed = true;

        }

        private void OnHistorySettingsButtonExecuted(object sender)
        {
            CreateSettingsTab(5);
            IsFlyoutClosed = true;
        }
        private void OnAddTabButtonCommandExecuted(object sender)
        {
            CreateStartPageTab();
        }

        private void OnCloseTabButtonCommandExecuted(object sender)
        {
            if (sender is muxc.TabViewTabCloseRequestedEventArgs tab)
                CloseTabItemRequested(tab.Tab);
        }


        #endregion

        private readonly IDataTransferService _dataTransferService;

        public MainPageViewModel(IDataTransferService dataTransferService)
        {
            _dataTransferService = dataTransferService;

            TabViewItemsList = new ObservableCollection<object>();

            GetSearchTermList();
            CreateNewWebTab();
        }

        private async void GetSearchTermList()
        {
            var searchTermListTransfer = await _dataTransferService.GetSearchTerm();
            if (searchTermListTransfer == null) return;
            searchTermListTransfer.Reverse();
            _searchTermList = new HashSet<string>(searchTermListTransfer);

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

        private void OnSearchBoxTextChangedCommandExecuted(object obj)
        {
            if (obj is not AutoSuggestBoxTextChangedEventArgs eventArgs) return;

            var reason = eventArgs.Reason;
            if (reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                SearchBoxItemsCollection = AutoSuggestListFill(SearchBoxText);
            }
        }

        //TODO: Обновление поисковых запросов
        private void OnSearchBoxQuerySubmittedCommandExecuted(object obj)
        {
            if (obj is not AutoSuggestBoxQuerySubmittedEventArgs eventArgs) return;

            var queryForSearch = string.Empty;
            if (eventArgs.ChosenSuggestion != null)
                queryForSearch = eventArgs.ChosenSuggestion.ToString();

            else if (!string.IsNullOrEmpty(eventArgs.QueryText))
                queryForSearch = eventArgs.QueryText;

            if (CurrentSelectedWebView == null)
            {
                CreateNewWebTab();
            }
            NavigateTo(queryForSearch, CurrentSelectedWebView);
            _dataTransferService.SaveSearchTerm(queryForSearch);
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
            AppTitleText = ResourceExtensions.GetLocalized("AppDisplayName") + " | " + appTitleText;
            SearchBoxText = searchBoxText;
        }
        private void Browser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (!WebViewStates.ContainsKey(sender)) return;
            WebViewStates[sender] = true;
            SetVisualUiElementStates(sender);
            AppTitleText = ResourceExtensions.GetLocalized("AppDisplayName") + " | " + ResourceExtensions.GetLocalized("LoadingString");

        }


        private void Browser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            foreach (muxc.TabViewItem tabItem in TabViewItemsList)
            {
                if (sender == null || sender.Source == null || tabItem.Content != sender) continue;
                var icoUri = new Uri(FAVICONS_SERVICE + sender.Source);
                tabItem.Header = sender.DocumentTitle;
                tabItem.IconSource = new muxc.BitmapIconSource
                {
                    UriSource = icoUri,
                    ShowAsMonochrome = false
                };

                _dataTransferService.SaveHistory(new HistoryItemDetails
                {
                    Name = sender.DocumentTitle,
                    Url = sender.Source.AbsoluteUri,
                    Time = DateTime.Now.ToLongTimeString(),
                    Date = DateTime.Now.ToShortDateString(),
                });

                WebViewStates[sender] = false;
            }

            if (sender == null || sender.Source == null || CurrentSelectedWebView != sender) return;
            SetVisualUiLabels(sender.DocumentTitle, sender.Source.AbsoluteUri);
            SetVisualUiElementStates(sender);

        }

        //Event handler for opening a new page in a new tab
        private void Browser_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            CreateNewWebTab(args.Uri.AbsoluteUri);

        }

        private WebView CreateWebViewInstance(string urlToNavigate)
        {
            var newWebViewInstance = new WebView(WebViewExecutionMode.SeparateProcess);
            WebViewStates.Add(newWebViewInstance, true);
            newWebViewInstance.NavigationCompleted += Browser_NavigationCompleted;
            newWebViewInstance.NewWindowRequested += Browser_NewWindowRequested;
            newWebViewInstance.NavigationStarting += Browser_NavigationStarting;
            newWebViewInstance.Navigate(new Uri(urlToNavigate));
            return newWebViewInstance;
        }


        private muxc.TabViewItem CreateTabViewItemInstance(string header, object content, muxc.IconSource icon, Style style)
        {
            var newTab = new muxc.TabViewItem
            {
                Header = string.IsNullOrEmpty(header) ? ResourceExtensions.GetLocalized("LoadingString") : header,
                Content = content,
                IconSource = icon,
                Style = style,
                IsRightTapEnabled = true
            };
            return newTab;
        }


        public void NavigateTo(string address, WebView webViewInstance)
        {
            if (webViewInstance == null) return;

            switch (address)
            {
                case "app://settings":
                    CreateSettingsTab();
                    break;
                case "app://newtab":
                    CreateStartPageTab();
                    break;

                default:
                    webViewInstance.Source = address.Contains("https://") || address.Contains("http://") ?
                        new Uri(address) :
                        new Uri(App.CurrentWebEngine?.Prefix + address);

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

            TabViewItemsList.Add(newTab);
            CurrentSelectedTab = newTab;
        }


        public void CreateStartPageTab()
        {
            var startPageTab = new muxc.TabViewItem
            {
                Header = ResourceExtensions.GetLocalized("NewTabTitle"),
                IconSource = new muxc.SymbolIconSource { Symbol = Symbol.NewWindow },
                Style = Application.Current.Resources["TabViewItemStyle"] as Style,
                Content = new StartPage()
            };
            TabViewItemsList.Add(startPageTab);
            CurrentSelectedTab = startPageTab;
        }
        public void CreateSettingsTab(int mode = 0)
        {
            var settingsTab = new muxc.TabViewItem
            {
                Header = ResourceExtensions.GetLocalized("SettingsText"),
                IconSource = new muxc.SymbolIconSource { Symbol = Symbol.Setting },
                Style = Application.Current.Resources["TabViewItemStyle"] as Style,
                Content = new SettingsPage(mode)
            };
            TabViewItemsList.Add(settingsTab);
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
            TabViewItemsList.Add(newTab);
            CurrentSelectedTab = newTab;
            TabViewItemsList.Remove(previousTab);

        }


        private void SelectionChangedTabHandler()
        {
            if (CurrentSelectedTab == null) return;
            CurrentSelectedWebView = CurrentSelectedTab.Content as WebView;

            switch (CurrentSelectedTab.Content)
            {
                case SettingsPage:
                    SetVisualUiLabels(ResourceExtensions.GetLocalized("SettingsText"), "app://settings");
                    break;
                case StartPage:
                    SetVisualUiLabels(ResourceExtensions.GetLocalized("NewTabTitle"), string.Empty);
                    break;
                default:
                    if (CurrentSelectedWebView != null && CurrentSelectedWebView.Source != null)
                        SetVisualUiLabels(CurrentSelectedWebView.DocumentTitle, CurrentSelectedWebView.Source.AbsoluteUri);
                    break;
            }
            SetVisualUiElementStates(CurrentSelectedWebView);
        }
        private void CloseTabItemRequested(muxc.TabViewItem tab)
        {
            if (tab.Content is WebView webContent)
            {
                WebViewStates.Remove(webContent);

                webContent.Source = new Uri(@"about:blank");
                tab.Content = null;
            }
            TabViewItemsList.Remove(tab);


        }
        private void CloseTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            if (args.Element is not muxc.TabView invokedTabView) return;
            if (!((muxc.TabViewItem)invokedTabView.SelectedItem).IsClosable) return;
            if (invokedTabView.TabItems[invokedTabView.SelectedIndex] is muxc.TabViewItem tabItem)
                CloseTabItemRequested(tabItem);
        }


        private async void GetBookmarksAsync()
        {
            var bookmarksListTransfer = await _dataTransferService.GetBookmarkList();
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
            if (CurrentSelectedWebView == null)
            {
                SetBookmarkIconState(false);
                return;
            }
            GetBookmarksAsync();
            if (BookmarksList == null) return;
            var isExistsBookmark = false;
            BookmarksList.ForEach(bookmark =>
            {
                if (bookmark == null || CurrentSelectedWebView.Source == null) return;
                if (bookmark.Url == CurrentSelectedWebView.Source.AbsoluteUri)
                    isExistsBookmark = true;
            });

            SetBookmarkIconState(isExistsBookmark);
        }
    }
}
