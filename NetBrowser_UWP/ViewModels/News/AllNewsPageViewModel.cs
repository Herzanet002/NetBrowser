using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser.Utils;
using Prism.Commands;

namespace NetBrowser_UWP.ViewModels.News;

public class AllNewsPageViewModel : BindableBase
{
    private readonly IDataService _dataService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TabViewService _tabViewService;

    private bool _isProgressRingActive = true;
    private ObservableCollection<ContentModel> _news = new();
    private ContentModel _newsForSharing;
    private ContentModel _selectedItemInAllNews;

    public AllNewsPageViewModel(IServiceScopeFactory serviceScopeFactory,
        TabViewService tabViewService,
        IDataService dataService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tabViewService = tabViewService;
        _dataService = dataService;


        News = new ObservableCollection<ContentModel>();
        InitializeCommands();
        DataTransferManager.GetForCurrentView().DataRequested += OnDataSharing;
    }

    public IAsyncRelayCommand RotatorTileClickCommand { get; private set; }
    public IAsyncRelayCommand AllNewsItemClickCommand { get; private set; }
    public IAsyncRelayCommand AddNewsToFavoriteCommand { get; private set; }
    public IAsyncRelayCommand AllNewsPageLoadedCommand { get; private set; }
    public DelegateCommand<ContentModel> ShareNewsCommand { get; private set; }

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

    public ContentModel SelectedItemInAllNews
    {
        get => _selectedItemInAllNews;
        set => SetProperty(ref _selectedItemInAllNews, value);
    }

    private void InitializeCommands()
    {
        RotatorTileClickCommand = new AsyncRelayCommand<ContentModel>(OnRotatorTileClickCommandExecuted);
        AllNewsItemClickCommand = new AsyncRelayCommand<ContentModel>(OnAllNewsItemClickCommandExecuted);
        AllNewsPageLoadedCommand = new AsyncRelayCommand(OnAllNewsPageLoadedCommandExecuted);
        AddNewsToFavoriteCommand = new AsyncRelayCommand<ContentModel>(OnAddNewsToFavoriteCommandExecuted);
        ShareNewsCommand = new DelegateCommand<ContentModel>(OnShareNewsCommandExecuted);
    }

    private async Task OnAllNewsPageLoadedCommandExecuted(CancellationToken cancellationToken)
    {
        var rssFeeders = await _dataService.GetRssFeedersListAsync();
        var news = await GetNewsAsync(rssFeeders);
        var orderedEnumerable = new List<ContentModel>();
        await foreach (var content in news.WithCancellation(cancellationToken)) orderedEnumerable.Add(content);
        orderedEnumerable.Shuffle();
        News = new ObservableCollection<ContentModel>(orderedEnumerable);
        IsProgressRingActive = false;
    }

    private async Task OnAddNewsToFavoriteCommandExecuted(ContentModel contentItem)
    {
        if (contentItem.IsFavorite)
        {
            await _dataService.RemoveNewsContentFromFavoriteAsync(contentItem);
            contentItem.IsFavorite = false;
            News[News.IndexOf(contentItem)] = contentItem;
            return;
        }

        contentItem.IsFavorite = true;
        await _dataService.SaveNewsContentToFavoriteAsync(contentItem);
        News[News.IndexOf(contentItem)] = contentItem;
    }

    private async Task OnAllNewsItemClickCommandExecuted(ContentModel contentItem)
    {
        if (contentItem != null) await _tabViewService.CreateNewWebTab(contentItem.Link);
    }

    private void OnDataSharing(DataTransferManager sender, DataRequestedEventArgs args)
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
    
    private async Task<IAsyncEnumerable<ContentModel>> GetNewsAsync(IEnumerable<RssFeeder> sources)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var rssWorker = scope.ServiceProvider.GetRequiredService<IRssWorkerService>();
        var favoriteNews = await _dataService.GetAllFavoriteNewsContentAsync();
        var contentModels = rssWorker.GetFeeds(sources, favoriteNews.ToList(), 100);
        return contentModels;
    }
}