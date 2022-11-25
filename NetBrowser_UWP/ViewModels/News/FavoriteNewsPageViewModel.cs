using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Models;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using NetBrowser_UWP.Contracts.Services;
using Prism.Commands;
using Windows.ApplicationModel.DataTransfer;
using System;
using Windows.ApplicationModel;
using NetBrowser_UWP.Services;

namespace NetBrowser_UWP.ViewModels.News
{
    public class FavoriteNewsPageViewModel : ObservableObject
    {
        private readonly IDataTransferService _dataTransferService;
        private readonly TabViewService _tabViewService;
        private ObservableCollection<ContentModel> _favoriteNews = new();
        private ContentModel _newsForSharing;
        private ContentModel _selectedItemNews;
        public DelegateCommand<ContentModel> ShareNewsCommand { get; set; }
        public IAsyncRelayCommand FavoriteNewsPageLoadedCommand { get; set; }
        public IAsyncRelayCommand RemoveNewsCommand { get; set; }
        public IAsyncRelayCommand ItemOpenCommand { get; set; }
        public FavoriteNewsPageViewModel(IDataTransferService dataTransferService, TabViewService tabViewService)
        {
            _dataTransferService = dataTransferService;
            _tabViewService = tabViewService;
            FavoriteNewsPageLoadedCommand = new AsyncRelayCommand(OnFavoriteNewsPageLoadedExecuted);
            RemoveNewsCommand = new AsyncRelayCommand<ContentModel>(OnRemoveNewsCommandExecuted);
            ShareNewsCommand = new DelegateCommand<ContentModel>(OnShareNewsCommandExecuted);
            ItemOpenCommand = new AsyncRelayCommand<ContentModel>(OnItemOpenCommandExecuted);
            DataTransferManager.GetForCurrentView().DataRequested += OnDataSharing;
        }
        public ContentModel SelectedItemNews
        {
            get => _selectedItemNews;
            set => SetProperty(ref _selectedItemNews, value);
        }
        private async Task OnItemOpenCommandExecuted(ContentModel contentItem)
        {
            if (contentItem != null)
                await _tabViewService.CreateNewWebTab(contentItem.Link);
            SelectedItemNews = null;
        }

        private async Task OnRemoveNewsCommandExecuted(ContentModel contentItem)
        {
            await _dataTransferService.RemoveNewsContentFromFavorite(contentItem);
            FavoriteNews.Remove(contentItem);
        }

        public ObservableCollection<ContentModel> FavoriteNews
        {
            get => _favoriteNews;
            set => SetProperty(ref _favoriteNews, value);
        }
        private void OnShareNewsCommandExecuted(ContentModel param)
        {
            if (param == null) return;
            _newsForSharing = param;
            DataTransferManager.ShowShareUI();
        }
        private void OnDataSharing(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (_newsForSharing == null) return;

            args.Request.Data.SetText(_newsForSharing.Title);
            args.Request.Data.Properties.Title = Package.Current.DisplayName;
            args.Request.Data.SetWebLink(new Uri(_newsForSharing.Link));
        }
        private async Task OnFavoriteNewsPageLoadedExecuted(CancellationToken ct)
        {
            FavoriteNews = new ObservableCollection<ContentModel>(await _dataTransferService.GetAllFavoritesNewsContentAsync());
        }

    }
}
