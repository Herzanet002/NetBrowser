using NetBrowser_UWP.Models;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace NetBrowser_UWP.Constants
{
    public static class Constants
    {
        public const string FAVICONS_SERVICE = "https://www.google.com/s2/favicons?sz=32&domain_url=";

        //private const string FAVICONS_SERVICE = "https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=";

        public const string DARK_BLUE_THEME_PATH = "ms-appx:///Themes/Theme.DarkBlue.xaml";
        public const string LIGHT_THEME_PATH = "ms-appx:///Themes/Theme.Light.xaml";
        public const string LIGHT_PINK_THEME_PATH = "ms-appx:///Themes/Theme.LightPink.xaml";
        public const string DARK_THEME_PATH = "ms-appx:///Themes/Theme.Dark.xaml";
        public const string LIGHT_BLUE_THEME_PATH = "ms-appx:///Themes/Theme.LightBlue.xaml";
        public const string DARK_NAVY_BLUE_THEME_PATH = "ms-appx:///Themes/Theme.Dark.NavyBlue.xaml";
        public const string DARK_ACRYLIC_THEME_PATH = "ms-appx:///Themes/Theme.Dark.Acrylic.xaml";
        public const string LIGHT_ACRYLIC_THEME_PATH = "ms-appx:///Themes/Theme.Light.Acrylic.xaml";
        public const string LIGHT_LILAC_THEME_PATH = "ms-appx:///Themes/Theme.Light.Lilac.xaml";

        public const string SETTINGS_FILE_NAME = "configs.xml";
        public const string BOOKMARKS_FILE_NAME = "bookmarks.xml";
        public const string HISTORY_FILE_NAME = "history.xml";
        public const string STARTPAGE_FILE_NAME = "startpage.xml";


        public const string SETTINGS_ADDRESS = "app://settings"; 
        public const string STARTPAGE_ADDRESS = "app://newtab";
        public const string NEWS_ADDRESS = "app://news";

        // Light Themes
        public static ThemeItem LightTheme = new ThemeItem(LIGHT_THEME_PATH);

        public static ThemeItem LightPinkTheme = new ThemeItem(LIGHT_PINK_THEME_PATH);

        public static ThemeItem LightBlueTheme = new ThemeItem(LIGHT_BLUE_THEME_PATH);

        public static ThemeItem LightAcrylicTheme = new ThemeItem(LIGHT_ACRYLIC_THEME_PATH);

        public static ThemeItem LightLilacTheme = new ThemeItem(LIGHT_LILAC_THEME_PATH);

        // Dark Themes
        public static ThemeItem DarkTheme = new ThemeItem(DARK_THEME_PATH);

        public static ThemeItem DarkBlueTheme = new ThemeItem(DARK_BLUE_THEME_PATH);

        public static ThemeItem DarkNavyBlueTheme = new ThemeItem(DARK_NAVY_BLUE_THEME_PATH);

        public static ThemeItem DarkAcrylicTheme = new ThemeItem(DARK_ACRYLIC_THEME_PATH);



        public static Dictionary<string, ThemeItem> ThemesDictionary = new Dictionary<string, ThemeItem>
        {
            {LightTheme.Name, LightTheme},
            {LightPinkTheme.Name, LightPinkTheme},
            {LightBlueTheme.Name, LightBlueTheme},
            {LightAcrylicTheme.Name, LightAcrylicTheme},
            {LightLilacTheme.Name, LightLilacTheme},
            {DarkTheme.Name, DarkTheme},
            {DarkBlueTheme.Name, DarkBlueTheme},
            {DarkNavyBlueTheme.Name, DarkNavyBlueTheme},
            {DarkAcrylicTheme.Name, DarkAcrylicTheme},

        };

    }
}
