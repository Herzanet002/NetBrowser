using System;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;
using NetBrowser_UWP.Contracts;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.ViewModels.Base;
using CommunityToolkit.Mvvm.Messaging;
using NetBrowser_UWP.Messages;

namespace NetBrowser_UWP.ViewModels.News;

public class NewsShellPageViewModel : BindableBase, INavigationViewContentPage
{
    private NavigationViewItem _selectedNavViewItem;
    private Type _innerPageType;

    public NewsShellPageViewModel(INavigationViewService navigationViewService)
    {
        NavigationViewService = navigationViewService;
        NavigationViewService.Navigated += OnNavigated;
    }

    public INavigationViewService NavigationViewService { get; }

    public NavigationViewItem SelectedNavViewItem
    {
        get => _selectedNavViewItem;
        set => SetProperty(ref _selectedNavViewItem, value);
    }

    public Type InnerPageType
    {
        get => _innerPageType;
        set => SetProperty(ref _innerPageType, value);
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null) SelectedNavViewItem = selectedItem;
        InnerPageType = e.SourcePageType;
        WeakReferenceMessenger.Default.Send(new InnerPageTypeChangedMessage(e.SourcePageType));
    }
}