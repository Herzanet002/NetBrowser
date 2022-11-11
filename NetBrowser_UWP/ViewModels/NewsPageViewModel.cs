using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.Views.Settings;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NetBrowser_UWP.Views.News;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.ViewModels;

public class NewsPageViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TabViewService _tabViewService;
    private readonly INavigationService _navigationService;
    private ObservableCollection<ContentModel> _news = new();
    private bool _isProgressRingActive = true;
    private ContentModel _newsForSharing;
    private NavigationViewItem _selected;
    private NavigationView _navigationView;

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

    public NavigationViewItem Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }
    private ContentModel _selectedItemInAllNews;

    public ContentModel SelectedItemInAllNews
    {
        get => _selectedItemInAllNews;
        set
        {
            SetProperty(ref _selectedItemInAllNews, value);
            _tabViewService.CreateNewWebTab(value.Link);
        }
    }


    public NewsPageViewModel(IServiceScopeFactory serviceScopeFactory,
        TabViewService tabViewService,
        INavigationService navigationService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tabViewService = tabViewService;
        _navigationService = navigationService;

        RotatorTileClickCommand = new DelegateCommand<ContentModel>(OnRotatorTileClickCommandExecuted);
        ShareNewsCommand = new DelegateCommand<ContentModel>(OnShareNewsCommandExecuted);
        NavigationViewItemInvokedCommand = new DelegateCommand<NavigationViewItemInvokedEventArgs>(OnNavigationViewItemInvokedCommandExecuted);
        
        DataTransferManager.GetForCurrentView().DataRequested += NewsPageViewModel_DataRequested;
        InitializeNewsContent();
    }

    private void OnNavigationViewItemInvokedCommandExecuted(NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            _navigationService.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
        }
        else
        {
            var selectedItem = args.InvokedItemContainer as NavigationViewItem;

            try
            {
                if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is Type pageType)
                {
                    _navigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
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

    public void Initialize(Frame frame, NavigationView navigationView)
    {
        _navigationView = navigationView;
        _navigationService.Frame = frame;
        _navigationService.NavigationFailed += NavigationServiceOnNavigationFailed;
        _navigationService.Navigated += NavigationServiceOnNavigated;

        _navigationService.Navigate(typeof(AllNewsPage));
    }

    private void NavigationServiceOnNavigated(object sender, NavigationEventArgs e)
    {
        if (e.SourcePageType == typeof(SettingsPage))
        {
            Selected = _navigationView.SettingsItem as NavigationViewItem;
            return;
        }

        var selectedItem = GetSelectedItem(_navigationView.MenuItems, e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }

    private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
    {
        var pageType = menuItem.GetValue(NavigationHelper.NavigateToProperty) as Type;
        return pageType == sourcePageType;
    }

    private NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
    {
        foreach (var item in menuItems.OfType<NavigationViewItem>())
        {
            if (IsMenuItemForPageType(item, pageType))
            {
                return item;
            }

            var selectedChild = GetSelectedItem(item.MenuItems, pageType);
            if (selectedChild != null)
            {
                return selectedChild;
            }
        }

        return null;
    }

    private void NavigationServiceOnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {

    }

    private void OnRotatorTileClickCommandExecuted(ContentModel param)
    {
        if (param == null) return;
        _tabViewService.CreateNewWebTab(param.Link);
    }

    public async void InitializeNewsContent()
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