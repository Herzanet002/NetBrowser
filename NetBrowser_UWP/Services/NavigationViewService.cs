using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.Views.Settings;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewBackRequestedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;

namespace NetBrowser_UWP.Services;

public class NavigationViewService : INavigationViewService
{
    private NavigationView _navigationView;

    public NavigationViewService(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    public INavigationService NavigationService { get; }
    public event NavigatedEventHandler Navigated;

    public IList<object> MenuItems => _navigationView?.MenuItems;

    public object SettingsItem => _navigationView?.SettingsItem;

    public void Initialize(Frame frame, NavigationView navigationView, Type pageType = default)
    {
        NavigationService.Frame = frame;
        _navigationView = navigationView;
        _navigationView.BackRequested += OnBackRequested;
        _navigationView.ItemInvoked += OnItemInvoked;
        NavigationService.Navigated += OnNavigated;
        if (pageType != default) NavigationService.Navigate(pageType);
    }

    public void UnregisterEvents()
    {
        if (_navigationView != null)
        {
            _navigationView.BackRequested -= OnBackRequested;
            _navigationView.ItemInvoked -= OnItemInvoked;
        }
    }

    public NavigationViewItem GetSelectedItem(Type pageType)
    {
        if (_navigationView != null)
            return GetSelectedItem(_navigationView.MenuItems, pageType) ??
                   GetSelectedItem(_navigationView.FooterMenuItems, pageType);

        return null;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        Navigated?.Invoke(sender, e);
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        NavigationService.GoBack();
    }

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            NavigationService.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
        }
        else
        {
            var selectedItem = args.InvokedItemContainer as NavigationViewItem;

            if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is Type pageType)
                NavigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
        }
    }

    private NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
    {
        foreach (var item in menuItems.OfType<NavigationViewItem>())
        {
            if (IsMenuItemForPageType(item, pageType)) return item;

            var selectedChild = GetSelectedItem(item.MenuItems, pageType);
            if (selectedChild != null) return selectedChild;
        }

        return null;
    }

    private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
    {
        var pageType = menuItem.GetValue(NavigationHelper.NavigateToProperty) as Type;
        return pageType == sourcePageType;
    }
}