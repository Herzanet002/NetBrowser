using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace NetBrowser_UWP.ViewModels.News
{
    public class FirstRunRecommendationsViewModel : ObservableObject
    {
        private readonly IDataTransferService _dataTransferService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ObservableCollection<SyndicationCategory> _categories;
        public IAsyncRelayCommand PageLoadedCommand { get; set; }
        public IAsyncRelayCommand OkButtonCommand { get; set; }
        public DelegateCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; set; }
        private ObservableCollection<SyndicationCategory> _chosenCategories;

        private bool _canContinue;

        public ObservableCollection<SyndicationCategory> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public bool CanContinue
        {
            get => _canContinue;
            set => SetProperty(ref _canContinue, value);
        }

        public FirstRunRecommendationsViewModel(IDataTransferService dataTransferService, IServiceScopeFactory serviceScopeFactory)
        {
            _dataTransferService = dataTransferService;
            _serviceScopeFactory = serviceScopeFactory;
            PageLoadedCommand = new AsyncRelayCommand(OnPageLoaded);
            OkButtonCommand = new AsyncRelayCommand(OnOkButtonCommandExecuted);
            SelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(OnSelectionChangedCommandExecuted);
            Categories = new ObservableCollection<SyndicationCategory>();
            _chosenCategories = new ObservableCollection<SyndicationCategory>();
        }

        private async Task OnOkButtonCommandExecuted(CancellationToken ct = default)
        {
            var sc = _chosenCategories.Select(syndicationCategory => new SyndicationCategoryModel
            { Name = syndicationCategory.Name, Label = syndicationCategory.Label, Scheme = syndicationCategory.Scheme }).ToList();
            await _dataTransferService.AddRecommendationSyndicationCategoryAsync(sc);
        }

        private void OnSelectionChangedCommandExecuted(SelectionChangedEventArgs obj)
        {
            foreach (var item in obj.AddedItems)
            {
                _chosenCategories.Add(item as SyndicationCategory);
            }

            foreach (var item in obj.RemovedItems)
            {
                _chosenCategories.Remove(item as SyndicationCategory);
            }

            CanContinue = _chosenCategories.Count > 4;
        }

        public Task<IAsyncEnumerable<ContentModel>> GetNewsAsync(IEnumerable<RssFeeder> sources)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var rssWorker = scope.ServiceProvider.GetRequiredService<IRssWorkerService>();
            var contentModels = rssWorker.GetFeeds(sources);

            return Task.FromResult(contentModels);
        }

        private async Task OnPageLoaded(CancellationToken ct = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var firstRun = scope.ServiceProvider.GetRequiredService<FirstRunRecommendationsService>();
            await firstRun.ShowIfAppropriateAsync();
            var rssFeeders = await _dataTransferService.GetRssFeedersListAsync();
            var news = await GetNewsAsync(rssFeeders);
            var categories = new HashSet<SyndicationCategory>();

            await foreach (var contentModel in news.WithCancellation(ct))
            {
                foreach (var syndicationCategoryModel in contentModel.Categories)
                {
                    if (categories.Any(x => x.Name == syndicationCategoryModel.Name)) continue;
                    categories.Add(syndicationCategoryModel);
                }
            }

            Categories = new ObservableCollection<SyndicationCategory>(categories);
        }
    }
}