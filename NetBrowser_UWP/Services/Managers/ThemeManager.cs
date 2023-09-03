using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Services.Managers;

public sealed class ThemeManager : ObservableRecipient, IThemeManager
{
    public static ThemeItem CurrentTheme { get; set; }

    public ThemeItem GetRequestedTheme(string themeName)
        => themeName == null
            ? Constants.ApplicationConstants.LightTheme
            : Constants.ApplicationConstants.ThemesDictionary.TryGetValue(themeName, out var value)
                ? value
                : Constants.ApplicationConstants.LightTheme;

    public ThemeItem SetRequestedTheme(string themeName)
    {
        CurrentTheme = GetRequestedTheme(themeName);
        App.CurrentTheme = CurrentTheme;

        var buttonForegroundColor = CurrentTheme.NavigationButtonBrush as SolidColorBrush;
        var buttonInactiveBackgroundColor = CurrentTheme.BackgroundBrush as SolidColorBrush;

        if (App.TitleBar != null)
        {
            App.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            App.TitleBar.ButtonForegroundColor = buttonForegroundColor?.Color;
            App.TitleBar.ButtonInactiveBackgroundColor = buttonInactiveBackgroundColor?.Color;
        }

        SetRequestedElementThemeMode();
        OnPropertyChanged(nameof(CurrentTheme));
        return CurrentTheme;
    }

    public void SetRequestedElementThemeMode()
    {
        if (Window.Current.Content is FrameworkElement frameworkElement)
            frameworkElement.RequestedTheme = CurrentTheme.ThemeMode;
    }
}