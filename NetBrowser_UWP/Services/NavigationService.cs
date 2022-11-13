using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.Services;

public class NavigationService : INavigationService
{
    private Frame _frame;
    private object _lastParamUsed;

    public bool CanGoForward => Frame.CanGoForward;
    public event NavigatedEventHandler Navigated;

    public event NavigationFailedEventHandler NavigationFailed;

    public Frame Frame
    {
        get
        {
            if (_frame != null) return _frame;
            _frame = Window.Current.Content as Frame;
            RegisterFrameEvents();

            return _frame;
        }

        set
        {
            UnregisterFrameEvents();
            _frame = value;
            RegisterFrameEvents();
        }
    }

    public bool CanGoBack => Frame is {CanGoBack: true};

    public bool GoBack()
    {
        if (!CanGoBack) return false;
        Frame.GoBack();
        return true;
    }

    public bool Navigate(Type pageType, object parameter = null,
        NavigationTransitionInfo infoOverride = null)
    {
        if (pageType == null || !pageType.IsSubclassOf(typeof(Page)))
            throw new ArgumentException($"Invalid pageType '{pageType}', please provide a valid pageType.",
                nameof(pageType));

        // Don't open the same page multiple times
        if (Frame.Content?.GetType() == pageType && (parameter == null || parameter.Equals(_lastParamUsed)))
            return false;

        var navigationResult = Frame.Navigate(pageType, parameter, infoOverride);
        if (navigationResult) _lastParamUsed = parameter;

        return navigationResult;
    }

    public bool Navigate<T>(object parameter = null, NavigationTransitionInfo infoOverride = null)
        where T : Page
    {
        return Navigate(typeof(T), parameter, infoOverride);
    }

    public void GoForward()
    {
        Frame.GoForward();
    }

    private void RegisterFrameEvents()
    {
        if (_frame != null)
        {
            _frame.Navigated += FrameOnNavigated;
            _frame.NavigationFailed += FrameOnNavigationFailed;
        }
    }

    private void UnregisterFrameEvents()
    {
        if (_frame == null) return;
        _frame.Navigated -= FrameOnNavigated;
        _frame.NavigationFailed -= FrameOnNavigationFailed;
    }

    private void FrameOnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        NavigationFailed?.Invoke(sender, e);
    }

    private void FrameOnNavigated(object sender, NavigationEventArgs e)
    {
        Navigated?.Invoke(sender, e);
    }
}