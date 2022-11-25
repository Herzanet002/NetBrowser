using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.ViewModels.News;

public class NewsShellPageViewModel : ObservableObject
{
    private NavigationViewItem _selectedNavViewItem;

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

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null) SelectedNavViewItem = selectedItem;
    }
}