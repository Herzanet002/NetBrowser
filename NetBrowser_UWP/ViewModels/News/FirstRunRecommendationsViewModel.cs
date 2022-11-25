using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace NetBrowser_UWP.ViewModels.News
{
    public class FirstRunRecommendationsViewModel : ObservableObject
    {
        private readonly IDataTransferService _dataTransferService;
        private ObservableCollection<RssFeeder> _categories;
        public IAsyncRelayCommand PageLoadedCommand { get; set; }
        public IAsyncRelayCommand OkButtonCommand { get; set; }
        public DelegateCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; set; }
        private ObservableCollection<RssFeeder> _chosenCategories;
        private bool _isProgressRingActive = true;
        private bool _canContinue;


        public ObservableCollection<RssFeeder> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public bool CanContinue
        {
            get => _canContinue;
            set => SetProperty(ref _canContinue, value);
        }



        public bool IsProgressRingActive
        {
            get => _isProgressRingActive;
            set => SetProperty(ref _isProgressRingActive, value);
        }

        public FirstRunRecommendationsViewModel(IDataTransferService dataTransferService)
        {
            _dataTransferService = dataTransferService;
            PageLoadedCommand = new AsyncRelayCommand(OnPageLoaded);
            OkButtonCommand = new AsyncRelayCommand(OnOkButtonCommandExecuted);
            SelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(OnSelectionChangedCommandExecuted);
            Categories = new ObservableCollection<RssFeeder>();
            _chosenCategories = new ObservableCollection<RssFeeder>();
        }

        private async Task OnOkButtonCommandExecuted(CancellationToken ct = default)
        {
            await _dataTransferService.AddRecommendationRssCategoryAsync(_chosenCategories);
        }

        private void OnSelectionChangedCommandExecuted(SelectionChangedEventArgs obj)
        {
            foreach (var item in obj.AddedItems)
            {
                _chosenCategories.Add(item as RssFeeder);
            }

            foreach (var item in obj.RemovedItems)
            {
                _chosenCategories.Remove(item as RssFeeder);
            }

            CanContinue = _chosenCategories.Count > 2;
        }

        private async Task OnPageLoaded(CancellationToken ct = default)
        {
            var appConfigService = Ioc.Default.GetRequiredService<AppConfigService>();
            var feedResources = appConfigService.GetSection<List<RssFeeder>>("FeedResources");
            var categories = new HashSet<RssFeeder>();
            foreach (var feedResource in feedResources)
            {
                categories.Add(feedResource);
            }

            Categories = new ObservableCollection<RssFeeder>(categories);
            IsProgressRingActive = false;
        }
    }
}