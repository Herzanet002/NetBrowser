using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Constants
{
    public static class Constants
    {
        public const string DARK_BLUE_THEME_PATH = "ms-appx:///Themes/Theme.DarkBlue.xaml";
        public const string LIGHT_THEME_PATH = "ms-appx:///Themes/Theme.Light.xaml";
        public const string LIGHT_PINK_THEME_PATH = "ms-appx:///Themes/Theme.LightPink.xaml";
        public const string DARK_THEME_PATH = "ms-appx:///Themes/Theme.Dark.xaml";
        public const string LIGHT_BLUE_THEME_PATH = "ms-appx:///Themes/Theme.LightBlue.xaml";
        public const string DARK_NAVY_BLUE_THEME_PATH = "ms-appx:///Themes/Theme.Dark.NavyBlue.xaml";
        public const string DARK_ACRYLIC_THEME_PATH = "ms-appx:///Themes/Theme.Dark.Acrylic.xaml";
        public const string LIGHT_ACRYLIC_THEME_PATH = "ms-appx:///Themes/Theme.Light.Acrylic.xaml";

        public const string SETTINGS_FILE_NAME = "configs.xml";
        public const string BOOKMARKS_FILE_NAME = "bookmarks.xml";
        public const string HISTORY_FILE_NAME = "history.xml";
        public const string STARTPAGE_FILE_NAME = "startpage.xml";

        public static Dictionary<int, (string, int)> Themes = new()
        {
            {1, (LIGHT_THEME_PATH, 1) },
            {2, (DARK_THEME_PATH, 2) },
            {3, (DARK_BLUE_THEME_PATH, 2) },
            {4, (LIGHT_PINK_THEME_PATH, 1) },
            {5, (LIGHT_BLUE_THEME_PATH, 1) },
            {6, (DARK_NAVY_BLUE_THEME_PATH, 2) },
            {7, (DARK_ACRYLIC_THEME_PATH, 2) },
            {8, (LIGHT_ACRYLIC_THEME_PATH, 1) },
        };

        // Light Themes
        public static ThemeItem LightTheme = new ThemeItem(LIGHT_THEME_PATH, ElementTheme.Light);

        public static ThemeItem LightPinkTheme = new ThemeItem(LIGHT_PINK_THEME_PATH, ElementTheme.Light);

        public static ThemeItem LightBlueTheme = new ThemeItem(LIGHT_BLUE_THEME_PATH, ElementTheme.Light);

        public static ThemeItem LightAcrylicTheme = new ThemeItem(LIGHT_ACRYLIC_THEME_PATH, ElementTheme.Light);
        

        // Dark Themes
        public static ThemeItem DarkTheme = new ThemeItem(DARK_THEME_PATH, ElementTheme.Dark);

        public static ThemeItem DarkBlueTheme = new ThemeItem(DARK_BLUE_THEME_PATH, ElementTheme.Dark);

        public static ThemeItem DarkNavyBlueTheme = new ThemeItem(DARK_NAVY_BLUE_THEME_PATH, ElementTheme.Dark);

        public static ThemeItem DarkAcrylicTheme = new ThemeItem(DARK_ACRYLIC_THEME_PATH, ElementTheme.Dark);


        public static Dictionary<string, ThemeItem> ThemesDictionary = new Dictionary<string, ThemeItem>
        {
            {LightTheme.Name, LightTheme},
            {LightPinkTheme.Name, LightPinkTheme},
            {LightBlueTheme.Name, LightBlueTheme},
            {LightAcrylicTheme.Name, LightAcrylicTheme},
            {DarkTheme.Name, DarkTheme},
            {DarkBlueTheme.Name, DarkBlueTheme},
            {DarkNavyBlueTheme.Name, DarkNavyBlueTheme},
            {DarkAcrylicTheme.Name, DarkAcrylicTheme},

        };


        public static FontIcon BookmarkExixstsActiveIcon = new()
        {
            FontSize = 14,
            Foreground = Application.Current.Resources["BookmarkAdded"] as Brush,
            Glyph = "\xE735"
        };
        
        public static FontIcon BookmarkExistsUnactiveIcon = new()
        {
            FontSize = 14,
            Glyph = "\xE734"
        };
        public static FontIcon RefreshButtonIcon = new()
        {
            FontSize = 18,
            Glyph = "\xE72C"
        };
        public static FontIcon StopLoadButtonIcon = new()
        {
            FontSize = 22,
            Glyph = "\xE711"
        };
    }
}
