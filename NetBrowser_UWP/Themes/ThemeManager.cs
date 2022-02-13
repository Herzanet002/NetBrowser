using System;
using System.ComponentModel;
using System.IO;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

// ReSharper disable once CheckNamespace
namespace NetBrowser_UWP
{
    public sealed class ThemeManager : INotifyPropertyChanged
    {

        private static ResourceDictionary _currentThemeDictionary;
        public static int ThemeMode = 1;

        public static string CurrentTheme { get; private set; }

        public static Brush BackgroundBrush => _currentThemeDictionary[nameof(BackgroundBrush)] as Brush;
        public static Brush SecondBrush => _currentThemeDictionary[nameof(SecondBrush)] as Brush;
        public static Brush ThirdBrush => _currentThemeDictionary[nameof(ThirdBrush)] as Brush;

        public static Brush AppTitleBrush => _currentThemeDictionary[nameof(AppTitleBrush)] as Brush;
        public static Brush ForegroundBrush => _currentThemeDictionary[nameof(ForegroundBrush)] as Brush;
        public static Brush NavigationButtonBrush => _currentThemeDictionary[nameof(NavigationButtonBrush)] as Brush;

        public static Brush SearchBoxForeground => _currentThemeDictionary[nameof(SearchBoxForeground)] as Brush;

        public static Brush SearchBoxBorderBrush => _currentThemeDictionary[nameof(SearchBoxBorderBrush)] as Brush;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
                    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public void LoadTheme(string path)
        {
            _currentThemeDictionary = new ResourceDictionary();
            Application.LoadComponent(_currentThemeDictionary, new Uri(path));
            CurrentTheme = Path.GetFileNameWithoutExtension(path);
            SolidColorBrush foreground = ThemeManager.NavigationButtonBrush as SolidColorBrush;

            if (App.TitleBar != null)
            {
                App.TitleBar.BackgroundColor = Colors.Transparent;
                App.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                App.TitleBar.ButtonHoverForegroundColor = Colors.Transparent;
                App.TitleBar.ButtonForegroundColor = foreground?.Color;

            }
            RaisePropertyChanged();
        }


        public static void SetRequestedTheme()
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = (ElementTheme)ThemeMode;
            }
        }

        public void LoadThemeByMode(int mode)
        {
            (string, int) theme = Constants.Constants.Themes[mode];
            string themePath = theme.Item1;
            CurrentTheme = themePath;
            ThemeMode = theme.Item2;
            LoadTheme(themePath);

            App.ThemeMode = mode;

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
            OnPropertyChanged(nameof(CurrentTheme));
        }
    }
}
