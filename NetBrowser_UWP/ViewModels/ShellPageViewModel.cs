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
using NetBrowser_UWP.CommandResolver;
using NetBrowser_UWP.Constants;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Contracts.Services.Settings;
using NetBrowser_UWP.Enums;
using NetBrowser_UWP.EventArguments;
using NetBrowser_UWP.Messages;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.UiUpdater;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser_UWP.Views.Settings;
using Prism.Commands;
using winUI = Microsoft.UI.Xaml.Controls;
using NetBrowser_UWP.Contracts;
using NetBrowser_UWP.Views;

namespace NetBrowser_UWP.ViewModels;

public class ShellPageViewModel : BindableBase
{
    private readonly IDataService _dataService;
    private readonly IAppearanceSettingsService _appearanceSettingsService;
    private readonly ITabViewService _tabViewService;
    private readonly ICommandResolver _commandResolver;
    private readonly IPageService _pageService;
    private readonly IWebView2Service _webView2Service;

    public ShellPageViewModel(IDataService dataService,
        IAppearanceSettingsService appearanceSettingsService,
        IWebView2Service webView2Service,
        ITabViewService tabViewService,
        ICommandResolver commandResolver,
        IPageService pageService)
    {
        _dataService = dataService;
        _appearanceSettingsService = appearanceSettingsService;
        _webView2Service = webView2Service;
        _tabViewService = tabViewService;
        _commandResolver = commandResolver;
        _pageService = pageService;
        SetEventHandlers();
        InitializeCommands();
        WeakReferenceMessenger.Default.Register<ShellPageViewModel, InnerPageTypeChangedMessage>(this,
            UpdateUiLabelsByReason);
    }

    private void UpdateUiLabelsByReason(ShellPageViewModel recipient, InnerPageTypeChangedMessage message)
    {
        if (SelectedTabItem != null)
        {
            UpdateUiElementsStateByReason(new InnerPageUpdateUiReasonArgument(message.Value));
        }
    }

    private async Task InitializeAsync()
    {
        IsHomeButtonEnabled = _appearanceSettingsService.IsHomeButtonEnabled.GetSetting();
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
        _appearanceSettingsService.SettingChanged += AppearanceSettingsServiceOnSettingChanged;
    }

    private void AppearanceSettingsServiceOnSettingChanged(object sender, SettingChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsHomeButtonEnabled))
        {
            IsHomeButtonEnabled = (bool)e.NewValue;
        }
    }

    private void WebViewOnContainsFullScreenElementChanged(CoreWebView2 sender, object args)
    {
        var applicationView = ApplicationView.GetForCurrentView();

        if (sender.ContainsFullScreenElement)
        {
            applicationView.TryEnterFullScreenMode();
        }
        else if (applicationView.IsFullScreenMode)
        {
            applicationView.ExitFullScreenMode();
        }
    }

    private void TabViewServiceSelectionChangedHandler(object sender, SelectionChangedEventArgs e)
    {
        var selectedTabItem = _tabViewService.SelectedTabItem;
        if (selectedTabItem == null)
        {
            AppTitleText = string.Empty;
            UpdateFindBoxQueryText(string.Empty);
            UpdateUiElementsStateByReason(new TabViewUpdateUiReasonArgument(null));
            return;
        }

        _tabViewService.SelectedWebView = selectedTabItem.Content as winUI.WebView2;

        var contentBaseType = DeriveBaseTabItemContentType(selectedTabItem.Content);

        if (contentBaseType == typeof(winUI.WebView2))
        {
            AppTitleText = _tabViewService.SelectedWebView?.CoreWebView2.DocumentTitle;
            UpdateFindBoxQueryText(_tabViewService.SelectedWebView?.Source?.AbsoluteUri);
        }

        else if (contentBaseType == typeof(NavigationViewContentPage))
        {
            AppTitleText = selectedTabItem.Header.ToString();
            if (selectedTabItem.Content is NavigationViewContentPage pageCasted)
            {
                var innerPage = _pageService.GetPageInfoByPageType(pageCasted.GetInnerPageType());
                UpdateFindBoxQueryText(innerPage is { PathIsVisible: true }
                    ? innerPage.Path
                    : string.Empty);
            }
        }

        else if (contentBaseType == typeof(Page))
        {
            var contentType = selectedTabItem.Content?.GetType();
            var pageInfo = _pageService.GetPageInfoByPageType(contentType);

            AppTitleText = selectedTabItem.Header.ToString();
            UpdateFindBoxQueryText(pageInfo is { PathIsVisible: true }
                ? pageInfo.Path
                : string.Empty);
        }

        else
        {
            UpdateFindBoxQueryText(string.Empty);
            AppTitleText = selectedTabItem.Header.ToString();
        }

        UpdateUiElementsStateByReason(new TabViewUpdateUiReasonArgument(_tabViewService.SelectedWebView));
        CommandsRaiseCanExecuteChanged();
    }

    private static Type DeriveBaseTabItemContentType(object content)
    {
        var contentType = content.GetType();
        return contentType.BaseType != typeof(Control) ? contentType.BaseType : contentType;
    }

    private void TabViewServiceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        => OnPropertyChanged(e);

    private void InitializeCommands()
    {
        LoadedPageCommand = new AsyncRelayCommand(InitializeAsync);
        BackButtonCommand = new DelegateCommand(OnBackButtonCommandExecuted,
            () => _tabViewService.SelectedWebView is { CanGoBack: true });
        ForwardButtonCommand = new DelegateCommand(OnForwardButtonCommandExecuted,
            () => _tabViewService.SelectedWebView is { CanGoForward: true });
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
            () => _tabViewService.SelectedWebView != null);
        TaskManagerButtonCommand = new DelegateCommand(OnTaskManagerButtonCommandExecuted,
            () => _tabViewService.SelectedWebView != null);
    }

    private void CommandsRaiseCanExecuteChanged()
    {
        BackButtonCommand.RaiseCanExecuteChanged();
        ForwardButtonCommand.RaiseCanExecuteChanged();
    }

    private void UpdateUiElementsStateByReason(UpdateUiReasonArgument reasonArgument)
    {
        if (reasonArgument is InnerPageUpdateUiReasonArgument innerPageUpdateUiReason)
        {
            var pageInfo = _pageService.GetPageInfoByPageType(innerPageUpdateUiReason.PageType);
            Messenger.Send(new FindBoxQueryChangedMessage(pageInfo?.Path));
        }

        if (reasonArgument is WebViewUpdateUiReasonArgument webViewUpdateUiReason)
        {
            IsProgressRingActive = (bool)webViewUpdateUiReason.WebViewInstance.Tag;
            return;
        }

        if (reasonArgument is TabViewUpdateUiReasonArgument tabViewUpdateUiReason)
        {
            if (tabViewUpdateUiReason.Content is winUI.WebView2 webView)
            {
                IsProgressRingActive = (bool)webView.Tag;
                IsWebLoading = (bool)webView.Tag;
                return;
            }

            IsWebLoading = false;
            IsProgressRingActive = false;
            return;
        }

        Messenger.Send(new FindBoxSetBookmarkButtonAppearanceMessage());
    }

    private void UpdateFindBoxQueryText(string queryText)
        => Messenger.Send(new FindBoxQueryChangedMessage(queryText));

    private void WebViewOnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
    {
        IsWebLoading = true;
        UpdateUiElementsStateByReason(new WebViewUpdateUiReasonArgument(sender as winUI.WebView2));
        AppTitleText = "LoadingString".GetLocalized();
        UpdateFindBoxQueryText(args.Uri);
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

        rightTab.Header = webInstance.CoreWebView2.DocumentTitle;
        try
        {
            rightTab.IconSource = new winUI.BitmapIconSource
            {
                UriSource = new Uri(webInstance.CoreWebView2.FaviconUri),
                ShowAsMonochrome = false
            };
        }
        catch (Exception)
        {
            rightTab.IconSource = new winUI.BitmapIconSource
            {
                UriSource = new Uri(ApplicationConstants.FAVICONS_SERVICE + webInstance.Source),
                ShowAsMonochrome = false
            };
        }

        _dataService.SaveHistoryAsync(new HistoryItem
        {
            Name = webInstance.CoreWebView2.DocumentTitle,
            Url = webInstance.Source.AbsoluteUri,
            Time = DateTime.Now.ToLongTimeString(),
            Date = DateTime.Now.ToShortDateString()
        });

        IsWebLoading = false;

        if (webInstance.Source == null || _tabViewService.SelectedWebView != sender) return;
        AppTitleText = webInstance.CoreWebView2.DocumentTitle;
        UpdateFindBoxQueryText(webInstance.Source.AbsoluteUri);
        UpdateUiElementsStateByReason(new WebViewUpdateUiReasonArgument(webInstance));
        CommandsRaiseCanExecuteChanged();
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

    private bool _isHomeButtonEnabled;
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

    public ObservableCollection<winUI.TabViewItem> TabViewItemsList => _tabViewService.ItemsCollection;

    public TabViewPlacementMode TabViewPlacementMode => _appearanceSettingsService.TabViewPlacementMode.GetSetting();

    public winUI.TabViewItem SelectedTabItem
    {
        get => _tabViewService.SelectedTabItem;
        set => _tabViewService.ChangeSelectedTabItem(value);
    }

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

    public bool IsHomeButtonEnabled
    {
        get => _isHomeButtonEnabled;
        set => SetProperty(ref _isHomeButtonEnabled, value);
    }

    #endregion Global Properties Region

    #region On Command Executed Region

    private void OnBackButtonCommandExecuted()
    {
        if (_tabViewService.SelectedWebView is { CanGoBack: true })
            _tabViewService.SelectedWebView.GoBack();
    }

    private void OnForwardButtonCommandExecuted()
    {
        if (_tabViewService.SelectedWebView is { CanGoForward: true })
            _tabViewService.SelectedWebView.GoForward();
    }

    private void OnReloadButtonCommandExecuted()
    {
        _tabViewService.SelectedWebView?.CoreWebView2.Reload();
    }

    private void OnStopLoadingButtonCommandExecuted()
    {
        _tabViewService.SelectedWebView?.CoreWebView2.Stop();
    }

    private void OnNewsContentButtonCommandExecuted()
    {
        _tabViewService.CreateNewsTab();
    }

    private void OnHomeButtonCommandExecuted()
    {
        if (App.CurrentWebEngine?.HomePage != null
            && _tabViewService.SelectedWebView != null)
        {
            _tabViewService.SelectedWebView.Source = new Uri(App.CurrentWebEngine.HomePage);
        }
    }

    private void OnSettingsButtonCommandExecuted()
    {
        _tabViewService.CreateSettingsTab();
    }

    private void OnDeveloperInstrumentsButtonCommandExecuted()
    {
        _tabViewService.SelectedWebView?.CoreWebView2.OpenDevToolsWindow();
    }

    private void OnTaskManagerButtonCommandExecuted()
    {
        _tabViewService.SelectedWebView?.CoreWebView2.OpenTaskManagerWindow();
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
                Messenger.Send(new FindBoxNavigateToMessage(_commandResolver.ResolveCommand(new Command(url))));
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