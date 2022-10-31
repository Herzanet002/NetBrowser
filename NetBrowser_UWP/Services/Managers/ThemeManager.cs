using NetBrowser_UWP.Models;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using NetBrowser_UWP.Contracts;


// ReSharper disable once CheckNamespace
namespace NetBrowser_UWP
{
    public sealed class ThemeManager : INotifyPropertyChanged, IThemeManager
    {
        public static ThemeItem CurrentTheme;

        public static Brush BackgroundBrush => CurrentTheme.BackgroundBrush;
        public static Brush SecondBrush => CurrentTheme.SecondBrush;
        public static Brush ThirdBrush => CurrentTheme.ThirdBrush;
        public static Brush AppTitleBrush => CurrentTheme.AppTitleBrush;
        public static Brush ForegroundBrush => CurrentTheme.ForegroundBrush;
        public static Brush NavigationButtonBrush => CurrentTheme.NavigationButtonBrush;
        public static Brush SearchBoxForeground => CurrentTheme.SearchBoxForeground;
        public static Brush SearchBoxBorderBrush => CurrentTheme.SearchBoxBorderBrush;
        public static Brush BookmarkSavedBrush => CurrentTheme.BookmarkSavedBrush;


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
                    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public ThemeItem GetRequestedTheme(string themeName)
        {
            return themeName == null ? Constants.Constants.LightTheme :
             Constants.Constants.ThemesDictionary.ContainsKey(themeName) ?
                Constants.Constants.ThemesDictionary[themeName] :
                Constants.Constants.LightTheme;
        }

        public ThemeItem SetRequestedTheme(string themeName)
        {
            CurrentTheme = GetRequestedTheme(themeName);
            
            var foreground = NavigationButtonBrush as SolidColorBrush;

            if (App.TitleBar != null)
            {
                App.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                App.TitleBar.ButtonForegroundColor = foreground?.Color;
            }

            SetRequestedElementThemeMode();
            RaisePropertyChanged();
            return CurrentTheme;
        }



        public void SetRequestedElementThemeMode()
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = CurrentTheme.ThemeMode;

            }
        }


        private void RaisePropertyChanged()
        {
            OnPropertyChanged(nameof(BackgroundBrush));
            OnPropertyChanged(nameof(SecondBrush));
            OnPropertyChanged(nameof(ThirdBrush));
            OnPropertyChanged(nameof(AppTitleBrush));
            OnPropertyChanged(nameof(ForegroundBrush));
            OnPropertyChanged(nameof(NavigationButtonBrush));
            OnPropertyChanged(nameof(SearchBoxBorderBrush));
            OnPropertyChanged(nameof(SearchBoxForeground));
            OnPropertyChanged(nameof(BookmarkSavedBrush));
        }
    }
}
