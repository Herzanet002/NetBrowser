using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using Microsoft.Web.WebView2.Core;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Views;
using NetBrowser_UWP.Views.Settings;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using winUI = Microsoft.UI.Xaml.Controls;

namespace NetBrowser_UWP.ViewModels
{
    public class MainPageViewModel : ObservableObject
    {
        #region Private Global Element Region

        private winUI.TabViewItem _selectedTabItem;
        private winUI.WebView2 _selectedWebView2;

        private ObservableCollection<BookmarkDetails> _bookmarksList;
        private IList<HistoryItemDetails> _historyList;
        private ObservableCollection<winUI.TabViewItem> _tabViewItemsList;
        private IList<string> _searchBoxItemsCollection;

        private string _appTitleText;
        private string _searchBoxText;
        private string _bookmarkTitleForSave;
        private string _bookmarkUrlForSave;

        private Visibility _visibilityDeleteBookmarkButton;
        private bool _visibilityHomeButton;
        private bool _isProgressRingActive;
        private bool _isFlyoutClosed;

        private bool _isWebLoading;
        private bool _isBookmarksExists;

        #endregion Private Global Element Region

        #region Commands Region

        public DelegateCommand BackButtonCommand { get; private set; }
        public DelegateCommand ForwardButtonCommand { get; private set; }
        public DelegateCommand ReloadButtonCommand { get; private set; }
        public DelegateCommand StopLoadingButtonCommand { get; private set; }
        public DelegateCommand HomeButtonCommand { get; private set; }
        public DelegateCommand SearchButtonCommand { get; private set; }
        public DelegateCommand AddBookmarkButtonCommand { get; private set; }
        public DelegateCommand SaveBookmarkButtonCommand { get; private set; }
        public DelegateCommand CancelSaveBookmarkButtonCommand { get; private set; }
        public DelegateCommand DeleteBookmarkButtonCommand { get; private set; }
        public DelegateCommand BookmarksButtonCommand { get; private set; }
        public DelegateCommand BookmarksSettingsButtonCommand { get; private set; }
        public DelegateCommand<object> BookmarksItemClickCommand { get; private set; }
        public DelegateCommand<object> SearchBoxTextChangedCommand { get; private set; }
        public DelegateCommand<object> SearchBoxQuerySubmittedCommand { get; private set; }
        public DelegateCommand HistoryButtonCommand { get; private set; }
        public DelegateCommand HistorySettingsButtonCommand { get; private set; }
        public DelegateCommand<object> HistoryItemClickCommand { get; private set; }
        public DelegateCommand SettingsButtonCommand { get; private set; }
        public DelegateCommand AddTabButtonCommand { get; private set; }
        public DelegateCommand<object> CloseTabButtonCommand { get; private set; }
        public DelegateCommand DeveloperInstrumentsButtonCommand { get; private set; }
        public DelegateCommand TaskManagerButtonCommand { get; private set; }
        public DelegateCommand NewsContentButtonCommand { get; private set; }

        #endregion Commands Region

        #region Global Properties Region

        public ObservableCollection<winUI.TabViewItem> TabViewItemsList
        {
            get => _tabViewItemsList;
            set => SetProperty(ref _tabViewItemsList, value);
        }

        public winUI.WebView2 SelectedWebView2
        {
            get => _selectedWebView2;
            set => SetProperty(ref _selectedWebView2, value);
        }

        public IList<string> SearchBoxItemsCollection
        {
            get => _searchBoxItemsCollection;
            set => SetProperty(ref _searchBoxItemsCollection, value);
        }

        public string SearchBoxText
        {
            get => _searchBoxText;
            set => SetProperty(ref _searchBoxText, value);
        }

        public ObservableCollection<BookmarkDetails> BookmarksList
        {
            get => _bookmarksList;
            set => SetProperty(ref _bookmarksList, value);
        }

        public IList<HistoryItemDetails> HistoryList
        {
            get => _historyList;
            set => SetProperty(ref _historyList, value);
        }

        public string AppTitleText
        {
            get => _appTitleText;
            set => SetProperty(ref _appTitleText, value);
        }

        public string BookmarkTitleForSave
        {
            get => _bookmarkTitleForSave;
            set => SetProperty(ref _bookmarkTitleForSave, value);
        }

        public string BookmarkUrlForSave
        {
            get => _bookmarkUrlForSave;
            set => SetProperty(ref _bookmarkUrlForSave, value);
        }

        public bool IsProgressRingActive
        {
            get => _isProgressRingActive;
            set => SetProperty(ref _isProgressRingActive, value);
        }

        public Visibility DeleteBookmarkButtonVisibility
        {
            get => _visibilityDeleteBookmarkButton;
            set => SetProperty(ref _visibilityDeleteBookmarkButton, value);
        }

        public bool IsFlyoutClosed
        {
            get => _isFlyoutClosed;
            set
            {
                SetProperty(ref _isFlyoutClosed, value);
                if (value)
                    IsFlyoutClosed = false;
            }
        }

        public winUI.TabViewItem SelectedTabItem
        {
            get => _selectedTabItem;
            set
            {
                SetProperty(ref _selectedTabItem, value);
                SelectionChangedTabHandler();
            }
        }

        public bool IsWebLoading
        {
            get => _isWebLoading;
            set => SetProperty(ref _isWebLoading, value);
        }

        public bool IsBookmarksExists
        {
            get => _isBookmarksExists;
            set => SetProperty(ref _isBookmarksExists, value);
        }

        public bool VisibilityHomeButton
        {
            get => _visibilityHomeButton;
            set
            {
                SetProperty(ref _visibilityHomeButton, value);
                _localSettingsService.SaveSettingAsync("IsHomeButtonEnabled", value);
            }
        }

        #endregion Global Properties Region

        #region On Command Executed Region

        private void OnBackButtonCommandExecuted()
        {
            if (SelectedWebView2 is { CanGoBack: true })
                SelectedWebView2.GoBack();
        }

        private void OnForwardButtonCommandExecuted()
        {
            if (SelectedWebView2 is { CanGoForward: true })
                SelectedWebView2.GoForward();
        }

        private void OnReloadButtonCommandExecuted()
        {
            SelectedWebView2?.CoreWebView2.Reload();
        }

        private void OnStopLoadingButtonCommandExecuted()
        {
            SelectedWebView2?.CoreWebView2.Stop();
        }

        private void OnNewsContentButtonCommandExecuted()
        {
            CreateNewContentTab();
        }

        private void OnHomeButtonCommandExecuted()
        {
            if (App.CurrentWebEngine?.HomePage != null
                && SelectedWebView2 != null)
            {
                NavigateTo(App.CurrentWebEngine.HomePage, SelectedWebView2);
            }
        }

        private void OnSearchBoxTextChangedCommandExecuted(object obj)
        {
            if (obj is not AutoSuggestBoxTextChangedEventArgs eventArgs) return;
            if (eventArgs.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                AutoSuggestListFill();
            }
        }

        //TODO: Обновление поисковых запросов
        private void OnSearchBoxQuerySubmittedCommandExecuted(object obj)
        {
            if (obj is not AutoSuggestBoxQuerySubmittedEventArgs eventArgs) return;

            var queryForSearch = string.Empty;
            if (!string.IsNullOrWhiteSpace(eventArgs.QueryText))
                queryForSearch = eventArgs.QueryText;

            if (string.IsNullOrWhiteSpace(queryForSearch)) return;
            if (SelectedWebView2 == null) return;

            NavigateTo(queryForSearch, SelectedWebView2);
            _dataTransferService.SaveSearchTerm(new SiteItem
            {
                Name = queryForSearch
            });
        }

        private void OnSearchButtonCommandExecuted()
        {
            if (string.IsNullOrWhiteSpace(SearchBoxText)) return;
            NavigateTo(SearchBoxText, SelectedWebView2);
            _dataTransferService.SaveSearchTerm(new SiteItem
            {
                Name = SearchBoxText
            });
        }

        private void OnSettingsButtonCommandExecuted() => CreateSettingsTab();

        private void OnDeveloperInstrumentsButtonCommandExecuted() =>
            SelectedWebView2?.CoreWebView2.OpenDevToolsWindow();

        private void OnTaskManagerButtonCommandExecuted() =>
            SelectedWebView2?.CoreWebView2.OpenTaskManagerWindow();

        private async void OnHistoryButtonCommandExecuted()
        {
            var historyListTransfer = await _dataTransferService.GetHistory();

            const int MAX_DISPLAY_COUNT = 100;

            HistoryList = historyListTransfer.Count <= MAX_DISPLAY_COUNT ?
                historyListTransfer.Reverse().ToList() :
                historyListTransfer.Skip(Math.Max(0, historyListTransfer.Count() - MAX_DISPLAY_COUNT)).Reverse().ToList();
        }

        private void OnHistoryFlyoutItemClickCommandExecuted(object obj)
        {
            if (obj is not ItemClickEventArgs objArgs) return;
            if (objArgs.ClickedItem is HistoryItemDetails selectedHistoryItem)
            {
                var url = selectedHistoryItem.Url;
                CreateNewWebTab();
                if (url != null)
                    NavigateTo(url, SelectedWebView2);
            }

            IsFlyoutClosed = true;
        }

        private void OnBookmarksButtonCommandExecuted() => GetBookmarksAsync();

        private void OnCancelSaveBookmarkCommandExecuted() => IsFlyoutClosed = true;

        private async void OnSaveBookmarkCommandExecuted()
        {
            if (!(string.IsNullOrWhiteSpace(BookmarkTitleForSave) ||
                  string.IsNullOrWhiteSpace(BookmarkUrlForSave)) &&
                Uri.IsWellFormedUriString(BookmarkUrlForSave, UriKind.Absolute))
            {
                await _dataTransferService.SaveBookmark(
                    new BookmarkDetails()
                    {
                        Name = BookmarkTitleForSave,
                        Url = BookmarkUrlForSave,
                        FaviconUrl = Constants.Constants.FAVICONS_SERVICE + BookmarkUrlForSave
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

        private async void OnDeleteBookmarkCommandExecuted()
        {
            var result = await _dataTransferService.RemoveBookmark(new BookmarkDetails
            {
                Name = BookmarkTitleForSave,
                Url = BookmarkUrlForSave
            });
            SetBookmarkIconState(!result);
            IsFlyoutClosed = result;
        }

        private void OnAddBookmarkButtonCommandExecuted()
        {
            if (SelectedWebView2 == null) return;
            BookmarkTitleForSave = SelectedWebView2.CoreWebView2.DocumentTitle;
            BookmarkUrlForSave = SelectedWebView2.Source.AbsoluteUri;
        }

        private void OnBookmarkSettingButtonExecuted()
        {
            CreateSettingsTab(3);
            IsFlyoutClosed = true;
        }

        private void OnBookmarksFlyoutListViewItemClickExecuted(object sender)
        {
            if (sender is not ItemClickEventArgs { ClickedItem: BookmarkDetails selectedBookmarkItem }) return;
            CreateNewWebTab(selectedBookmarkItem.Url);
            IsFlyoutClosed = true;
        }

        private void OnHistorySettingsButtonExecuted()
        {
            CreateSettingsTab(5);
            IsFlyoutClosed = true;
        }

        private void OnAddTabButtonCommandExecuted() => CreateStartPageTab();

        private void OnCloseTabButtonCommandExecuted(object sender)
        {
            if (sender is winUI.TabViewTabCloseRequestedEventArgs tab)
                CloseTabItemRequested(tab.Tab);
        }

        #endregion On Command Executed Region

        private readonly IDataTransferService _dataTransferService;
        private readonly IWebView2Service _webView2Service;
        private readonly ILocalSettingsService _localSettingsService;

        public MainPageViewModel(IDataTransferService dataTransferService, IWebView2Service webView2Service, ILocalSettingsService localSettingsService)
        {
            _dataTransferService = dataTransferService;
            _webView2Service = webView2Service;
            _localSettingsService = localSettingsService;

            _webView2Service.NavigationStarting += WebViewOnNavigationStarting;
            _webView2Service.NewWindowRequested += WebViewOnNewWindowRequested;
            _webView2Service.NavigationCompleted += WebViewOnNavigationCompleted;

            InitializePageComponents();
            CreateNewWebTab();
            InitializeCommands();
        }

        private async void InitializePageComponents()
        {
            TabViewItemsList = new ObservableCollection<winUI.TabViewItem>();
            VisibilityHomeButton = await _localSettingsService.ReadSettingAsync<bool>("IsHomeButtonEnabled");
        }

        private void InitializeCommands()
        {
            BackButtonCommand = new DelegateCommand(OnBackButtonCommandExecuted, () => SelectedWebView2 is { CanGoBack: true });
            ForwardButtonCommand = new DelegateCommand(OnForwardButtonCommandExecuted, () => SelectedWebView2 is { CanGoForward: true });
            ReloadButtonCommand = new DelegateCommand(OnReloadButtonCommandExecuted);
            StopLoadingButtonCommand = new DelegateCommand(OnStopLoadingButtonCommandExecuted);
            HomeButtonCommand = new DelegateCommand(OnHomeButtonCommandExecuted);
            SearchButtonCommand = new DelegateCommand(OnSearchButtonCommandExecuted);
            AddBookmarkButtonCommand = new DelegateCommand(OnAddBookmarkButtonCommandExecuted);
            SaveBookmarkButtonCommand = new DelegateCommand(OnSaveBookmarkCommandExecuted);
            CancelSaveBookmarkButtonCommand = new DelegateCommand(OnCancelSaveBookmarkCommandExecuted);
            DeleteBookmarkButtonCommand = new DelegateCommand(OnDeleteBookmarkCommandExecuted);
            BookmarksButtonCommand = new DelegateCommand(OnBookmarksButtonCommandExecuted);
            BookmarksSettingsButtonCommand = new DelegateCommand(OnBookmarkSettingButtonExecuted);
            BookmarksItemClickCommand = new DelegateCommand<object>(OnBookmarksFlyoutListViewItemClickExecuted);
            SearchBoxTextChangedCommand = new DelegateCommand<object>(OnSearchBoxTextChangedCommandExecuted);
            SearchBoxQuerySubmittedCommand = new DelegateCommand<object>(OnSearchBoxQuerySubmittedCommandExecuted);
            NewsContentButtonCommand = new DelegateCommand(OnNewsContentButtonCommandExecuted);
            HistoryButtonCommand = new DelegateCommand(OnHistoryButtonCommandExecuted);
            HistorySettingsButtonCommand = new DelegateCommand(OnHistorySettingsButtonExecuted);
            HistoryItemClickCommand = new DelegateCommand<object>(OnHistoryFlyoutItemClickCommandExecuted);
            SettingsButtonCommand = new DelegateCommand(OnSettingsButtonCommandExecuted);
            AddTabButtonCommand = new DelegateCommand(OnAddTabButtonCommandExecuted);
            CloseTabButtonCommand = new DelegateCommand<object>(OnCloseTabButtonCommandExecuted);
            DeveloperInstrumentsButtonCommand = new DelegateCommand(OnDeveloperInstrumentsButtonCommandExecuted, () => SelectedWebView2 != null);
            TaskManagerButtonCommand = new DelegateCommand(OnTaskManagerButtonCommandExecuted, () => SelectedWebView2 != null);
        }

        private async Task<IEnumerable<string>> GetSearchTermListAsync()
        {
            var searchTermListTransfer = await _dataTransferService.GetSearchTerm();
            var searchTermListReversed = searchTermListTransfer.Reverse();

            return searchTermListReversed.Select(term => term.Name).ToList();
        }

        private async void AutoSuggestListFill()
        {
            var searchTermList = await GetSearchTermListAsync();

            var enumerable = searchTermList.ToList();
            var suitableItems = from item in enumerable
                                where item.Contains(SearchBoxText, StringComparison.OrdinalIgnoreCase)
                                select item;

            var enumerableList = suitableItems.ToList();

            if (enumerableList.Count == 0)
            {
                enumerableList.Add("Искать в " + App.CurrentWebEngine.Name + " " + SearchBoxText);
            }

            if (SearchBoxText.Length != 0)
            {
                SearchBoxItemsCollection = enumerableList;
                return;
            }

            var recentlySearch = new List<string>();
            if (enumerable.ToList().Count < 10)
            {
                recentlySearch = enumerableList;
            }
            else
            {
                recentlySearch.AddRange(enumerable.GetRange(0, 8));
            }

            suitableItems = recentlySearch;

            SearchBoxItemsCollection = suitableItems.ToList();
        }

        private void CommandsRaiseCanExecuteChanged()
        {
            BackButtonCommand.RaiseCanExecuteChanged();
            ForwardButtonCommand.RaiseCanExecuteChanged();
        }

        private void SetProgressRingActivity(bool isActive) => IsProgressRingActive = isActive;

        private void SetVisualUiElementStates(object sender)
        {
            if (sender is not winUI.WebView2 webInstance)
            {
                SetProgressRingActivity(false);
            }
            else
            {
                var loadingState = (bool)webInstance.Tag;
                SetProgressRingActivity(loadingState);
            }
            SetBookmarkButtonAppearance();
        }

        private void SetVisualUiLabels(string appTitleText, string searchBoxText)
        {
            AppTitleText = appTitleText;
            SearchBoxText = searchBoxText;
        }

        private void WebViewOnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            IsWebLoading = true;
            SetVisualUiElementStates(sender);
            SetVisualUiLabels("LoadingString".GetLocalized(), args.Uri);
        }

        private void WebViewOnNewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            CreateNewWebTab(args.Uri);
        }

        private void WebViewOnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (sender is not winUI.WebView2 webInstance) return;
            var rightTab = TabViewItemsList.SingleOrDefault(tab => tab.Content == webInstance);
            if (rightTab == null) return;

            var faviconUri = new Uri(Constants.Constants.FAVICONS_SERVICE + webInstance.Source);
            rightTab.Header = webInstance.CoreWebView2.DocumentTitle;
            rightTab.IconSource = new winUI.BitmapIconSource
            {
                UriSource = faviconUri,
                ShowAsMonochrome = false
            };

            _dataTransferService.SaveHistory(new HistoryItemDetails
            {
                Name = webInstance.CoreWebView2.DocumentTitle,
                Url = webInstance.Source.AbsoluteUri,
                Time = DateTime.Now.ToLongTimeString(),
                Date = DateTime.Now.ToShortDateString(),
            });

            IsWebLoading = false;

            if (webInstance.Source == null || SelectedWebView2 != sender) return;
            SetVisualUiLabels(webInstance.CoreWebView2.DocumentTitle, webInstance.Source.AbsoluteUri);
            SetVisualUiElementStates(sender);
            CommandsRaiseCanExecuteChanged();
        }

        private static winUI.TabViewItem CreateTabViewItemInstance(string header, object content, winUI.IconSource icon)
        {
            var newTab = new winUI.TabViewItem
            {
                Header = string.IsNullOrWhiteSpace(header) ? "LoadingString".GetLocalized() : header,
                Content = content,
                IconSource = icon,
                IsRightTapEnabled = true,
            };
            return newTab;
        }

        public void NavigateTo(string address, winUI.WebView2 webViewInstance)
        {
            if (webViewInstance == null) return;

            switch (address)
            {
                case Constants.Constants.SETTINGS_ADDRESS:
                    CreateSettingsTab();
                    break;

                case Constants.Constants.STARTPAGE_ADDRESS:
                    CreateStartPageTab();
                    break;

                case Constants.Constants.NEWS_ADDRESS:
                    CreateNewContentTab();
                    break;

                default:
                    webViewInstance.Source = ResolveUri(address);
                    break;
            }
        }

        private static Uri ResolveUri(string address)
        {
            address = address.Trim().ToLower();
            const string PATTERN = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            var rgx = new Regex(PATTERN, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var httpsScheme = string.Concat(Uri.UriSchemeHttps, "://");
            var httpScheme = string.Concat(Uri.UriSchemeHttp, "://");

            if (rgx.IsMatch(address))
            {
                if (!(address.StartsWith(httpScheme) || address.StartsWith(httpsScheme)))
                {
                    address = string.Concat(httpsScheme, address);
                }
            }
            else return new Uri(App.CurrentWebEngine.Prefix + address);

            var isUriCreated = Uri.TryCreate(address, UriKind.Absolute, out var uriAddress) &&
                               (uriAddress.Scheme == Uri.UriSchemeHttp ||
                                uriAddress.Scheme == Uri.UriSchemeHttps ||
                                uriAddress.Scheme == Uri.UriSchemeFtp);

            return isUriCreated ? uriAddress : new Uri(App.CurrentWebEngine.Prefix + address);
        }

        public async void CreateNewWebTab(string url = null)
        {
            var newWebView = await _webView2Service.InstantiateWebView2(string.IsNullOrWhiteSpace(url) ?
                App.CurrentWebEngine.HomePage :
                ResolveUri(url).ToString());

            var newTab = CreateTabViewItemInstance(
                newWebView.CoreWebView2.DocumentTitle,
                newWebView,
                new winUI.SymbolIconSource() { Symbol = Symbol.More });

            TabViewItemsList.Add(newTab);
            SelectedTabItem = newTab;
        }

        public void CreateStartPageTab()
        {
            var startPageTab = CreateTabViewItemInstance(
                "NewTab".GetLocalized(),
                new StartPage(),
                new winUI.FontIconSource
                {
                    Glyph = "\xE737"
                });

            TabViewItemsList.Add(startPageTab);
            SelectedTabItem = startPageTab;
        }

        public void CreateSettingsTab(int mode = 0)
        {
            var alreadyExistsSettingsTab = TabViewItemsList.FirstOrDefault(tab => tab.Content is SettingsPage);

            if (alreadyExistsSettingsTab != null)
            {
                SelectedTabItem = alreadyExistsSettingsTab;
                return;
            }

            var settingsTab = CreateTabViewItemInstance(
                "Settings".GetLocalized(),
                new SettingsPage(mode),
                new winUI.SymbolIconSource { Symbol = Symbol.Setting });

            TabViewItemsList.Add(settingsTab);
            SelectedTabItem = settingsTab;
        }

        public async void SearchWebFromStartPage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            var webViewInstance = await _webView2Service.InstantiateWebView2(ResolveUri(url).ToString());

            var newTab = CreateTabViewItemInstance(
                webViewInstance.CoreWebView2.DocumentTitle,
                webViewInstance,
                new winUI.SymbolIconSource()
                {
                    Symbol = Symbol.More
                });

            TabViewItemsList[TabViewItemsList.IndexOf(SelectedTabItem)] = newTab;
            SelectedTabItem = newTab;
        }

        private void SelectionChangedTabHandler()
        {
            if (SelectedTabItem == null)
            {
                SetVisualUiLabels(null, null);
                SetVisualUiElementStates(null);
                return;
            }
            SelectedWebView2 = SelectedTabItem.Content as winUI.WebView2;

            switch (SelectedTabItem.Content)
            {
                case SettingsPage:
                    SetVisualUiLabels(SelectedTabItem.Header.ToString(), Constants.Constants.SETTINGS_ADDRESS);
                    break;

                case StartPage:
                    SetVisualUiLabels(SelectedTabItem.Header.ToString(), string.Empty);
                    break;

                case winUI.WebView2:
                    if (SelectedWebView2.Source != null)
                        SetVisualUiLabels(SelectedWebView2.CoreWebView2.DocumentTitle, SelectedWebView2.Source.AbsoluteUri);
                    break;

                case NewsPage:
                    SetVisualUiLabels(SelectedTabItem.Header.ToString(), Constants.Constants.NEWS_ADDRESS);
                    break;

                default:
                    SetVisualUiLabels(SelectedTabItem.Header.ToString(), string.Empty);
                    break;
            }
            if (SelectedWebView2 != null)
                IsWebLoading = (bool)SelectedWebView2.Tag;
            SetVisualUiElementStates(SelectedWebView2);

            CommandsRaiseCanExecuteChanged();
        }

        private void CreateNewContentTab()
        {
            var newsTab = CreateTabViewItemInstance(
                "News".GetLocalized(),
                new NewsPage(),
                new winUI.FontIconSource
                {
                    Glyph = "\xE8A1"
                });

            TabViewItemsList.Add(newsTab);
            SelectedTabItem = newsTab;
        }

        private void CloseTabItemRequested(winUI.TabViewItem tab)
        {
            if (tab.Content is winUI.WebView2 webContent)
                webContent.Close();

            TabViewItemsList.Remove(tab);
            if (TabViewItemsList.Count == 0)
                SelectedWebView2 = null;
        }

        private void CloseTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            if (args.Element is not winUI.TabView invokedTabView) return;
            if (!((winUI.TabViewItem)invokedTabView.SelectedItem).IsClosable) return;
            if (invokedTabView.TabItems[invokedTabView.SelectedIndex] is winUI.TabViewItem tabItem)
                CloseTabItemRequested(tabItem);
        }

        private async void GetBookmarksAsync()
        {
            var bookmarksListTransfer = await _dataTransferService.GetBookmarksList();
            var bookmarkDetailsEnumerable = bookmarksListTransfer.Reverse();
            BookmarksList = new ObservableCollection<BookmarkDetails>(bookmarkDetailsEnumerable);
        }

        private void SetBookmarkIconState(bool isAccessable)
        {
            IsBookmarksExists = isAccessable;
            DeleteBookmarkButtonVisibility = isAccessable ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetBookmarkButtonAppearance()
        {
            if (SelectedWebView2 == null ||
                SelectedWebView2.Source == null)
            {
                SetBookmarkIconState(false);
                return;
            }
            GetBookmarksAsync();
            if (BookmarksList == null) return;

            var existableBookmark = BookmarksList.FirstOrDefault(bookmark => bookmark.Url == SelectedWebView2.Source.AbsoluteUri);

            SetBookmarkIconState(existableBookmark != null);
        }
    }
}