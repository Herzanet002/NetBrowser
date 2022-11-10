using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using winUI = Microsoft.UI.Xaml.Controls;

namespace NetBrowser_UWP.ViewModels
{
    public class ShellPageViewModel : ObservableObject
    {
        #region Private Global Element Region

        private ObservableCollection<BookmarkDetails> _bookmarksList;
        private IList<HistoryItemDetails> _historyList;

        private IList<string> _searchBoxItemsCollection;
        private string _bookmarkTitleForSave;
        private string _bookmarkUrlForSave;

        private bool _isWebLoading;

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

        public IList<string> SearchBoxItemsCollection
        {
            get => _searchBoxItemsCollection;
            set => SetProperty(ref _searchBoxItemsCollection, value);
        }

        public ObservableCollection<BookmarkDetails> BookmarksList
        {
            get => _bookmarksList;
            set => SetProperty(ref _bookmarksList, value);
        }

        public string AppTitleText => _visualElementsService.AppTitleText;
        public string SearchBoxText => _visualElementsService.SearchBoxText;
        public bool VisibilityHomeButton => _visualElementsService.VisibilityHomeButton;
        public bool IsFlyoutClosed => _visualElementsService.IsFlyoutClosed;
        public bool IsProgressRingActive => _visualElementsService.IsProgressRingActive;
        public bool IsBookmarksExists => _visualElementsService.IsBookmarksExists;
        public Visibility DeleteBookmarkButtonVisibility => _visualElementsService.DeleteBookmarkButtonVisibility;


        public IList<HistoryItemDetails> HistoryList
        {
            get => _historyList;
            set => SetProperty(ref _historyList, value);
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

        public bool IsWebLoading
        {
            get => _isWebLoading;
            set => SetProperty(ref _isWebLoading, value);
        }


        #endregion Global Properties Region

        #region On Command Executed Region

        private void OnBackButtonCommandExecuted()
        {
            if (_tabViewService.GetSelectedWebView() is { CanGoBack: true })
                _tabViewService.GetSelectedWebView().GoBack();
        }

        private void OnForwardButtonCommandExecuted()
        {
            if (_tabViewService.GetSelectedWebView() is { CanGoForward: true })
                _tabViewService.GetSelectedWebView().GoForward();
        }

        private void OnReloadButtonCommandExecuted()
        {
            _tabViewService.GetSelectedWebView()?.CoreWebView2.Reload();
        }

        private void OnStopLoadingButtonCommandExecuted()
        {
            _tabViewService.GetSelectedWebView()?.CoreWebView2.Stop();
        }

        private void OnNewsContentButtonCommandExecuted()
        {
            _tabViewService.CreateNewContentTab();
        }

        private void OnHomeButtonCommandExecuted()
        {
            if (App.CurrentWebEngine?.HomePage != null
                && _tabViewService.GetSelectedWebView() != null)
            {
                NavigateTo(App.CurrentWebEngine.HomePage, _tabViewService.GetSelectedWebView());
            }
        }

        private async void OnSearchBoxTextChangedCommandExecuted(object obj)
        {
            if (obj is not AutoSuggestBoxTextChangedEventArgs eventArgs) return;
            if (eventArgs.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                await AutoSuggestListFill();
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
            if (_tabViewService.GetSelectedWebView() == null) return;

            NavigateTo(queryForSearch, _tabViewService.GetSelectedWebView());
            _dataTransferService.SaveSearchTerm(new SiteItem
            {
                Name = queryForSearch
            });
        }

        private void OnSearchButtonCommandExecuted()
        {
            if (string.IsNullOrWhiteSpace(SearchBoxText)) return;
            NavigateTo(SearchBoxText, _tabViewService.GetSelectedWebView());
            _dataTransferService.SaveSearchTerm(new SiteItem
            {
                Name = SearchBoxText
            });
        }

        private void OnSettingsButtonCommandExecuted() => _tabViewService.CreateSettingsTab();

        private void OnDeveloperInstrumentsButtonCommandExecuted() =>
            _tabViewService.GetSelectedWebView()?.CoreWebView2.OpenDevToolsWindow();

        private void OnTaskManagerButtonCommandExecuted() =>
            _tabViewService.GetSelectedWebView()?.CoreWebView2.OpenTaskManagerWindow();

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
                _tabViewService.CreateNewWebTab();
                if (url != null)
                    NavigateTo(url, _tabViewService.GetSelectedWebView());
            }

            _visualElementsService.SetFlyoutClosedState(true);
        }

        private async void OnBookmarksButtonCommandExecuted() => await GetBookmarksAsync();

        private void OnCancelSaveBookmarkCommandExecuted() => _visualElementsService.SetFlyoutClosedState(true);

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
                _visualElementsService.SetFlyoutClosedState(true);
                _visualElementsService.SetBookmarkIconState(true);
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

            await GetBookmarksAsync();
        }

        private async void OnDeleteBookmarkCommandExecuted()
        {
            var result = await _dataTransferService.RemoveBookmark(new BookmarkDetails
            {
                Name = BookmarkTitleForSave,
                Url = BookmarkUrlForSave
            });
            _visualElementsService.SetBookmarkIconState(!result);
            _visualElementsService.SetFlyoutClosedState(result);
            await GetBookmarksAsync();
        }

        private void OnAddBookmarkButtonCommandExecuted()
        {
            if (_tabViewService.GetSelectedWebView() == null) return;
            BookmarkTitleForSave = _tabViewService.GetSelectedWebView().CoreWebView2.DocumentTitle;
            BookmarkUrlForSave = _tabViewService.GetSelectedWebView().Source.AbsoluteUri;
        }

        private void OnBookmarkSettingButtonExecuted()
        {
            _tabViewService.CreateSettingsTab(3);
            _visualElementsService.SetFlyoutClosedState(true);
        }

        private void OnBookmarksFlyoutListViewItemClickExecuted(object sender)
        {
            if (sender is not ItemClickEventArgs { ClickedItem: BookmarkDetails selectedBookmarkItem }) return;
            _tabViewService.CreateNewWebTab(selectedBookmarkItem.Url);
            _visualElementsService.SetFlyoutClosedState(true);
        }

        private void OnHistorySettingsButtonExecuted()
        {
            _tabViewService.CreateSettingsTab(5);
            _visualElementsService.SetFlyoutClosedState(true);
        }

        private void OnAddTabButtonCommandExecuted() => _tabViewService.CreateStartPageTab();

        private void OnCloseTabButtonCommandExecuted(object sender)
        {
            if (sender is TabViewTabCloseRequestedEventArgs tab)
                CloseTabItemRequested(tab.Tab);
        }

        #endregion On Command Executed Region

        public ObservableCollection<TabViewItem> TabViewItemsList => _tabViewService.GetAllTabItems();

        public TabViewItem SelectedTabItem
        {
            get => _tabViewService.GetSelectedTabItem();
            set => _tabViewService.ChangeSelectedTabItem(value);
        }

        private readonly IDataTransferService _dataTransferService;
        private readonly IWebView2Service _webView2Service;
        private readonly TabViewService _tabViewService;
        private readonly VisualElementsService _visualElementsService;

        public ShellPageViewModel(IDataTransferService dataTransferService,
            IWebView2Service webView2Service,
            TabViewService tabViewService,
            VisualElementsService visualElementsService)
        {
            _dataTransferService = dataTransferService;
            _webView2Service = webView2Service;
            _tabViewService = tabViewService;
            _visualElementsService = visualElementsService;

            InitializeStorage();

            SetEventHandlers();
            _tabViewService.CreateNewWebTab();
            InitializeCommands();
        }

        private async void InitializeStorage()
        {
            await GetBookmarksAsync();
        }

        private void SetEventHandlers()
        {
            _visualElementsService.PropertyChanged += VisualElementsServiceOnPropertyChanged;
            _tabViewService.PropertyChanged += TabViewServiceOnPropertyChanged;
            _tabViewService.SelectionChangedHandler += TabViewServiceSelectionChangedHandler;
            _webView2Service.NavigationStarting += WebViewOnNavigationStarting;
            _webView2Service.NewWindowRequested += WebViewOnNewWindowRequested;
            _webView2Service.NavigationCompleted += WebViewOnNavigationCompleted;
        }

        private void VisualElementsServiceOnPropertyChanged(object sender, PropertyChangedEventArgs e) =>
            OnPropertyChanged(e);

        private void TabViewServiceSelectionChangedHandler(object sender, SelectionChangedEventHandler e)
        {
            IsWebLoading = _tabViewService.GetSelectedWebView() != null && (bool)_tabViewService.GetSelectedWebView().Tag;
            CommandsRaiseCanExecuteChanged();
        }


        private void TabViewServiceOnPropertyChanged(object sender, PropertyChangedEventArgs e) =>
            OnPropertyChanged(e);

        private void InitializeCommands()
        {
            BackButtonCommand = new DelegateCommand(OnBackButtonCommandExecuted, () => _tabViewService.GetSelectedWebView() is { CanGoBack: true });
            ForwardButtonCommand = new DelegateCommand(OnForwardButtonCommandExecuted, () => _tabViewService.GetSelectedWebView() is { CanGoForward: true });
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
            DeveloperInstrumentsButtonCommand = new DelegateCommand(OnDeveloperInstrumentsButtonCommandExecuted, () => _tabViewService.GetSelectedWebView() != null);
            TaskManagerButtonCommand = new DelegateCommand(OnTaskManagerButtonCommandExecuted, () => _tabViewService.GetSelectedWebView() != null);
        }

        private async Task<IEnumerable<string>> GetSearchTermListAsync()
        {
            var searchTermListTransfer = await _dataTransferService.GetSearchTerm();
            var searchTermListReversed = searchTermListTransfer.Reverse();

            return searchTermListReversed.Select(term => term.Name).ToHashSet();
        }

        private async Task AutoSuggestListFill()
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

        //TODO: To service
        private void CommandsRaiseCanExecuteChanged()
        {
            BackButtonCommand.RaiseCanExecuteChanged();
            ForwardButtonCommand.RaiseCanExecuteChanged();
        }

        private void WebViewOnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            if (sender is not WebView2 webInstance) return;
            IsWebLoading = true;
            _visualElementsService.SetVisualUiLabels("LoadingString".GetLocalized(), args.Uri);
            _visualElementsService.SetProgressRingActivity((bool)(webInstance).Tag);
            _visualElementsService.SetBookmarkButtonAppearance(webInstance, BookmarksList);
        }

        private void WebViewOnNewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            _tabViewService.CreateNewWebTab(args.Uri);
        }

        private async void WebViewOnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (sender is not WebView2 webInstance) return;
            var rightTab = _tabViewService.GetTabItemByFilter(tab => tab.Content == webInstance);
            if (rightTab == null) return;

            var faviconUri = new Uri(Constants.Constants.FAVICONS_SERVICE + webInstance.Source);
            rightTab.Header = webInstance.CoreWebView2.DocumentTitle;
            rightTab.IconSource = new winUI.BitmapIconSource
            {
                UriSource = faviconUri,
                ShowAsMonochrome = false
            };

            await _dataTransferService.SaveHistory(new HistoryItemDetails
            {
                Name = webInstance.CoreWebView2.DocumentTitle,
                Url = webInstance.Source.AbsoluteUri,
                Time = DateTime.Now.ToLongTimeString(),
                Date = DateTime.Now.ToShortDateString(),
            });

            IsWebLoading = false;

            if (webInstance.Source == null || _tabViewService.GetSelectedWebView() != sender) return;
            _visualElementsService.SetVisualUiLabels(webInstance.CoreWebView2.DocumentTitle, webInstance.Source.AbsoluteUri);
            _visualElementsService.SetProgressRingActivity((bool)(webInstance).Tag);
            _visualElementsService.SetBookmarkButtonAppearance(webInstance, BookmarksList);
            CommandsRaiseCanExecuteChanged();
        }

        public void NavigateTo(string address, WebView2 webViewInstance)
        {
            if (webViewInstance == null) return;

            switch (address)
            {
                case Constants.Constants.SETTINGS_ADDRESS:
                    _tabViewService.CreateSettingsTab();
                    break;

                case Constants.Constants.STARTPAGE_ADDRESS:
                    _tabViewService.CreateStartPageTab();
                    break;

                case Constants.Constants.NEWS_ADDRESS:
                    _tabViewService.CreateNewContentTab();
                    break;

                default:
                    webViewInstance.Source = _webView2Service.ResolveUri(address);
                    break;
            }
        }

        public async Task SearchWebFromStartPage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;
            var webViewInstance = await _webView2Service.InstantiateWebView2(_webView2Service.ResolveUri(url).ToString());
            var newTab = _tabViewService.CreateTabViewItemInstance(
                webViewInstance.CoreWebView2.DocumentTitle,
                webViewInstance,
                new winUI.SymbolIconSource()
                {
                    Symbol = Symbol.More
                });

            _tabViewService.ChangeTabItem(_tabViewService.GetSelectedTabItem(), newTab);
            _tabViewService.ChangeSelectedTabItem(newTab);
        }



        private void CloseTabItemRequested(TabViewItem tab)
        {
            if (tab.Content is WebView2 webContent)
                webContent.Close();

            _tabViewService.RemoveTabItem(tab);
            if (_tabViewService.GetTabItemsCount() == 0)
                _tabViewService.ChangeSelectedWebView(null);
        }

        private async Task GetBookmarksAsync()
        {
            var bookmarksListTransfer = await _dataTransferService.GetBookmarksList();
            var bookmarkDetailsEnumerable = bookmarksListTransfer.Reverse();
            BookmarksList = new ObservableCollection<BookmarkDetails>(bookmarkDetailsEnumerable);
        }

    }
}