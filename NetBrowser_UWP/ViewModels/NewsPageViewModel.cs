using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Navigation;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace NetBrowser_UWP.ViewModels;

public class NewsPageViewModel : ObservableObject
{
    public INavigationViewService NavigationViewService { get; }

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TabViewService _tabViewService;
    private readonly IDataTransferService _dataTransferService;
    private readonly AppConfigService _appConfigService;
    private bool _isProgressRingActive = true;
    private ObservableCollection<ContentModel> _news = new();
    private ObservableCollection<ContentModel> _favoriteNews = new();
    private ContentModel _newsForSharing;
    private NavigationViewItem _selectedNavViewItem;
    private ContentModel _selectedItemInAllNews;

    public IAsyncRelayCommand PageLoadedCommand { get; set; }
    public IAsyncRelayCommand RotatorTileClickCommand { get; set; }
    public IAsyncRelayCommand AllNewsItemClickCommand { get; set; }
    public IAsyncRelayCommand AddNewsToFavoriteCommand { get; set; }
    public DelegateCommand<ContentModel> ShareNewsCommand { get; set; }

    public NewsPageViewModel(IServiceScopeFactory serviceScopeFactory,
        TabViewService tabViewService,
        IDataTransferService dataTransferService,
        INavigationViewService navigationViewService,
        AppConfigService appConfigService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tabViewService = tabViewService;
        _dataTransferService = dataTransferService;
        _appConfigService = appConfigService;
        NavigationViewService = navigationViewService;
        NavigationViewService.Navigated += OnNavigated;

        InitializeCommands();

        DataTransferManager.GetForCurrentView().DataRequested += NewsPageViewModel_DataRequested;
    }

    private void InitializeCommands()
    {
        RotatorTileClickCommand = new AsyncRelayCommand<ContentModel>(OnRotatorTileClickCommandExecuted);
        AllNewsItemClickCommand = new AsyncRelayCommand<ContentModel>(OnAllNewsItemClickCommandExecuted);
        PageLoadedCommand = new AsyncRelayCommand(OnPageLoadedCommandExecuted);
        AddNewsToFavoriteCommand = new AsyncRelayCommand<ContentModel>(OnAddNewsToFavoriteCommandExecuted);
        ShareNewsCommand = new DelegateCommand<ContentModel>(OnShareNewsCommandExecuted);
    }

    private async Task OnAddNewsToFavoriteCommandExecuted(ContentModel contentItem, CancellationToken ct)
    {
        await _dataTransferService.SaveNewsContentToFavoriteAsync(contentItem, ct);
    }

    private async Task OnPageLoadedCommandExecuted(CancellationToken ct)
    {
        await InitializeNewsContent();
    }

    private async Task OnAllNewsItemClickCommandExecuted(ContentModel contentItem, CancellationToken ct)
    {
        if (contentItem != null) await _tabViewService.CreateNewWebTab(contentItem.Link);
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null) SelectedNavViewItem = selectedItem;
    }

    public bool IsProgressRingActive
    {
        get => _isProgressRingActive;
        set => SetProperty(ref _isProgressRingActive, value);
    }

    public ObservableCollection<ContentModel> News
    {
        get => _news;
        set => SetProperty(ref _news, value);
    }

    public ObservableCollection<ContentModel> FavoriteNews
    {
        get => _favoriteNews;
        set => SetProperty(ref _favoriteNews, value);
    }

    public NavigationViewItem SelectedNavViewItem
    {
        get => _selectedNavViewItem;
        set => SetProperty(ref _selectedNavViewItem, value);
    }

    public ContentModel SelectedItemInAllNews
    {
        get => _selectedItemInAllNews;
        set => SetProperty(ref _selectedItemInAllNews, value);
    }

    private void NewsPageViewModel_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        if (_newsForSharing == null) return;

        args.Request.Data.SetText(_newsForSharing.Title);
        args.Request.Data.Properties.Title = Package.Current.DisplayName;
        args.Request.Data.SetWebLink(new Uri(_newsForSharing.Link));
    }

    private void OnShareNewsCommandExecuted(ContentModel param)
    {
        if (param == null) return;
        _newsForSharing = param;
        DataTransferManager.ShowShareUI();
    }

    private async Task OnRotatorTileClickCommandExecuted(ContentModel param)
    {
        if (param == null) return;
        await _tabViewService.CreateNewWebTab(param.Link).ConfigureAwait(false);
    }

    public async Task InitializeNewsContent()
    {
        var feedResources = _appConfigService.GetSection<Dictionary<string, string>>("FeedResources");
        await GetNews(feedResources);
        IsProgressRingActive = false;
    }

    public async Task GetNews(Dictionary<string, string> sources)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var rssWorker = scope.ServiceProvider.GetService<IRssWorkerService>();
        if (rssWorker is null) return;

        var syndicationFeeds = new List<SyndicationFeed>();
        await Task.Run(async () =>
        {
            foreach (var source in sources)
                syndicationFeeds.Add(await rssWorker.GetSyndicationFeedAsync(source.Value));
        });

        News = new ObservableCollection<ContentModel>(rssWorker.GetFeeds(syndicationFeeds));
    }
}