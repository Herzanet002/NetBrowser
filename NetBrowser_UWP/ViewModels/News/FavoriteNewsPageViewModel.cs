using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Models;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.ViewModels.News
{
    public class FavoriteNewsPageViewModel : ObservableObject
    {
        private readonly IDataTransferService _dataTransferService;
        private ObservableCollection<ContentModel> _favoriteNews = new();
        public IAsyncRelayCommand FavoriteNewsPageLoadedCommand { get; set; }

        public FavoriteNewsPageViewModel(IDataTransferService dataTransferService)
        {
            _dataTransferService = dataTransferService;
            FavoriteNewsPageLoadedCommand = new AsyncRelayCommand(OnFavoriteNewsPageLoadedExecuted);
        }
        public ObservableCollection<ContentModel> FavoriteNews
        {
            get => _favoriteNews;
            set => SetProperty(ref _favoriteNews, value);
        }

        private async Task OnFavoriteNewsPageLoadedExecuted(CancellationToken ct)
        {
            FavoriteNews = new ObservableCollection<ContentModel>(await _dataTransferService.GetAllFavoritesNewsContentAsync());
        }

    }
}
