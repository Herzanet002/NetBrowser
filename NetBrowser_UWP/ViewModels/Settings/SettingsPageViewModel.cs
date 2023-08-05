using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.ViewModels.Base;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace NetBrowser_UWP.ViewModels.Settings;

public class SettingsPageViewModel : BindableBase
{
    private readonly INavigationViewService _navigationViewService;
    private bool _isBackEnabled;
    private NavigationViewItem _selected;

    public SettingsPageViewModel(INavigationViewService navigationViewService)
    {
        _navigationViewService = navigationViewService;
        _navigationViewService = navigationViewService;
        _navigationViewService.Navigated += OnNavigated;
    }

    public void Initialize(Frame frame, NavigationView navigationView, Type pageType)
        => _navigationViewService?.Initialize(frame, navigationView, pageType);


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

    public void NavigateToPageType(Type pageType)
        => _navigationViewService.NavigateToPageType(pageType);
    
    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = _navigationViewService.NavigationService.CanGoBack;
        var selectedItem = _navigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }
}