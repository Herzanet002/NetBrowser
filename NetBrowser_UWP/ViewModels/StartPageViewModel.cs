using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.Views.UserControls;

namespace NetBrowser_UWP.ViewModels;

public class StartPageViewModel : ObservableObject
{
    private readonly IDataTransferService _dataTransferService;
    private readonly TabViewService _tabViewService;
    private readonly ILocalSettingsService _localSettingsService;
    private int _gridViewOrientation;
    private StartPageItem _gridViewSelectedItem;
    private StartPageItem _editableStartPageItem;
    private bool _isAnimationEnabled;
    private bool _isFlyoutClosed;

    private bool _isSuggestionBarEnabled;
    private Uri _logoSource;
    private string _newSiteName;
    private string _newSiteUrl;
    private string _placeholderText;
    private HashSet<SiteItem> _recentlySearchedItems;
    private SearchTermItem _searchBarSelectedItem;
    private string _searchBoxText;
    private ObservableCollection<StartPageItem> _startPageItems;

    public StartPageViewModel(IDataTransferService dataTransferService,
        TabViewService tabViewService,
        ILocalSettingsService localSettingsService)
    {
        _dataTransferService = dataTransferService;
        _tabViewService = tabViewService;
        _localSettingsService = localSettingsService;
        InitializePageComponents();
    }

    public IAsyncRelayCommand GridViewItemDeleteCommand =>
        new AsyncRelayCommand<StartPageItem>(OnGridViewItemDeleteCommandExecuted);

    public IAsyncRelayCommand SearchButtonTappedCommand => new AsyncRelayCommand(OnSearchButtonTappedCommandExecuted);
    public IAsyncRelayCommand SaveNewSiteCommand => new AsyncRelayCommand(OnSaveNewSiteCommandExecuted);
    public IAsyncRelayCommand KeyDownCommand => new AsyncRelayCommand<KeyRoutedEventArgs>(OnKeyDownCommandExecuted);
    public ICommand CancelCommand => new DelegateCommand(() => IsFlyoutClosed = true);
    public ICommand EditStartPageItem => new AsyncRelayCommand<StartPageItem>(OnEditStartPageItem);

    public bool IsSuggestionBarEnabled
    {
        get => _isSuggestionBarEnabled;
        set => SetProperty(ref _isSuggestionBarEnabled, value);
    }

    public bool IsAnimationEnabled
    {
        get => _isAnimationEnabled;
        set => SetProperty(ref _isAnimationEnabled, value);
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

    public StartPageItem GridViewSelectedItem
    {
        get => _gridViewSelectedItem;
        set
        {
            SetProperty(ref _gridViewSelectedItem, value);
            if (value == null) return;
            _tabViewService.CreateNewWebTab(value.Url, isReplaced: true);
        }
    }

    public SearchTermItem SearchBarSelectedItem
    {
        get => _searchBarSelectedItem;
        set
        {
            SetProperty(ref _searchBarSelectedItem, value);
            if (value == null) return;
            _tabViewService.CreateNewWebTab(value.Name, isReplaced: true);
        }
    }

    public StartPageItem EditableStartPageItem
    {
        get => _editableStartPageItem;
        set => SetProperty(ref _editableStartPageItem, value);
    }

    public int GridViewOrientation
    {
        get => _gridViewOrientation;
        set => SetProperty(ref _gridViewOrientation, value);
    }

    public string PlaceholderText
    {
        get => _placeholderText;
        set => SetProperty(ref _placeholderText, value);
    }

    public string NewSiteUrl
    {
        get => _newSiteUrl;
        set => SetProperty(ref _newSiteUrl, value);
    }

    public string NewSiteName
    {
        get => _newSiteName;
        set => SetProperty(ref _newSiteName, value);
    }

    public Uri LogoSource
    {
        get => _logoSource;
        set => SetProperty(ref _logoSource, value);
    }

    public string SearchBoxText
    {
        get => _searchBoxText;
        set => SetProperty(ref _searchBoxText, value);
    }

    public ObservableCollection<StartPageItem> StartPageItems
    {
        get => _startPageItems;
        set => SetProperty(ref _startPageItems, value);
    }

    public HashSet<SiteItem> RecentlySearchedItems
    {
        get => _recentlySearchedItems;
        set => SetProperty(ref _recentlySearchedItems, value);
    }

    //TODO: OnEditStartPageItem
    private async Task OnEditStartPageItem(StartPageItem startPageItem)
    {
        EditableStartPageItem = startPageItem;
        await new EditStartPageItemDialog().ShowAsync();
    }

    private async Task OnKeyDownCommandExecuted(object obj)
    {
        if (obj is not KeyRoutedEventArgs {Key: VirtualKey.Enter}) return;
        await OnSearchButtonTappedCommandExecuted().ConfigureAwait(false);
    }

    private async Task OnGridViewItemDeleteCommandExecuted(StartPageItem obj)
    {
        if (obj is not { }) return;
        await _dataTransferService.RemoveSiteOnStartPageAsync(obj);
        await GetStartPageElementsAsync().ConfigureAwait(false);
    }

    private async Task OnSaveNewSiteCommandExecuted()
    {
        if (string.IsNullOrWhiteSpace(NewSiteName) ||
            string.IsNullOrWhiteSpace(NewSiteUrl))
        {
            var dialogError = new ContentDialog
            {
                Title = "Внимание",
                Content = "Убедитесь, что все поля заполнены",
                CloseButtonText = "Закрыть"
            };

            await dialogError.ShowAsync();
            return;
        }

        if (!(NewSiteUrl.StartsWith("http://") ||
              NewSiteUrl.StartsWith("https://")))
            NewSiteUrl = "https://" + NewSiteUrl;
        await _dataTransferService.AddNewSiteOnStartPageAsync(new StartPageItem
        {
            Name = NewSiteName,
            Url = NewSiteUrl
        });
        IsFlyoutClosed = true;
        await GetStartPageElementsAsync().ConfigureAwait(false);
    }

    private async Task InitializePageComponents()
    {
        IsSuggestionBarEnabled = await _localSettingsService.ReadSettingAsync<bool>(nameof(IsSuggestionBarEnabled));
        IsAnimationEnabled = await _localSettingsService.ReadSettingAsync<bool>(nameof(IsAnimationEnabled));
        GridViewOrientation = await _localSettingsService.ReadSettingAsync<int>("StartPageGridViewOrientation");

        var currentWebEngineName = App.CurrentWebEngine.Name;
        if (currentWebEngineName == null) return;
        LogoSource = new Uri($"ms-appx:///Resources/Logos/{currentWebEngineName}Logo.png");
        PlaceholderText = "Искать с помощью " + currentWebEngineName;

        await GetStartPageElementsAsync();
        if (IsSuggestionBarEnabled) await GetRecentlySearchedItemsAsync();
    }

    private async Task GetRecentlySearchedItemsAsync()
    {
        var searchTermListTransfer = await _dataTransferService.GetSearchTermAsync();
        if (searchTermListTransfer == null) return;
        var termListTransfer = searchTermListTransfer.ToList();
        termListTransfer.Reverse();
        RecentlySearchedItems = new HashSet<SiteItem>(termListTransfer);
    }

    private async Task OnSearchButtonTappedCommandExecuted()
    {
        await _tabViewService.CreateNewWebTab(SearchBoxText, isReplaced: true);
    }

    private async Task GetStartPageElementsAsync()
    {
        StartPageItems = new ObservableCollection<StartPageItem>(await _dataTransferService.GetStartPageElementsAsync());
    }
}