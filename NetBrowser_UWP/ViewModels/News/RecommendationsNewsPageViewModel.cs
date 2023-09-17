using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.IncrementalSources;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser_UWP.Views.News;
using Prism.Commands;

namespace NetBrowser_UWP.ViewModels.News;

public class RecommendationsNewsPageViewModel : BindableBase
{
    private readonly IDataService _dataService;
    private readonly ITabViewService _tabViewService;
    private readonly INewsIncrementalSourceFactory _newsIncrementalSourceFactory;
    private bool _isConfiguredHidden;
    private bool _isProgressRingActive = true;
    private ContentModel _newsForSharing;
    private IncrementalLoadingCollection<NewsIncrementalSource, ContentModel> _recommendedNews;

    public RecommendationsNewsPageViewModel(IDataService dataService,
        ITabViewService tabViewService,
        INewsIncrementalSourceFactory newsIncrementalSourceFactory)
    {
        _dataService = dataService;
        _tabViewService = tabViewService;
        _newsIncrementalSourceFactory = newsIncrementalSourceFactory;
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
        private set => SetProperty(ref _isConfiguredHidden, value);
    }

    public bool IsProgressRingActive
    {
        get => _isProgressRingActive;
        private set => SetProperty(ref _isProgressRingActive, value);
    }


    public IncrementalLoadingCollection<NewsIncrementalSource, ContentModel> RecommendedNews
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

        args.Request.Data.SetText(_newsForSharing.Title!);
        args.Request.Data.Properties.Title = Package.Current.DisplayName;
        args.Request.Data.SetWebLink(new Uri(_newsForSharing.Link!));
    }

    private async Task OnCategoriesButtonSetCommandExecuted()
    {
        var result = await new FirstRunRecommendationsDialog().ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            IsConfiguredHidden = false;
            await OnRecommendationsNewsPageLoadedCommandExecuted();
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

    private async Task OnAddNewsToFavoriteCommandExecuted(ContentModel contentItem)
    {
        await _dataService.SaveNewsContentToFavoriteAsync(contentItem);
        RecommendedNews.Remove(contentItem);
    }

    private async Task OnRecommendationsNewsPageLoadedCommandExecuted()
    {
        var likedNewsProviders =
            await _dataService.GetLikedNewsProvidersAsync();
        if (!likedNewsProviders.Any())
        {
            var result = await new FirstRunRecommendationsDialog().ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                likedNewsProviders =
                    await _dataService.GetLikedNewsProvidersAsync();
                IsConfiguredHidden = false;
            }
            else
            {
                IsConfiguredHidden = true;
                IsProgressRingActive = false;
                return;
            }
        }

        var newsByProvidersIncrementalSource =
            _newsIncrementalSourceFactory.CreateNewsByProvidersIncrementalSource(likedNewsProviders);
        RecommendedNews =
            new IncrementalLoadingCollection<NewsIncrementalSource, ContentModel>(newsByProvidersIncrementalSource)
            {
                OnEndLoading = () => IsProgressRingActive = false,
                OnStartLoading = () => IsProgressRingActive = true
            };
    }
}