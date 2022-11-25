using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.Views.News;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;

namespace NetBrowser_UWP.ViewModels.News
{
    public class RecommendationsNewsPageViewModel : ObservableObject
    {
        private readonly IDataTransferService _dataTransferService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TabViewService _tabViewService;
        private ObservableCollection<ContentModel> _recommendedNews;
        public IAsyncRelayCommand RecommendationsNewsPageLoadedCommand { get; set; }
        public IAsyncRelayCommand CategoriesButtonSetCommand { get; set; }
        public IAsyncRelayCommand AddNewsToFavoriteCommand { get; set; }
        public DelegateCommand<ContentModel> ShareNewsCommand { get; set; }
        public IAsyncRelayCommand ItemOpenCommand { get; set; }
        private ContentModel _newsForSharing;
        private bool _isProgressRingActive = true;
        private bool _isConfiguredHidden;

        public RecommendationsNewsPageViewModel(IDataTransferService dataTransferService, IServiceScopeFactory serviceScopeFactory,
            TabViewService tabViewService)
        {
            _dataTransferService = dataTransferService;
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

        private async Task OnCategoriesButtonSetCommandExecuted(CancellationToken ct = default)
        {
            var result = await new FirstRunRecommendationsDialog().ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                IsConfiguredHidden = false;
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
            await _dataTransferService.SaveNewsContentToFavoriteAsync(contentItem);
            RecommendedNews.Remove(contentItem);
        }
        private async Task OnRecommendationsNewsPageLoadedCommandExecuted(CancellationToken ct = default)
        {
            var recommendationCategories =
                await _dataTransferService.GetRecommendationRssCategoryAsync();
            if (recommendationCategories.Count == 0)
            {
                var result = await new FirstRunRecommendationsDialog().ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    recommendationCategories =
                        await _dataTransferService.GetRecommendationRssCategoryAsync();
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
            await foreach (var content in news.WithCancellation(ct))
            {
                suitableNews.Add(content);
            }
            suitableNews.Shuffle();
            RecommendedNews = new ObservableCollection<ContentModel>(suitableNews);
            IsProgressRingActive = false;
        }

        public async Task<IAsyncEnumerable<ContentModel>> GetNewsAsync(IEnumerable<RssFeeder> sources)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var rssWorker = scope.ServiceProvider.GetRequiredService<IRssWorkerService>();

            var favoriteNews = await _dataTransferService.GetAllFavoritesNewsContentAsync();
            var contentModels = rssWorker.GetFeeds(sources, favoriteNews.ToList());

            return contentModels;
        }


        public ObservableCollection<ContentModel> RecommendedNews
        {
            get => _recommendedNews;
            set => SetProperty(ref _recommendedNews, value);
        }


    }
}
