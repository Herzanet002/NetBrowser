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
        private static ThemeItem _currentThemeItem;
        public static ElementTheme ThemeMode;
        
        public static Brush BackgroundBrush => _currentThemeItem.BackgroundBrush;
        public static Brush SecondBrush => _currentThemeItem.SecondBrush;
        public static Brush ThirdBrush => _currentThemeItem.ThirdBrush;

        public static Brush AppTitleBrush => _currentThemeItem.AppTitleBrush;
        public static Brush ForegroundBrush => _currentThemeItem.ForegroundBrush;
        public static Brush NavigationButtonBrush => _currentThemeItem.NavigationButtonBrush;

        public static Brush SearchBoxForeground => _currentThemeItem.SearchBoxForeground;

        public static Brush SearchBoxBorderBrush => _currentThemeItem.SearchBoxBorderBrush;

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

            _currentThemeItem = GetRequestedTheme(themeName);
            
            ThemeMode = _currentThemeItem.Mode;
            var foreground = NavigationButtonBrush as SolidColorBrush;

            if (App.TitleBar != null)
            {
                App.TitleBar.BackgroundColor = Colors.Transparent;
                App.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                App.TitleBar.ButtonHoverForegroundColor = Colors.Transparent;
                App.TitleBar.ButtonForegroundColor = foreground?.Color;

            }

            RaisePropertyChanged();
            return _currentThemeItem;
        }



        public void SetRequestedElementThemeMode()
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = ThemeMode;

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
            
            
        }
    }
}
