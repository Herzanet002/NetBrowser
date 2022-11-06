using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml.Controls;

namespace NetBrowser_UWP.ViewModels;

public class NewsPageViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TabViewService _tabViewService;
    private ObservableCollection<ContentModel> _news = new();
    private bool _isProgressRingActive = true;
    private ContentModel _newsForSharing;
    public DelegateCommand<ContentModel> RotatorTileClickCommand { get; }
    public DelegateCommand<ContentModel> ShareNewsCommand { get; }
    public DelegateCommand<NavigationViewItemInvokedEventArgs> NavigationViewItemInvokedCommand { get; }

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

    public NewsPageViewModel(IServiceScopeFactory serviceScopeFactory, TabViewService tabViewService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tabViewService = tabViewService;
        RotatorTileClickCommand = new DelegateCommand<ContentModel>(OnRotatorTileClickCommandExecuted);
        ShareNewsCommand = new DelegateCommand<ContentModel>(OnShareNewsCommandExecuted);
        NavigationViewItemInvokedCommand = new DelegateCommand<NavigationViewItemInvokedEventArgs>(OnNavigationViewItemInvokedCommandExecuted);
        DataTransferManager.GetForCurrentView().DataRequested += NewsPageViewModel_DataRequested;
        Initialize();
    }

    private void OnNavigationViewItemInvokedCommandExecuted(NavigationViewItemInvokedEventArgs args)
    {
        //this.NavigationService.NavigateTo(args.InvokedItem);
    }

    private void NewsPageViewModel_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        if (_newsForSharing == null) return;

        args.Request.Data.SetText(_newsForSharing.Title);
        args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
        args.Request.Data.SetWebLink(new Uri(_newsForSharing.Link));
    }

    private void OnShareNewsCommandExecuted(ContentModel param)
    {
        if (param == null) return;
        _newsForSharing = param;
        DataTransferManager.ShowShareUI();

    }

    private void OnRotatorTileClickCommandExecuted(ContentModel param)
    {
        if(param == null) return;
        _tabViewService.CreateNewWebTab(param.Link);
    }

    public async void Initialize()
    {
        await GetNews(new Dictionary<string, string>
        {
            { "Lenta", "https://lenta.ru/rss/news" },
            { "RT", "https://russian.rt.com/rss" },
            { "Habr", "https://habr.com/ru/rss/all/all/" }
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
            foreach (var source in sources)
            {
                feeds.Add(await rssWorker.ParseRss(source.Value));
            }
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
                    PubDate = element.PublishDate.DateTime.ToString("g"),
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