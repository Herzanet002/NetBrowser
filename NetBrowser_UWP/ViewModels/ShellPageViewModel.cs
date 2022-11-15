using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Web.WebView2.Core;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.Views;
using NetBrowser_UWP.Views.News;
using NetBrowser_UWP.Views.Settings;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using winUI = Microsoft.UI.Xaml.Controls;

namespace NetBrowser_UWP.ViewModels;

public class ShellPageViewModel : ObservableObject
{
    private readonly IDataTransferService _dataTransferService;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly TabViewService _tabViewService;
    private readonly IWebView2Service _webView2Service;


    public ShellPageViewModel(IDataTransferService dataTransferService,
        IWebView2Service webView2Service,
        ILocalSettingsService localSettingsService,
        TabViewService tabViewService)
    {
        _dataTransferService = dataTransferService;
        _webView2Service = webView2Service;
        _localSettingsService = localSettingsService;
        _tabViewService = tabViewService;
        SetEventHandlers();
        //InitializeAsync();

        InitializeCommands();
    }

    private async Task InitializeAsync()
    {
        await InitializePageComponents();
        await GetBookmarksAsync();
        await _tabViewService.CreateNewWebTab().ConfigureAwait(false);
    }

    private void SetEventHandlers()
    {
        _tabViewService.PropertyChanged += TabViewServiceOnPropertyChanged;
        _tabViewService.SelectionChangedHandler += TabViewServiceSelectionChangedHandler;
        _webView2Service.NavigationStarting += WebViewOnNavigationStarting;
        _webView2Service.NewWindowRequested += WebViewOnNewWindowRequested;
        _webView2Service.NavigationCompleted += WebViewOnNavigationCompleted;
    }

    public ObservableCollection<winUI.TabViewItem> TabViewItemsList => _tabViewService.GetAllTabItems();

    public winUI.TabViewItem SelectedTabItem
    {
        get => _tabViewService.GetSelectedTabItem();
        set => _tabViewService.ChangeSelectedTabItem(value);
    }

    private void TabViewServiceSelectionChangedHandler(object sender, SelectionChangedEventHandler e)
    {
        SelectionChangedTabHandler();
        if (_tabViewService.GetSelectedWebView() != null)
            IsWebLoading = (bool) _tabViewService.GetSelectedWebView().Tag;
        SetVisualUiElementStates(_tabViewService.GetSelectedWebView());
        CommandsRaiseCanExecuteChanged();
    }

    private void TabViewServiceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e);
    }

    private async Task InitializePageComponents()
    {
        VisibilityHomeButton = await _localSettingsService.ReadSettingAsync<bool>("IsHomeButtonEnabled");
    }

    private void InitializeCommands()
    {
        LoadedPageCommand = new AsyncRelayCommand(InitializeAsync);
        BackButtonCommand = new DelegateCommand(OnBackButtonCommandExecuted,
            () => _tabViewService.GetSelectedWebView() is {CanGoBack: true});
        ForwardButtonCommand = new DelegateCommand(OnForwardButtonCommandExecuted,
            () => _tabViewService.GetSelectedWebView() is {CanGoForward: true});
        ReloadButtonCommand = new DelegateCommand(OnReloadButtonCommandExecuted);
        StopLoadingButtonCommand = new DelegateCommand(OnStopLoadingButtonCommandExecuted);
        HomeButtonCommand = new DelegateCommand(OnHomeButtonCommandExecuted);
        AddBookmarkButtonCommand = new DelegateCommand(OnAddBookmarkButtonCommandExecuted);
        SaveBookmarkButtonCommand = new AsyncRelayCommand(OnSaveBookmarkCommandExecuted);
        CancelSaveBookmarkButtonCommand = new DelegateCommand(OnCancelSaveBookmarkCommandExecuted);
        DeleteBookmarkButtonCommand = new AsyncRelayCommand(OnDeleteBookmarkCommandExecuted);
        BookmarksButtonCommand = new AsyncRelayCommand(OnBookmarksButtonCommandExecuted);
        BookmarksSettingsButtonCommand = new DelegateCommand(OnBookmarkSettingButtonExecuted);
        BookmarksItemClickCommand =
            new AsyncRelayCommand<ItemClickEventArgs>(OnBookmarksFlyoutListViewItemClickExecuted);
        SearchBoxTextChangedCommand =
            new AsyncRelayCommand<AutoSuggestBoxTextChangedEventArgs>(OnSearchBoxTextChangedCommandExecuted);
        SearchBoxQuerySubmittedCommand =
            new AsyncRelayCommand<AutoSuggestBoxQuerySubmittedEventArgs>(OnSearchBoxQuerySubmittedCommandExecuted);
        NewsContentButtonCommand = new DelegateCommand(OnNewsContentButtonCommandExecuted);
        HistoryButtonCommand = new AsyncRelayCommand(OnHistoryButtonCommandExecuted);
        HistorySettingsButtonCommand = new DelegateCommand(OnHistorySettingsButtonExecuted);
        HistoryItemClickCommand = new AsyncRelayCommand<ItemClickEventArgs>(OnHistoryFlyoutItemClickCommandExecuted);
        SettingsButtonCommand = new DelegateCommand(OnSettingsButtonCommandExecuted);
        AddTabButtonCommand = new DelegateCommand(OnAddTabButtonCommandExecuted);
        CloseTabButtonCommand =
            new DelegateCommand<winUI.TabViewTabCloseRequestedEventArgs>(OnCloseTabButtonCommandExecuted);
        DeveloperInstrumentsButtonCommand = new DelegateCommand(OnDeveloperInstrumentsButtonCommandExecuted,
            () => _tabViewService.GetSelectedWebView() != null);
        TaskManagerButtonCommand = new DelegateCommand(OnTaskManagerButtonCommandExecuted,
            () => _tabViewService.GetSelectedWebView() != null);
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
            enumerableList.Add("Искать в " + App.CurrentWebEngine.Name + " " + SearchBoxText);

        if (SearchBoxText.Length != 0)
        {
            SearchBoxItemsCollection = enumerableList;
            return;
        }

        var recentlySearch = new List<string>();
        if (enumerable.ToList().Count < 10)
            recentlySearch = enumerableList;
        else
            recentlySearch.AddRange(enumerable.GetRange(0, 8));

        suitableItems = recentlySearch;

        SearchBoxItemsCollection = suitableItems.ToList();
    }

    private void CommandsRaiseCanExecuteChanged()
    {
        BackButtonCommand.RaiseCanExecuteChanged();
        ForwardButtonCommand.RaiseCanExecuteChanged();
    }

    private void SetProgressRingActivity(bool isActive)
    {
        IsProgressRingActive = isActive;
    }

    private void SetVisualUiElementStates(object sender)
    {
        if (sender is not winUI.WebView2 webInstance)
        {
            SetProgressRingActivity(false);
        }
        else
        {
            var loadingState = (bool) webInstance.Tag;
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

    private async void WebViewOnNewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs args)
    {
        args.Handled = true;
        await _tabViewService.CreateNewWebTab(args.Uri).ConfigureAwait(false);
    }

    private void WebViewOnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        if (sender is not winUI.WebView2 webInstance) return;
        var rightTab = _tabViewService.GetTabItemByFilter(tab => tab.Content == webInstance);
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
            Date = DateTime.Now.ToShortDateString()
        });

        IsWebLoading = false;

        if (webInstance.Source == null || _tabViewService.GetSelectedWebView() != sender) return;
        SetVisualUiLabels(webInstance.CoreWebView2.DocumentTitle, webInstance.Source.AbsoluteUri);
        SetVisualUiElementStates(sender);
        CommandsRaiseCanExecuteChanged();
    }

    public void NavigateTo(string address, winUI.WebView2 webViewInstance)
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
                _tabViewService.CreateNewsTab();
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
            new winUI.SymbolIconSource
            {
                Symbol = Symbol.More
            });

        _tabViewService.ChangeTabItem(_tabViewService.GetSelectedTabItem(), newTab);
        _tabViewService.ChangeSelectedTabItem(newTab);
    }

    private void SelectionChangedTabHandler()
    {
        if (_tabViewService.GetSelectedTabItem() == null)
        {
            SetVisualUiLabels(null, null);
            SetVisualUiElementStates(null);
            return;
        }

        _tabViewService.ChangeSelectedWebView(_tabViewService.GetSelectedTabItem().Content as winUI.WebView2);

        switch (_tabViewService.GetSelectedTabItem().Content)
        {
            case SettingsPage:
                SetVisualUiLabels(_tabViewService.GetSelectedTabItem().Header.ToString(),
                    Constants.Constants.SETTINGS_ADDRESS);
                break;

            case StartPage:
                SetVisualUiLabels(_tabViewService.GetSelectedTabItem().Header.ToString(), string.Empty);
                break;

            case winUI.WebView2:
                if (_tabViewService.GetSelectedWebView()?.Source != null)
                    SetVisualUiLabels(_tabViewService.GetSelectedWebView().CoreWebView2.DocumentTitle,
                        _tabViewService.GetSelectedWebView().Source.AbsoluteUri);
                break;

            case MainNewsPage:
                SetVisualUiLabels(_tabViewService.GetSelectedTabItem().Header.ToString(),
                    Constants.Constants.NEWS_ADDRESS);
                break;

            default:
                SetVisualUiLabels(_tabViewService.GetSelectedTabItem().Header.ToString(), string.Empty);
                break;
        }
    }

    private void CloseTabItemRequested(winUI.TabViewItem tab)
    {
        if (tab.Content is winUI.WebView2 webContent)
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

    private void SetBookmarkIconState(bool isAccessable)
    {
        IsBookmarksExists = isAccessable;
        DeleteBookmarkButtonVisibility = isAccessable;
    }

    private void SetBookmarkButtonAppearance()
    {
        if (_tabViewService.GetSelectedWebView() == null ||
            _tabViewService.GetSelectedWebView().Source == null)
        {
            SetBookmarkIconState(false);
            return;
        }

        if (BookmarksList == null) return;

        var existableBookmark = BookmarksList.FirstOrDefault(bookmark =>
            bookmark.Url == _tabViewService.GetSelectedWebView().Source.AbsoluteUri);

        SetBookmarkIconState(existableBookmark != null);
    }

    #region Private Global Element Region

    private ObservableCollection<BookmarkDetails> _bookmarksList;
    private IList<HistoryItemDetails> _historyList;

    private IList<string> _searchBoxItemsCollection;

    private string _appTitleText;
    private string _searchBoxText;
    private string _bookmarkTitleForSave;
    private string _bookmarkUrlForSave;

    private bool _visibilityDeleteBookmarkButton;
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

    public IAsyncRelayCommand SaveBookmarkButtonCommand { get; private set; }
    public IAsyncRelayCommand DeleteBookmarkButtonCommand { get; private set; }
    public IAsyncRelayCommand BookmarksButtonCommand { get; private set; }
    public IAsyncRelayCommand BookmarksItemClickCommand { get; private set; }
    public IAsyncRelayCommand SearchBoxTextChangedCommand { get; private set; }
    public IAsyncRelayCommand SearchBoxQuerySubmittedCommand { get; private set; }
    public IAsyncRelayCommand HistoryItemClickCommand { get; private set; }
    public IAsyncRelayCommand HistoryButtonCommand { get; private set; }
    public IAsyncRelayCommand LoadedPageCommand { get; private set; }

    public ICommand AddBookmarkButtonCommand { get; private set; }
    public ICommand CancelSaveBookmarkButtonCommand { get; private set; }
    public ICommand BookmarksSettingsButtonCommand { get; private set; }
    public ICommand HistorySettingsButtonCommand { get; private set; }
    public ICommand SettingsButtonCommand { get; private set; }
    public ICommand AddTabButtonCommand { get; private set; }
    public ICommand CloseTabButtonCommand { get; private set; }
    public ICommand DeveloperInstrumentsButtonCommand { get; private set; }
    public ICommand TaskManagerButtonCommand { get; private set; }
    public ICommand NewsContentButtonCommand { get; private set; }

    #endregion Commands Region

    #region Global Properties Region

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

    public bool DeleteBookmarkButtonVisibility
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
        if (_tabViewService.GetSelectedWebView() is {CanGoBack: true})
            _tabViewService.GetSelectedWebView().GoBack();
    }

    private void OnForwardButtonCommandExecuted()
    {
        if (_tabViewService.GetSelectedWebView() is {CanGoForward: true})
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
        _tabViewService.CreateNewsTab();
    }

    private void OnHomeButtonCommandExecuted()
    {
        if (App.CurrentWebEngine?.HomePage != null
            && _tabViewService.GetSelectedWebView() != null)
            NavigateTo(App.CurrentWebEngine.HomePage, _tabViewService.GetSelectedWebView());
    }

    private async Task OnSearchBoxTextChangedCommandExecuted(object obj)
    {
        if (obj is not AutoSuggestBoxTextChangedEventArgs eventArgs) return;
        if (eventArgs.Reason == AutoSuggestionBoxTextChangeReason.UserInput) await AutoSuggestListFill();
    }

    //TODO: Обновление поисковых запросов
    private async Task OnSearchBoxQuerySubmittedCommandExecuted(object obj)
    {
        if (obj is not AutoSuggestBoxQuerySubmittedEventArgs eventArgs) return;

        var queryForSearch = string.Empty;
        if (!string.IsNullOrWhiteSpace(eventArgs.QueryText))
            queryForSearch = eventArgs.QueryText;

        if (string.IsNullOrWhiteSpace(queryForSearch)) return;
        if (_tabViewService.GetSelectedWebView() == null) return;

        NavigateTo(queryForSearch, _tabViewService.GetSelectedWebView());
        await _dataTransferService.SaveSearchTerm(new SiteItem
        {
            Name = queryForSearch
        }).ConfigureAwait(false);
    }


    private void OnSettingsButtonCommandExecuted()
    {
        _tabViewService.CreateSettingsTab();
    }

    private void OnDeveloperInstrumentsButtonCommandExecuted()
    {
        _tabViewService.GetSelectedWebView()?.CoreWebView2.OpenDevToolsWindow();
    }

    private void OnTaskManagerButtonCommandExecuted()
    {
        _tabViewService.GetSelectedWebView()?.CoreWebView2.OpenTaskManagerWindow();
    }

    private async Task OnHistoryButtonCommandExecuted()
    {
        var historyListTransfer = await _dataTransferService.GetHistory();

        const int MAX_DISPLAY_COUNT = 100;

        HistoryList = historyListTransfer.Count <= MAX_DISPLAY_COUNT
            ? historyListTransfer.Reverse().ToList()
            : historyListTransfer.Skip(Math.Max(0, historyListTransfer.Count() - MAX_DISPLAY_COUNT)).Reverse().ToList();
    }

    private async Task OnHistoryFlyoutItemClickCommandExecuted(object obj)
    {
        if (obj is not ItemClickEventArgs objArgs) return;
        if (objArgs.ClickedItem is HistoryItemDetails selectedHistoryItem)
        {
            var url = selectedHistoryItem.Url;
            await _tabViewService.CreateNewWebTab();
            if (url != null)
                NavigateTo(url, _tabViewService.GetSelectedWebView());
        }

        IsFlyoutClosed = true;
    }

    private async Task OnBookmarksButtonCommandExecuted()
    {
        await GetBookmarksAsync();
    }

    private void OnCancelSaveBookmarkCommandExecuted()
    {
        IsFlyoutClosed = true;
    }

    private async Task OnSaveBookmarkCommandExecuted()
    {
        if (!(string.IsNullOrWhiteSpace(BookmarkTitleForSave) ||
              string.IsNullOrWhiteSpace(BookmarkUrlForSave)) &&
            Uri.IsWellFormedUriString(BookmarkUrlForSave, UriKind.Absolute))
        {
            await _dataTransferService.SaveBookmark(
                new BookmarkDetails
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

    private async Task OnDeleteBookmarkCommandExecuted()
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
        if (_tabViewService.GetSelectedWebView() == null) return;
        BookmarkTitleForSave = _tabViewService.GetSelectedWebView().CoreWebView2.DocumentTitle;
        BookmarkUrlForSave = _tabViewService.GetSelectedWebView().Source.AbsoluteUri;
    }

    private void OnBookmarkSettingButtonExecuted()
    {
        _tabViewService.CreateSettingsTab(typeof(BookmarksPageSettings));
        IsFlyoutClosed = true;
    }

    private async Task OnBookmarksFlyoutListViewItemClickExecuted(object sender)
    {
        if (sender is not ItemClickEventArgs {ClickedItem: BookmarkDetails selectedBookmarkItem}) return;
        await _tabViewService.CreateNewWebTab(selectedBookmarkItem.Url);
        IsFlyoutClosed = true;
    }

    private void OnHistorySettingsButtonExecuted()
    {
        _tabViewService.CreateSettingsTab(typeof(HistoryPageSettings));
        IsFlyoutClosed = true;
    }

    private void OnAddTabButtonCommandExecuted()
    {
        _tabViewService.CreateStartPageTab();
    }

    private void OnCloseTabButtonCommandExecuted(object sender)
    {
        if (sender is winUI.TabViewTabCloseRequestedEventArgs tab)
            CloseTabItemRequested(tab.Tab);
    }

    #endregion On Command Executed Region
}