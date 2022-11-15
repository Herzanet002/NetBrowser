using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Navigation;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace NetBrowser_UWP.ViewModels;

public class NewsPageViewModel : ObservableObject
{
    public INavigationViewService NavigationViewService
    {
        get;
    }

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TabViewService _tabViewService;
    private bool _isProgressRingActive = true;
    private ObservableCollection<ContentModel> _news = new();
    private ContentModel _newsForSharing;
    private NavigationViewItem _selectedNavViewItem;
    private ContentModel _selectedItemInAllNews;

    public IAsyncRelayCommand RotatorTileClickCommand { get; }
    public IAsyncRelayCommand AllNewsItemClickCommand { get; }
    public DelegateCommand<ContentModel> ShareNewsCommand { get; }

    public NewsPageViewModel(IServiceScopeFactory serviceScopeFactory,
        TabViewService tabViewService,
        INavigationViewService navigationViewService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tabViewService = tabViewService;
        NavigationViewService = navigationViewService;
        NavigationViewService.Navigated += OnNavigated;

        RotatorTileClickCommand = new AsyncRelayCommand<ContentModel>(OnRotatorTileClickCommandExecuted);
        AllNewsItemClickCommand = new AsyncRelayCommand<ContentModel>(OnAllNewsItemClickCommandExecuted);
        ShareNewsCommand = new DelegateCommand<ContentModel>(OnShareNewsCommandExecuted);

        DataTransferManager.GetForCurrentView().DataRequested += NewsPageViewModel_DataRequested;
        InitializeNewsContent();
    }

    private async Task OnAllNewsItemClickCommandExecuted(ContentModel contentItem, CancellationToken ct)
    {
        if (contentItem != null)
        {
            await _tabViewService.CreateNewWebTab(contentItem.Link);
        }
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            SelectedNavViewItem = selectedItem;
        }
    }

    public bool IsProgressRingActive
    {
        get => _isProgressRingActive;
        set => SetProperty(ref _isProgressRingActive, value);
    }

    public ObservableCollection<ContentModel> News
    {
        get => _news;
        set => SetProperty(ref _news, value);
    }

    public NavigationViewItem SelectedNavViewItem
    {
        get => _selectedNavViewItem;
        set => SetProperty(ref _selectedNavViewItem, value);
    }

    public ContentModel SelectedItemInAllNews
    {
        get => _selectedItemInAllNews;
        set => SetProperty(ref _selectedItemInAllNews, value);
    }

    private void NewsPageViewModel_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        if (_newsForSharing == null) return;

        args.Request.Data.SetText(_newsForSharing.Title);
        args.Request.Data.Properties.Title = Package.Current.DisplayName;
        args.Request.Data.SetWebLink(new Uri(_newsForSharing.Link));
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

    public async Task InitializeNewsContent()
    {
        await GetNews(new Dictionary<string, string>
        {
            {"Lenta", "https://lenta.ru/rss/news"},
            {"RT", "https://russian.rt.com/rss"},
            {"Habr", "https://habr.com/ru/rss/all/all/"}
        });
        IsProgressRingActive = false;
    }

    public async Task GetNews(Dictionary<string, string> sources)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var rssWorker = scope.ServiceProvider.GetService<IRssWorkerService>();
        if (rssWorker is null) return;

        var feeds = new List<SyndicationFeed>();
        await Task.Run(async () =>
        {
            foreach (var source in sources) feeds.Add(await rssWorker.ParseRss(source.Value));
        });

        foreach (var syndicationFeed in feeds)
        {
            if (syndicationFeed is null) continue;
            foreach (var element in syndicationFeed.Items)
            {
                if (element is null || element.Links.Count != 2) continue;
                News.Add(new ContentModel
                {
                    Title = element.Title.Text,
                    Description = element.Summary.Text.Trim().Replace("\n", string.Empty),
                    PubDate = element.PublishDate.LocalDateTime.ToString("g"),
                    Link = element.Links[0].Uri.ToString(),
                    ImageUrl = element.Links[1].Uri.ToString(),
                    FeederImageLink = syndicationFeed.ImageUrl.ToString(),
                    Feeder = syndicationFeed.Title.Text
                });
            }
        }
        //News.Shuffle();
    }
}