using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;
using Windows.UI.Xaml.Navigation;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace NetBrowser_UWP.ViewModels.News;

public class NewsShellPageViewModel : ObservableObject
{
    public INavigationViewService NavigationViewService { get; }

    private NavigationViewItem _selectedNavViewItem;

    public NewsShellPageViewModel(INavigationViewService navigationViewService)
    {
        NavigationViewService = navigationViewService;
        NavigationViewService.Navigated += OnNavigated;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null) SelectedNavViewItem = selectedItem;
    }

    public NavigationViewItem SelectedNavViewItem
    {
        get => _selectedNavViewItem;
        set => SetProperty(ref _selectedNavViewItem, value);
    }
}