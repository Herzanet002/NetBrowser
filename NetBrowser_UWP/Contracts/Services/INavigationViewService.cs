using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace NetBrowser_UWP.Contracts.Services
{
    public interface INavigationViewService
    {
        IList<object> MenuItems
        {
            get;
        }

        object SettingsItem
        {
            get;
        }

        void Initialize(Frame frame, NavigationView navigationView, Type pageType = default);

        void UnregisterEvents();

        event NavigatedEventHandler Navigated;
        INavigationService NavigationService { get; }

        NavigationViewItem GetSelectedItem(Type pageType);
    }
}
