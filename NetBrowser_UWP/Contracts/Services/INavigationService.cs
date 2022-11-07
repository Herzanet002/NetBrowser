using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace NetBrowser_UWP.Contracts.Services
{
    public interface INavigationService
    {
        event NavigatedEventHandler Navigated;
        event NavigationFailedEventHandler NavigationFailed;

        bool CanGoBack
        {
            get;
        }

        Frame? Frame
        {
            get; set;
        }

        bool Navigate(Type pageType, object parameter = null,
            NavigationTransitionInfo infoOverride = null);
        bool Navigate<T>(object parameter = null, NavigationTransitionInfo infoOverride = null) where T : Page;
        bool GoBack();
    }
}
