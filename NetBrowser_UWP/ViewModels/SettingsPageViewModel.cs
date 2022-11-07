using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.Views.Settings;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;

namespace NetBrowser_UWP.ViewModels
{
    public class SettingsPageViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private NavigationView _navigationView;
        private NavigationViewItem _selected;
        private bool _isBackEnabled;

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
        public SettingsPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigationViewItemInvokedCommand = new DelegateCommand<NavigationViewItemInvokedEventArgs>(OnNavigationViewItemInvokedCommandExecuted);
        }

        public DelegateCommand<NavigationViewItemInvokedEventArgs> NavigationViewItemInvokedCommand { get; }

        private void OnNavigationViewItemInvokedCommandExecuted(NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                _navigationService.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else
            {
                var selectedItem = args.InvokedItemContainer as NavigationViewItem;

                if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is Type pageType)
                {
                    _navigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
                }
            }
        }

        public void Initialize(Frame frame, NavigationView navigationView)
        {
            _navigationView = navigationView;
            _navigationService.Frame = frame;
            _navigationService.NavigationFailed += NavigationServiceOnNavigationFailed;
            _navigationService.Navigated += NavigationServiceOnNavigated;
            _navigationView.BackRequested += NavigationViewOnBackRequested;
            _navigationService.Navigate(typeof(MainItemPageSettings));
        }

        private void NavigationViewOnBackRequested(NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            _navigationService.GoBack();
        }

        private void NavigationServiceOnNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
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

        private void NavigationServiceOnNavigationFailed(object sender, Windows.UI.Xaml.Navigation.NavigationFailedEventArgs e)
        {

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



    }
}
