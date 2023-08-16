using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Uwp;
using Microsoft.Web.WebView2.Core;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Messages;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser_UWP.ViewModels.Controls;
using NetBrowser_UWP.Views;
using NetBrowser_UWP.Views.News;
using NetBrowser_UWP.Views.Settings;
using Prism.Commands;
using winUI = Microsoft.UI.Xaml.Controls;

namespace NetBrowser_UWP.ViewModels;

public class ShellPageViewModel : BindableBase
{
    private readonly IDataService _dataService;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly TabViewService _tabViewService;
    private readonly IWebView2Service _webView2Service;

    public ShellPageViewModel(IDataService dataService,
        IWebView2Service webView2Service,
        ILocalSettingsService localSettingsService,
        TabViewService tabViewService)
    {
        _dataService = dataService;
        _webView2Service = webView2Service;
        _localSettingsService = localSettingsService;
        _tabViewService = tabViewService;
        SetEventHandlers();
        InitializeCommands();
    }

    public ObservableCollection<winUI.TabViewItem> TabViewItemsList => _tabViewService.GetAllTabItems();

    public winUI.TabViewItem SelectedTabItem
    {
        get => _tabViewService.GetSelectedTabItem();
        set => _tabViewService.ChangeSelectedTabItem(value);
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
        _webView2Service.ContainsFullScreenElementChanged += WebViewOnContainsFullScreenElementChanged;
    }

    private void WebViewOnContainsFullScreenElementChanged(CoreWebView2 sender, object args)
    {
        var applicationView = ApplicationView.GetForCurrentView();

        if (sender.ContainsFullScreenElement)
        {
            var t = applicationView.TryEnterFullScreenMode();
        }
        else if (applicationView.IsFullScreenMode)
        {
            applicationView.ExitFullScreenMode();
        }
    }

    private void TabViewServiceSelectionChangedHandler(object sender, SelectionChangedEventHandler e)
    {
        SelectionChangedTabHandler();
        if (_tabViewService.GetSelectedWebView() != null)
            IsWebLoading = (bool)_tabViewService.GetSelectedWebView().Tag;
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
            () => _tabViewService.GetSelectedWebView() is { CanGoBack: true });
        ForwardButtonCommand = new DelegateCommand(OnForwardButtonCommandExecuted,
            () => _tabViewService.GetSelectedWebView() is { CanGoForward: true });
        ReloadButtonCommand = new DelegateCommand(OnReloadButtonCommandExecuted);
        StopLoadingButtonCommand = new DelegateCommand(OnStopLoadingButtonCommandExecuted);
        HomeButtonCommand = new DelegateCommand(OnHomeButtonCommandExecuted);
        BookmarksButtonCommand = new AsyncRelayCommand(OnBookmarksButtonCommandExecuted);
        BookmarksSettingsButtonCommand = new DelegateCommand(OnBookmarkSettingButtonExecuted);
        BookmarksItemClickCommand =
            new AsyncRelayCommand<ItemClickEventArgs>(OnBookmarksFlyoutListViewItemClickExecuted);
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
            var loadingState = (bool)webInstance.Tag;
            SetProgressRingActivity(loadingState);
        }

        Messenger.Send(new FindBoxSetBookmarkButtonAppearanceMessage());
    }

    private void SetVisualUiLabels(string appTitleText, string searchBoxText)
    {
        AppTitleText = appTitleText;
        Messenger.Send(new FindBoxQueryChangedMessage(searchBoxText));
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

        var faviconUri = new Uri(Constants.ApplicationConstants.FAVICONS_SERVICE + webInstance.Source);
        rightTab.Header = webInstance.CoreWebView2.DocumentTitle;
        rightTab.IconSource = new winUI.BitmapIconSource
        {
            UriSource = faviconUri,
            ShowAsMonochrome = false
        };

        _dataService.SaveHistoryAsync(new HistoryItem
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
                    Constants.ApplicationConstants.SETTINGS_ADDRESS);
                break;

            case StartPage:
                SetVisualUiLabels(_tabViewService.GetSelectedTabItem().Header.ToString(), string.Empty);
                break;

            case winUI.WebView2:
                if (_tabViewService.GetSelectedWebView()?.Source != null)
                    SetVisualUiLabels(_tabViewService.GetSelectedWebView().CoreWebView2.DocumentTitle,
                        _tabViewService.GetSelectedWebView().Source.AbsoluteUri);
                break;

            case NewsShellPage:
                SetVisualUiLabels(_tabViewService.GetSelectedTabItem().Header.ToString(),
                    Constants.ApplicationConstants.NEWS_ADDRESS);
                break;

            default:
                SetVisualUiLabels(_tabViewService.GetSelectedTabItem().Header.ToString(), string.Empty);
                break;
        }
    }


    private async Task GetBookmarksAsync()
    {
        var bookmarksListTransfer = await _dataService.GetBookmarksAsync();
        bookmarksListTransfer.Reverse();
        BookmarksList = new ObservableCollection<BookmarkItem>(bookmarksListTransfer);
    }

    #region Private Global Element Region

    private ObservableCollection<BookmarkItem> _bookmarksList;
    private IList<HistoryItem> _historyList;

    private string _appTitleText;

    private bool _visibilityHomeButton;
    private bool _isProgressRingActive;
    private bool _isFlyoutClosed;
    private bool _isWebLoading;

    #endregion Private Global Element Region

    #region Commands Region

    public DelegateCommand BackButtonCommand { get; private set; }
    public DelegateCommand ForwardButtonCommand { get; private set; }
    public DelegateCommand ReloadButtonCommand { get; private set; }
    public DelegateCommand StopLoadingButtonCommand { get; private set; }
    public DelegateCommand HomeButtonCommand { get; private set; }
    public IAsyncRelayCommand BookmarksButtonCommand { get; private set; }
    public IAsyncRelayCommand BookmarksItemClickCommand { get; private set; }
    public IAsyncRelayCommand HistoryItemClickCommand { get; private set; }
    public IAsyncRelayCommand HistoryButtonCommand { get; private set; }
    public IAsyncRelayCommand LoadedPageCommand { get; private set; }
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

    public ObservableCollection<BookmarkItem> BookmarksList
    {
        get => _bookmarksList;
        set => SetProperty(ref _bookmarksList, value);
    }

    public IList<HistoryItem> HistoryList
    {
        get => _historyList;
        set => SetProperty(ref _historyList, value);
    }

    public string AppTitleText
    {
        get => _appTitleText;
        set => SetProperty(ref _appTitleText, value);
    }

    public bool IsProgressRingActive
    {
        get => _isProgressRingActive;
        set => SetProperty(ref _isProgressRingActive, value);
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
        _tabViewService.CreateNewsTab();
    }

    private void OnHomeButtonCommandExecuted()
    {
        if (App.CurrentWebEngine?.HomePage != null
            && _tabViewService.GetSelectedWebView() != null)
        {
            Messenger.Send(new FindBoxNavigateToMessage(App.CurrentWebEngine.HomePage));
        }
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
        var historyListTransfer = await _dataService.GetHistoryAsync();

        const int maxDisplayCount = 100;
        var count = historyListTransfer.Count();

        var orderedHistoryList = count <= maxDisplayCount
            ? historyListTransfer.ToList()
            : historyListTransfer.Skip(count - maxDisplayCount).ToList();

        orderedHistoryList.Reverse();

        HistoryList = orderedHistoryList;
    }

    private async Task OnHistoryFlyoutItemClickCommandExecuted(object obj)
    {
        if (obj is not ItemClickEventArgs objArgs) return;
        if (objArgs.ClickedItem is HistoryItem selectedHistoryItem)
        {
            var url = selectedHistoryItem.Url;
            await _tabViewService.CreateNewWebTab();
            if (url != null)
            {
                Messenger.Send(new FindBoxNavigateToMessage(url));
            }
        }

        IsFlyoutClosed = true;
    }

    private async Task OnBookmarksButtonCommandExecuted()
    {
        await GetBookmarksAsync();
    }

    private void OnBookmarkSettingButtonExecuted()
    {
        _tabViewService.CreateSettingsTab(typeof(BookmarksPageSettings));
        IsFlyoutClosed = true;
    }

    private async Task OnBookmarksFlyoutListViewItemClickExecuted(object sender)
    {
        if (sender is not ItemClickEventArgs { ClickedItem: BookmarkItem selectedBookmarkItem }) return;
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
            _tabViewService.CloseTabItemRequested(tab.Tab);
    }

    #endregion On Command Executed Region
}