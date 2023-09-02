using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace NetBrowser_UWP.Contracts.Services;

public interface INavigationViewService
{
    event NavigatedEventHandler Navigated;

    NavigationViewItem GetSelectedItem(Type pageType);

    IList<object> MenuItems { get; }

    INavigationService NavigationService { get; }

    void Initialize(Frame frame, NavigationView navigationView, Type pageType = default);

    void UnregisterEvents();

    void NavigateToPageType(Type pageType, object parameter = null, NavigationTransitionInfo transitionInfo = null);
}