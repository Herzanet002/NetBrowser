using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser_UWP.Views.News;
using NetBrowser.Utils;
using Prism.Commands;

namespace NetBrowser_UWP.ViewModels.News;

public class RecommendationsNewsPageViewModel : BindableBase
{
    private readonly IDataService _dataService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ITabViewService _tabViewService;
    private bool _isConfiguredHidden;
    private bool _isProgressRingActive = true;
    private ContentModel _newsForSharing;
    private ObservableCollection<ContentModel> _recommendedNews;

    public RecommendationsNewsPageViewModel(IDataService dataService,
        IServiceScopeFactory serviceScopeFactory,
        ITabViewService tabViewService)
    {
        _dataService = dataService;
        _serviceScopeFactory = serviceScopeFactory;
        _tabViewService = tabViewService;
        RecommendationsNewsPageLoadedCommand =
            new AsyncRelayCommand(OnRecommendationsNewsPageLoadedCommandExecuted);
        CategoriesButtonSetCommand = new AsyncRelayCommand(OnCategoriesButtonSetCommandExecuted);
        ShareNewsCommand = new DelegateCommand<ContentModel>(OnShareNewsCommandExecuted);
        AddNewsToFavoriteCommand = new AsyncRelayCommand<ContentModel>(OnAddNewsToFavoriteCommandExecuted);
        ItemOpenCommand = new AsyncRelayCommand<ContentModel>(OnItemOpenCommandExecuted);
        DataTransferManager.GetForCurrentView().DataRequested += NewsPageViewModelOnDataSharing;
    }

    public IAsyncRelayCommand RecommendationsNewsPageLoadedCommand { get; set; }
    public IAsyncRelayCommand CategoriesButtonSetCommand { get; set; }
    public IAsyncRelayCommand AddNewsToFavoriteCommand { get; set; }
    public DelegateCommand<ContentModel> ShareNewsCommand { get; set; }
    public IAsyncRelayCommand ItemOpenCommand { get; set; }

    public bool IsConfiguredHidden
    {
        get => _isConfiguredHidden;
        set => SetProperty(ref _isConfiguredHidden, value);
    }

    public bool IsProgressRingActive
    {
        get => _isProgressRingActive;
        set => SetProperty(ref _isProgressRingActive, value);
    }


    public ObservableCollection<ContentModel> RecommendedNews
    {
        get => _recommendedNews;
        set => SetProperty(ref _recommendedNews, value);
    }

    private async Task OnItemOpenCommandExecuted(ContentModel contentItem)
    {
        if (contentItem != null)
            await _tabViewService.CreateNewWebTab(contentItem.Link);
    }

    private void NewsPageViewModelOnDataSharing(DataTransferManager sender, DataRequestedEventArgs args)
    {
        if (_newsForSharing == null) return;

        args.Request.Data.SetText(_newsForSharing.Title);
        args.Request.Data.Properties.Title = Package.Current.DisplayName;
        args.Request.Data.SetWebLink(new Uri(_newsForSharing.Link));
    }

    private async Task OnCategoriesButtonSetCommandExecuted(CancellationToken ct = default)
    {
        var result = await new FirstRunRecommendationsDialog().ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            IsConfiguredHidden = false;
            await OnRecommendationsNewsPageLoadedCommandExecuted(ct);
        }
        else
        {
            IsConfiguredHidden = true;
            IsProgressRingActive = false;
        }
    }

    private void OnShareNewsCommandExecuted(ContentModel param)
    {
        if (param == null) return;
        _newsForSharing = param;
        DataTransferManager.ShowShareUI();
    }

    private async Task OnAddNewsToFavoriteCommandExecuted(ContentModel contentItem, CancellationToken ct)
    {
        await _dataService.SaveNewsContentToFavoriteAsync(contentItem);
        RecommendedNews.Remove(contentItem);
    }

    private async Task OnRecommendationsNewsPageLoadedCommandExecuted(CancellationToken ct = default)
    {
        var recommendationCategories =
            await _dataService.GetLikedRssFeedersAsync();
        if (!recommendationCategories.Any())
        {
            var result = await new FirstRunRecommendationsDialog().ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                recommendationCategories =
                    await _dataService.GetLikedRssFeedersAsync();
                IsConfiguredHidden = false;
            }
            else
            {
                IsConfiguredHidden = true;
                IsProgressRingActive = false;
                return;
            }
        }

        var news = await GetNewsAsync(recommendationCategories);
        var suitableNews = new List<ContentModel>();
        await foreach (var content in news.WithCancellation(ct)) suitableNews.Add(content);
        suitableNews.Shuffle();
        RecommendedNews = new ObservableCollection<ContentModel>(suitableNews);
        IsProgressRingActive = false;
    }

    private async Task<IAsyncEnumerable<ContentModel>> GetNewsAsync(IEnumerable<RssFeeder> sources)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var rssWorker = scope.ServiceProvider.GetRequiredService<IRssWorkerService>();

        var favoriteNews = await _dataService.GetAllFavoriteNewsContentAsync();
        var contentModels = rssWorker.GetFeeds(sources, favoriteNews.ToList(), null);

        return contentModels;
    }
}