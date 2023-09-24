using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Exceptions;
using NetBrowser_UWP.IncrementalSources;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.ViewModels.Base;
using Prism.Commands;

namespace NetBrowser_UWP.ViewModels.News;

public class AllNewsPageViewModel : BindableBase
{
    private readonly ITabViewService _tabViewService;
    private readonly IDataService _dataService;

    private bool _isProgressRingActive = true;
    private NewsApiException _apiThrownException;
    private ContentModel _newsForSharing;
    private ContentModel _selectedItemInAllNews;
    private IncrementalLoadingCollection<NewsIncrementalSource, ContentModel> _news;

    public AllNewsPageViewModel(ITabViewService tabViewService,
        INewsIncrementalSourceFactory newsIncrementalSourceFactory,
        IDataService dataService)
    {
        _tabViewService = tabViewService;
        _dataService = dataService;
        var newsIncrementalSource = newsIncrementalSourceFactory.CreateNewsIncrementalSource();
        News = new IncrementalLoadingCollection<NewsIncrementalSource, ContentModel>(newsIncrementalSource)
        {
            OnEndLoading = () => IsProgressRingActive = false,
            OnStartLoading = () => IsProgressRingActive = true,
            OnError = ex => ApiThrownException = ex as NewsApiException
        };
        InitializeCommands();
        DataTransferManager.GetForCurrentView().DataRequested += OnDataSharing;
    }

    #region Commands

    public IAsyncRelayCommand RotatorTileClickCommand { get; private set; } = null!;

    public IAsyncRelayCommand AllNewsItemClickCommand { get; private set; } = null!;

    public IAsyncRelayCommand AddNewsToFavoriteCommand { get; private set; } = null!;

    public DelegateCommand<ContentModel> ShareNewsCommand { get; private set; } = null!;

    #endregion

    public bool IsProgressRingActive
    {
        get => _isProgressRingActive;
        private set => SetProperty(ref _isProgressRingActive, value);
    }

    public NewsApiException ApiThrownException
    {
        get => _apiThrownException;
        private set => SetProperty(ref _apiThrownException, value);
    }

    public IncrementalLoadingCollection<NewsIncrementalSource, ContentModel> News
    {
        get => _news;
        set => SetProperty(ref _news, value);
    }

    public ContentModel SelectedItemInAllNews
    {
        get => _selectedItemInAllNews;
        set => SetProperty(ref _selectedItemInAllNews, value);
    }

    private void InitializeCommands()
    {
        RotatorTileClickCommand = new AsyncRelayCommand<ContentModel>(OnRotatorTileClickCommandExecuted);
        AllNewsItemClickCommand = new AsyncRelayCommand<ContentModel>(OnAllNewsItemClickCommandExecuted);
        AddNewsToFavoriteCommand = new AsyncRelayCommand<ContentModel>(OnAddNewsToFavoriteCommandExecuted);
        ShareNewsCommand = new DelegateCommand<ContentModel>(OnShareNewsCommandExecuted);
    }

    private async Task OnAddNewsToFavoriteCommandExecuted(ContentModel contentItem)
    {
        await _dataService.SaveNewsContentToFavoriteAsync(contentItem);
        News.Remove(contentItem);
    }

    private async Task OnAllNewsItemClickCommandExecuted(ContentModel contentItem)
    {
        if (contentItem != null)
        {
            await _tabViewService.CreateNewWebTab(contentItem.Link);
        }
    }

    private void OnDataSharing(DataTransferManager sender, DataRequestedEventArgs args)
    {
        if (_newsForSharing == null) return;

        args.Request.Data.SetText(_newsForSharing.Title!);
        args.Request.Data.Properties.Title = Package.Current.DisplayName;
        args.Request.Data.SetWebLink(new Uri(_newsForSharing.Link!));
    }

    private void OnShareNewsCommandExecuted(ContentModel param)
    {
        if (param == null) return;
        _newsForSharing = param;
        DataTransferManager.ShowShareUI();
    }

    private async Task OnRotatorTileClickCommandExecuted(ContentModel param)
    {
        if (param == null) return;
        await _tabViewService.CreateNewWebTab(param.Link).ConfigureAwait(false);
    }
}