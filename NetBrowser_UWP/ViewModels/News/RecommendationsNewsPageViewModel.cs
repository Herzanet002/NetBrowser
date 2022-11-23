using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;

namespace NetBrowser_UWP.ViewModels.News
{
    public class RecommendationsNewsPageViewModel : ObservableObject
    {
        private readonly IDataTransferService _dataTransferService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ObservableCollection<RssFeeder> _rssFeederCollection;
        

        public IAsyncRelayCommand RecommendationsNewsPageLoadedCommand { get; set; }

        public RecommendationsNewsPageViewModel(IDataTransferService dataTransferService, IServiceScopeFactory serviceScopeFactory)
        {
            _dataTransferService = dataTransferService;
            _serviceScopeFactory = serviceScopeFactory;
            RssFeederCollection = new ObservableCollection<RssFeeder>();
            RecommendationsNewsPageLoadedCommand =
                new AsyncRelayCommand(OnRecommendationsNewsPageLoadedCommandExecuted);
            
        }

        private async Task OnRecommendationsNewsPageLoadedCommandExecuted(CancellationToken ct = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var firstRun = scope.ServiceProvider.GetRequiredService<FirstRunRecommendationsService>();
            await firstRun.ShowIfAppropriateAsync();
        }

        public ObservableCollection<RssFeeder> RssFeederCollection
        {
            get => _rssFeederCollection;
            set => SetProperty(ref _rssFeederCollection, value);
        }

        
    }
}
