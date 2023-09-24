using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser.Core.Models;
using NetBrowser.Storage;
using Prism.Commands;

namespace NetBrowser_UWP.ViewModels.News;

public class FavoriteNewsPageViewModel : BindableBase
{
    private readonly IDataService _dataService;
    private readonly ITabViewService _tabViewService;
    private ObservableCollection<ContentModel> _favoriteNews = new();
    private ContentModel _newsForSharing;
    private ContentModel _selectedItemNews;

    public FavoriteNewsPageViewModel(IDataService dataService, ITabViewService tabViewService)
    {
        _dataService = dataService;
        _tabViewService = tabViewService;
        FavoriteNewsPageLoadedCommand = new AsyncRelayCommand(OnFavoriteNewsPageLoadedExecuted);
        RemoveNewsCommand = new AsyncRelayCommand<ContentModel>(OnRemoveNewsCommandExecuted);
        ShareNewsCommand = new DelegateCommand<ContentModel>(OnShareNewsCommandExecuted);
        ItemOpenCommand = new AsyncRelayCommand<ContentModel>(OnItemOpenCommandExecuted);
        DataTransferManager.GetForCurrentView().DataRequested += OnDataSharing;
    }

    public DelegateCommand<ContentModel> ShareNewsCommand { get; set; }
    public IAsyncRelayCommand FavoriteNewsPageLoadedCommand { get; set; }
    public IAsyncRelayCommand RemoveNewsCommand { get; set; }
    public IAsyncRelayCommand ItemOpenCommand { get; set; }

    public ContentModel SelectedItemNews
    {
        get => _selectedItemNews;
        set => SetProperty(ref _selectedItemNews, value);
    }

    public ObservableCollection<ContentModel> FavoriteNews
    {
        get => _favoriteNews;
        set => SetProperty(ref _favoriteNews, value);
    }

    private async Task OnItemOpenCommandExecuted(ContentModel contentItem)
    {
        if (contentItem != null)
            await _tabViewService.CreateNewWebTab(contentItem.Link);
        SelectedItemNews = null;
    }

    private async Task OnRemoveNewsCommandExecuted(ContentModel contentItem)
    {
        await _dataService.RemoveNewsContentFromFavoritesAsync(contentItem);
        FavoriteNews.Remove(contentItem);
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

        args.Request.Data.SetText(_newsForSharing.Title!);
        args.Request.Data.Properties.Title = Package.Current.DisplayName;
        args.Request.Data.SetWebLink(new Uri(_newsForSharing.Link!));
    }

    private async Task OnFavoriteNewsPageLoadedExecuted()
    {
        var allFavoritesNewsContent = await _dataService.GetAllFavoritesNewsContentAsync();
        allFavoritesNewsContent.Reverse();
        FavoriteNews = new ObservableCollection<ContentModel>(allFavoritesNewsContent);
    }
}