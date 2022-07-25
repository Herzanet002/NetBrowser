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
        public static ElementTheme CurrentThemeMode;
        
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


        public ThemeItem GetRequestedTheme(string name)
        {
            return Constants.Constants.ThemesDictionary.ContainsKey(name) ?
                Constants.Constants.ThemesDictionary[name] :
                Constants.Constants.LightTheme;
        }

        public ThemeItem SetRequestedTheme(string themeName)
        {

            CurrentTheme = GetRequestedTheme(themeName);
            
            CurrentThemeMode = CurrentTheme.Mode;
            var foreground = NavigationButtonBrush as SolidColorBrush;

            if (App.TitleBar != null)
            {
                App.TitleBar.BackgroundColor = Colors.Transparent;
                App.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                App.TitleBar.ButtonHoverForegroundColor = Colors.Transparent;
                App.TitleBar.ButtonForegroundColor = foreground?.Color;

            }

            RaisePropertyChanged();
            return CurrentTheme;
        }



        public void SetRequestedElementThemeMode()
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = CurrentThemeMode;

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
