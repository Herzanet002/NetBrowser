using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.Views.News;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace NetBrowser_UWP.ViewModels.News
{
    public class RecommendationsNewsPageViewModel : ObservableObject
    {
        private readonly IDataTransferService _dataTransferService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ObservableCollection<ContentModel> _recommendedNews;
        public IAsyncRelayCommand RecommendationsNewsPageLoadedCommand { get; set; }
        public IAsyncRelayCommand CategoriesButtonSetCommand { get; set; }
        private bool _isProgressRingActive = true;
        private bool _isConfigured;

        public RecommendationsNewsPageViewModel(IDataTransferService dataTransferService, IServiceScopeFactory serviceScopeFactory)
        {
            _dataTransferService = dataTransferService;
            _serviceScopeFactory = serviceScopeFactory;
            RecommendationsNewsPageLoadedCommand =
                new AsyncRelayCommand(OnRecommendationsNewsPageLoadedCommandExecuted);
            CategoriesButtonSetCommand = new AsyncRelayCommand(OnCategoriesButtonSetCommandExecuted);
        }

        public bool IsConfigured
        {
            get => _isConfigured;
            set => SetProperty(ref _isConfigured, value);
        }

        public bool IsProgressRingActive
        {
            get => _isProgressRingActive;
            set => SetProperty(ref _isProgressRingActive, value);
        }

        private async Task OnCategoriesButtonSetCommandExecuted(CancellationToken ct = default)
        {
            await new FirstRunRecommendationsDialog().ShowAsync();
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
                }

                IsConfigured = false;
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
