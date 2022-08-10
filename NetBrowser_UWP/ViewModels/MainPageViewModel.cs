using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Web.WebView2.Core;
using NetBrowser_UWP.BindingHelpers;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Properties;
using NetBrowser_UWP.Views;
using NetBrowser_UWP.Views.Settings;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private const string FAVICONS_SERVICE = "https://www.google.com/s2/favicons?sz=32&domain_url=";

        //private const string FAVICONS_SERVICE = "https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=";
        private static winUI.TabViewItem _currentSelectedTab;

        private static winUI.WebView2 _currentSelectedWeb;

        private static List<BookmarkDetails> _bookmarksList;
        private static IEnumerable<SiteItem> _searchTermList;
        private static List<HistoryItemDetails> _historyList;

        private static string _appTitleText;
        private static string _searchBoxText;
        private static string _bookmarkTitleForSave;
        private static string _bookmarkUrlForSave;

        private static Visibility _visibilityProgressBar;
        private static Visibility _visibilityDeleteBookmarkButton;
        private static bool _isFlyoutClosed;
        private ObservableCollection<winUI.TabViewItem> _tabViewItemsList;
        private List<SiteItem> _searchBoxItemsCollection;
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

        #endregion Commands Region

        #region Global Properties Region

        public ObservableCollection<winUI.TabViewItem> TabViewItemsList
        {
            get => _tabViewItemsList;
            set => SetProperty(ref _tabViewItemsList, value);
        }

        public winUI.WebView2 CurrentSelectedWebView
        {
            get => _currentSelectedWeb;
            set => SetProperty(ref _currentSelectedWeb, value);
        }

        public List<SiteItem> SearchBoxItemsCollection
        {
            get => _searchBoxItemsCollection;
            set => SetProperty(ref _searchBoxItemsCollection, value);
        }

        public string SearchBoxText
        {
            get => _searchBoxText;
            set => SetProperty(ref _searchBoxText, value);
        }

        public List<BookmarkDetails> BookmarksList
        {
            get => _bookmarksList;
            set => SetProperty(ref _bookmarksList, value);
        }

        public List<HistoryItemDetails> HistoryList
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

        public Visibility ProgressBarVisibility
        {
            get => _visibilityProgressBar;
            set => SetProperty(ref _visibilityProgressBar, value);
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

        public winUI.TabViewItem CurrentSelectedTab
        {
            get => _currentSelectedTab;
            set
            {
                SetProperty(ref _currentSelectedTab, value);
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

        #endregion Global Properties Region

        #region On Command Executed Region

        private void OnBackButtonCommandExecuted()
        {
            if (CurrentSelectedWebView is { CanGoBack: true })
                CurrentSelectedWebView.GoBack();
        }

        private void OnForwardButtonCommandExecuted()
        {
            if (CurrentSelectedWebView is { CanGoForward: true })
                CurrentSelectedWebView.GoForward();
        }

        private void OnReloadButtonCommandExecuted()
        {
            if (CurrentSelectedWebView == null) return;

            //WebViewStates[CurrentSelectedWebView] = true;
            CurrentSelectedWebView.CoreWebView2.Reload();

            SetVisualUiElementStates(CurrentSelectedWebView);
        }

        private void OnStopLoadingButtonCommandExecuted()
        {
            if (CurrentSelectedWebView == null) return;

            //WebViewStates[CurrentSelectedWebView] = false;
            CurrentSelectedWebView.CoreWebView2.Stop();

            SetVisualUiElementStates(CurrentSelectedWebView);
        }

        private void OnHomeButtonCommandExecuted()
        {
            if (App.CurrentWebEngine?.HomePage != null && CurrentSelectedWebView != null)
                NavigateTo(App.CurrentWebEngine.HomePage, CurrentSelectedWebView);
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
            else if (!(string.IsNullOrEmpty(eventArgs.QueryText) || string.IsNullOrWhiteSpace(eventArgs.QueryText)))
                queryForSearch = eventArgs.QueryText;

            if (queryForSearch == string.Empty) return;
            if (CurrentSelectedWebView == null)
            {
                CreateNewWebTab();
            }
            NavigateTo(queryForSearch, CurrentSelectedWebView);
            _dataTransferService.SaveSearchTerm(queryForSearch);
        }

        private void OnSearchButtonCommandExecuted()
        {
            if (string.IsNullOrEmpty(SearchBoxText) || string.IsNullOrWhiteSpace(SearchBoxText)) return;
            NavigateTo(SearchBoxText, CurrentSelectedWebView);
            _dataTransferService.SaveSearchTerm(SearchBoxText);
        }

        private void OnSettingsButtonCommandExecuted() => CreateSettingsTab();

        private void OnDeveloperInstrumentsButtonCommandExecuted() =>
            CurrentSelectedWebView?.CoreWebView2.OpenDevToolsWindow();

        private void OnTaskManagerButtonCommandExecuted() =>
            CurrentSelectedWebView?.CoreWebView2.OpenTaskManagerWindow();

        private async void OnHistoryButtonCommandExecuted()
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

        private void OnBookmarksButtonCommandExecuted() => GetBookmarksAsync();

        private void OnCancelSaveBookmarkCommandExecuted()
        {
            IsFlyoutClosed = true;
        }

        private async void OnSaveBookmarkCommandExecuted()
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
            if (CurrentSelectedWebView == null) return;
            BookmarkTitleForSave = CurrentSelectedWebView.CoreWebView2.DocumentTitle;
            BookmarkUrlForSave = CurrentSelectedWebView.Source.AbsoluteUri;
        }

        private void OnBookmarkSettingButtonExecuted()
        {
            CreateSettingsTab(3);
            IsFlyoutClosed = true;
        }

        private void OnBookmarksFlyoutListViewItemClickExecuted(object sender)
        {
            if (sender is not ItemClickEventArgs objArgs ||
                objArgs.ClickedItem is not BookmarkDetails selectedBookmarkItem) return;
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

        public MainPageViewModel(IDataTransferService dataTransferService, IWebView2Service webView2Service)
        {
            _dataTransferService = dataTransferService;
            _webView2Service = webView2Service;
            TabViewItemsList = new ObservableCollection<winUI.TabViewItem>();

            _webView2Service.NavigationStarting += WebViewOnNavigationStarting;
            _webView2Service.NewWindowRequested += WebViewOnNewWindowRequested;
            _webView2Service.NavigationCompleted += WebViewOnNavigationCompleted;
            GetSearchTermList();
            CreateNewWebTab();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            BackButtonCommand = new DelegateCommand(OnBackButtonCommandExecuted, () => CurrentSelectedWebView is { CanGoBack: true });
            ForwardButtonCommand = new DelegateCommand(OnForwardButtonCommandExecuted, () => CurrentSelectedWebView is { CanGoForward: true });
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
            HistoryButtonCommand = new DelegateCommand(OnHistoryButtonCommandExecuted);
            HistorySettingsButtonCommand = new DelegateCommand(OnHistorySettingsButtonExecuted);
            HistoryItemClickCommand = new DelegateCommand<object>(OnHistoryFlyoutItemClickCommandExecuted);
            SettingsButtonCommand = new DelegateCommand(OnSettingsButtonCommandExecuted);
            AddTabButtonCommand = new DelegateCommand(OnAddTabButtonCommandExecuted);
            CloseTabButtonCommand = new DelegateCommand<object>(OnCloseTabButtonCommandExecuted);
            DeveloperInstrumentsButtonCommand = new DelegateCommand(OnDeveloperInstrumentsButtonCommandExecuted, () => CurrentSelectedWebView != null);
            TaskManagerButtonCommand = new DelegateCommand(OnTaskManagerButtonCommandExecuted, () => CurrentSelectedWebView != null);
        }

        private async void GetSearchTermList()
        {
            var searchTermListTransfer = await _dataTransferService.GetSearchTerm();
            if (searchTermListTransfer == null) return;
            searchTermListTransfer.Reverse();
            _searchTermList = new HashSet<SiteItem>(searchTermListTransfer);
        }

        private static List<SiteItem> AutoSuggestListFill(string suggestBoxText)
        {
            var suitableItems = from item in _searchTermList
                                where item.Name.ToLower().Contains(suggestBoxText.ToLower())
                                select item;

            var enumerableList = suitableItems.ToList();
            if (enumerableList.Count == 0)
            {
                enumerableList.Add(new SiteItem
                {
                    Name = "Искать в " + App.CurrentWebEngine.Name + " " + suggestBoxText
                });
            }

            if (!(string.IsNullOrEmpty(suggestBoxText) ||
                  string.IsNullOrWhiteSpace(suggestBoxText)))
                return enumerableList;

            var recentlySearch = new List<SiteItem>();
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

        private void CommandsRaiseCanExecuteChanged()
        {
            BackButtonCommand.RaiseCanExecuteChanged();
            ForwardButtonCommand.RaiseCanExecuteChanged();
        }

        private void SetProgressBarStatus(bool isEnabled)
        {
            ProgressBarVisibility = isEnabled ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetRefreshButtonIconState(bool isLoading)
        {
            IsWebLoading = isLoading;
        }

        private void SetVisualUiElementStates(object sender)
        {
            var webInstance = sender as winUI.WebView2;
            if (webInstance == null)
            {
                SetProgressBarStatus(false);
                SetRefreshButtonIconState(false);
            }
            else
            {
                var loadingState = (bool) webInstance.Tag;
                SetProgressBarStatus(loadingState);
                SetRefreshButtonIconState(loadingState);
            }
            SetBookmarkButtonAppearance();
        }

        private void SetVisualUiLabels(string appTitleText, string searchBoxText = null)
        {
            AppTitleText = ResourceExtensions.GetLocalized("AppDisplayName") + " | " + appTitleText;
            SearchBoxText = searchBoxText;
        }


        private void WebViewOnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
           
            SetVisualUiElementStates(sender);
            IsWebLoading = true;
            SetVisualUiLabels(ResourceExtensions.GetLocalized("LoadingString"), args.Uri);
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

            var faviconUri = new Uri(FAVICONS_SERVICE + webInstance.Source);
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

            webInstance.Tag = false;
            IsWebLoading = false;

            if (webInstance.Source == null || CurrentSelectedWebView != sender) return;
            SetVisualUiLabels(webInstance.CoreWebView2.DocumentTitle, webInstance.Source.AbsoluteUri);
            SetVisualUiElementStates(sender);
            CommandsRaiseCanExecuteChanged();
        }

        private winUI.TabViewItem CreateTabViewItemInstance(string header, object content, winUI.IconSource icon)
        {
            var newTab = new winUI.TabViewItem
            {
                Header = string.IsNullOrEmpty(header) ? ResourceExtensions.GetLocalized("LoadingString") : header,
                Content = content,
                IconSource = icon,
                IsRightTapEnabled = true
            };
            return newTab;
        }

        public void NavigateTo(string address, winUI.WebView2 webViewInstance)
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
                    webViewInstance.Source = address.StartsWith("https://") || address.StartsWith("http://") ?
                        new Uri(address) :
                        new Uri(App.CurrentWebEngine.Prefix + address);
                    break;
            }
        }

        public async void CreateNewWebTab(string url = null)
        {
            var newWebView = await _webView2Service.InstantiateWebView2(string.IsNullOrEmpty(url) ? App.CurrentWebEngine.HomePage : url);
            var newTab = CreateTabViewItemInstance(
                newWebView.CoreWebView2.DocumentTitle,
                newWebView,
                new winUI.SymbolIconSource() { Symbol = Symbol.More });

            TabViewItemsList.Add(newTab);
            CurrentSelectedTab = newTab;
        }

        public void CreateStartPageTab()
        {
            var startPageTab = CreateTabViewItemInstance(
                ResourceExtensions.GetLocalized("NewTabTitle"),
                new StartPage(),
                new winUI.SymbolIconSource { Symbol = Symbol.NewWindow });

            TabViewItemsList.Add(startPageTab);
            CurrentSelectedTab = startPageTab;
        }

        public void CreateSettingsTab(int mode = 0)
        {
            var settingsTab = CreateTabViewItemInstance(
                ResourceExtensions.GetLocalized("SettingsText"),
                new SettingsPage(mode),
                new winUI.SymbolIconSource { Symbol = Symbol.Setting });

            TabViewItemsList.Add(settingsTab);
            CurrentSelectedTab = settingsTab;
        }

        public async void SearchWebFromStartPage(string url)
        {
            var webViewInstance = await _webView2Service.InstantiateWebView2(url);
            var newTab = CreateTabViewItemInstance(
                webViewInstance.CoreWebView2.DocumentTitle,
                webViewInstance,
                new winUI.SymbolIconSource() { Symbol = Symbol.More });

            TabViewItemsList[TabViewItemsList.IndexOf(CurrentSelectedTab)] = newTab;
            CurrentSelectedTab = newTab;
        }

        private void SelectionChangedTabHandler()
        {
            if (CurrentSelectedTab == null)
            {
                SetVisualUiLabels(string.Empty, string.Empty);
                SetVisualUiElementStates(null);
                return;
            }
            CurrentSelectedWebView = CurrentSelectedTab.Content as winUI.WebView2;

            switch (CurrentSelectedTab.Content)
            {
                case SettingsPage:
                    SetVisualUiLabels(CurrentSelectedTab.Header.ToString(), "app://settings");
                    break;

                case StartPage:
                    SetVisualUiLabels(CurrentSelectedTab.Header.ToString(), string.Empty);
                    break;

                case winUI.WebView2:
                    if (CurrentSelectedWebView.Source != null)
                        SetVisualUiLabels(CurrentSelectedWebView.CoreWebView2.DocumentTitle, CurrentSelectedWebView.Source.AbsoluteUri);
                    break;

                default:
                    SetVisualUiLabels(string.Empty, string.Empty);
                    break;
            }
            SetVisualUiElementStates(CurrentSelectedWebView);
            CommandsRaiseCanExecuteChanged();
        }

        private void CloseTabItemRequested(winUI.TabViewItem tab)
        {
            if (tab.Content is winUI.WebView2 webContent)
                webContent.Close();

            TabViewItemsList.Remove(tab);
            if (TabViewItemsList.Count == 0) CurrentSelectedWebView = null;
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
            var bookmarksListTransfer = await _dataTransferService.GetBookmarkList();
            bookmarksListTransfer.Reverse();
            BookmarksList = bookmarksListTransfer;
        }

        private void SetBookmarkIconState(bool isAccessable)
        {
            IsBookmarksExists = isAccessable;
            DeleteBookmarkButtonVisibility = isAccessable ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetBookmarkButtonAppearance()
        {
            if (CurrentSelectedWebView == null || CurrentSelectedWebView.Source == null)
            {
                SetBookmarkIconState(false);
                return;
            }
            GetBookmarksAsync();
            if (BookmarksList == null) return;

            var existableBookmark = BookmarksList.SingleOrDefault(bookmark => bookmark.Url == CurrentSelectedWebView.Source.AbsoluteUri);

            SetBookmarkIconState(existableBookmark != null);
        }
    }
}