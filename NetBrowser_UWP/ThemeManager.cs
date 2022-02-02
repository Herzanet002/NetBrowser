using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace NetBrowser_UWP
{
    public sealed class ThemeManager : INotifyPropertyChanged
    {

        public const string DarkBlueThemePath = "ms-appx:///Themes/Theme.DarkBlue.xaml";
        public const string LightThemePath = "ms-appx:///Themes/Theme.Light.xaml";
        public const string PinkRedThemePath = "ms-appx:///Themes/Theme.LightPink.xaml";
        public const string DarkTheme = "ms-appx:///Themes/Theme.Dark.xaml";

        private ResourceDictionary _currentThemeDictionary;


        public string CurrentTheme { get; private set; }

        public Brush BackgroundBrush => _currentThemeDictionary[nameof(BackgroundBrush)] as Brush;
        public Brush SecondBrush => _currentThemeDictionary[nameof(SecondBrush)] as Brush;
        public Brush ThirdBrush => _currentThemeDictionary[nameof(ThirdBrush)] as Brush;

        public Brush AppTitleBrush => _currentThemeDictionary[nameof(AppTitleBrush)] as Brush;
        public Brush ForegroundBrush => _currentThemeDictionary[nameof(ForegroundBrush)] as Brush;
        public Brush NavigationButtonBrush => _currentThemeDictionary[nameof(NavigationButtonBrush)] as Brush; 

        public Brush SearchBoxForeground => _currentThemeDictionary[nameof(SearchBoxForeground)] as Brush;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
                    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public void LoadTheme(string path)
        {
            _currentThemeDictionary = new ResourceDictionary();
            App.LoadComponent(_currentThemeDictionary, new Uri(path));
            CurrentTheme = Path.GetFileNameWithoutExtension(path);

            RaisePropertyChanged();
        }

        public void LoadThemeByMode(string mode)
        {
            switch (mode)
            {
                case "1":
                    CurrentTheme = LightThemePath;
                    LoadTheme(LightThemePath);
                    App.ThemeMode= mode;
                    break;
                case "2":
                    CurrentTheme = DarkTheme;
                    LoadTheme(DarkTheme);
                    App.ThemeMode = mode;
                    break;
                case "3":
                    CurrentTheme = DarkBlueThemePath;
                    LoadTheme(DarkBlueThemePath);
                    App.ThemeMode = mode;
                    break;
                case "4":
                    CurrentTheme = PinkRedThemePath;
                    LoadTheme(PinkRedThemePath);
                    App.ThemeMode = mode;
                    break;

                default:
                    CurrentTheme = DarkBlueThemePath;
                    LoadTheme(DarkBlueThemePath);
                    break;
            }
        }

            public async Task LoadThemeFromFile(StorageFile file)
        {
            string xaml = await FileIO.ReadTextAsync(file);
            _currentThemeDictionary = XamlReader.Load(xaml) as ResourceDictionary;
            CurrentTheme = Path.GetFileNameWithoutExtension(file.Path);

            RaisePropertyChanged();
        }
        private void RaisePropertyChanged()
        {
            OnPropertyChanged(nameof(BackgroundBrush));
            OnPropertyChanged(nameof(SecondBrush));
            OnPropertyChanged(nameof(ThirdBrush));
            OnPropertyChanged(nameof(AppTitleBrush));
            OnPropertyChanged(nameof(ForegroundBrush));
            OnPropertyChanged(nameof(NavigationButtonBrush));
            OnPropertyChanged(nameof(CurrentTheme));
        }
    }
}
