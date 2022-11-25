using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.ViewModels.Settings;

public class SettingsPageViewModel : ObservableObject
{
    private bool _isBackEnabled;
    private NavigationViewItem _selected;

    public SettingsPageViewModel(INavigationViewService navigationViewService)
    {
        NavigationViewService = navigationViewService;
        NavigationViewService.Navigated += OnNavigated;
    }

    public INavigationViewService NavigationViewService { get; }

    public NavigationViewItem Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public bool IsBackEnabled
    {
        get => _isBackEnabled;
        set => SetProperty(ref _isBackEnabled, value);
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationViewService.NavigationService.CanGoBack;

        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null) Selected = selectedItem;
    }
}