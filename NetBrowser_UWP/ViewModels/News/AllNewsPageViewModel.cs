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
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;

namespace NetBrowser_UWP.ViewModels.News;

public class AllNewsPageViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TabViewService _tabViewService;
    private readonly IDataTransferService _dataTransferService;
    private readonly AppConfigService _appConfigService;
    private bool _isProgressRingActive = true;
    private ContentModel _newsForSharing;
    private ContentModel _selectedItemInAllNews;
    private ObservableCollection<ContentModel> _news = new();

    public IAsyncRelayCommand RotatorTileClickCommand { get; set; }
    public IAsyncRelayCommand AllNewsItemClickCommand { get; set; }
    public IAsyncRelayCommand AddNewsToFavoriteCommand { get; set; }
    public IAsyncRelayCommand AllNewsPageLoadedCommand { get; set; }
    public DelegateCommand<ContentModel> ShareNewsCommand { get; set; }

    public AllNewsPageViewModel(IServiceScopeFactory serviceScopeFactory,
        TabViewService tabViewService,
        IDataTransferService dataTransferService, AppConfigService appConfigService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tabViewService = tabViewService;
        _dataTransferService = dataTransferService;
        _appConfigService = appConfigService;

        News = new ObservableCollection<ContentModel>();
        InitializeCommands();
        DataTransferManager.GetForCurrentView().DataRequested += NewsPageViewModelOnDataSharing;
    }

    private void InitializeCommands()
    {
        RotatorTileClickCommand = new AsyncRelayCommand<ContentModel>(OnRotatorTileClickCommandExecuted);
        AllNewsItemClickCommand = new AsyncRelayCommand<ContentModel>(OnAllNewsItemClickCommandExecuted);
        AllNewsPageLoadedCommand = new AsyncRelayCommand(OnAllNewsPageLoadedCommandExecuted);
        AddNewsToFavoriteCommand = new AsyncRelayCommand<ContentModel>(OnAddNewsToFavoriteCommandExecuted);
        ShareNewsCommand = new DelegateCommand<ContentModel>(OnShareNewsCommandExecuted);
    }

    private async Task OnAllNewsPageLoadedCommandExecuted(CancellationToken ct)
    {
        var rssFeeders = await _dataTransferService.GetRssFeedersListAsync();
        var news = await GetNewsAsync(rssFeeders);
        await foreach (var content in news.WithCancellation(ct))
        {
            News.Add(content);
        }
        IsProgressRingActive = false;
    }

    private async Task OnAddNewsToFavoriteCommandExecuted(ContentModel contentItem, CancellationToken ct)
    {
        if (contentItem.IsFavorite)
        {
            await _dataTransferService.RemoveNewsContentFromFavorite(contentItem);
            contentItem.IsFavorite = false;
            News[News.IndexOf(contentItem)] = contentItem;
            return;

        }
        contentItem.IsFavorite = true;
        await _dataTransferService.SaveNewsContentToFavoriteAsync(contentItem);
        News[News.IndexOf(contentItem)] = contentItem;
    }

    private async Task OnAllNewsItemClickCommandExecuted(ContentModel contentItem, CancellationToken ct)
    {
        if (contentItem != null) await _tabViewService.CreateNewWebTab(contentItem.Link);
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

    public ContentModel SelectedItemInAllNews
    {
        get => _selectedItemInAllNews;
        set => SetProperty(ref _selectedItemInAllNews, value);
    }

    private void NewsPageViewModelOnDataSharing(DataTransferManager sender, DataRequestedEventArgs args)
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



    public async Task<IAsyncEnumerable<ContentModel>> GetNewsAsync(IEnumerable<RssFeeder> sources)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var rssWorker = scope.ServiceProvider.GetRequiredService<IRssWorkerService>();

        var favoriteNews = await _dataTransferService.GetAllFavoritesNewsContentAsync();
        var contentModels = rssWorker.GetFeeds(sources, favoriteNews.ToList());
        
        return contentModels;
    }
}