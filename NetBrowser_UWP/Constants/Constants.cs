using System.Collections.Generic;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Constants;

public static class Constants
{
    public const string FAVICONS_SERVICE = "https://www.google.com/s2/favicons?sz=32&domain_url=";

    // Light Themes
    public static ThemeItem LightTheme = new(LIGHT_THEME_PATH);

    public static ThemeItem LightPinkTheme = new(LIGHT_PINK_THEME_PATH);

    public static ThemeItem LightBlueTheme = new(LIGHT_BLUE_THEME_PATH);

    public static ThemeItem LightAcrylicTheme = new(LIGHT_ACRYLIC_THEME_PATH);

    public static ThemeItem LightLilacTheme = new(LIGHT_LILAC_THEME_PATH);

    // Dark Themes
    public static ThemeItem DarkTheme = new(DARK_THEME_PATH);

    public static ThemeItem DarkBlueTheme = new(DARK_BLUE_THEME_PATH);

    public static ThemeItem DarkNavyBlueTheme = new(DARK_NAVY_BLUE_THEME_PATH);

    public static ThemeItem DarkAcrylicTheme = new(DARK_ACRYLIC_THEME_PATH);


    public static Dictionary<string, ThemeItem> ThemesDictionary = new()
    {
        { LightTheme.Name, LightTheme },
        { LightPinkTheme.Name, LightPinkTheme },
        { LightBlueTheme.Name, LightBlueTheme },
        { LightAcrylicTheme.Name, LightAcrylicTheme },
        { LightLilacTheme.Name, LightLilacTheme },
        { DarkTheme.Name, DarkTheme },
        { DarkBlueTheme.Name, DarkBlueTheme },
        { DarkNavyBlueTheme.Name, DarkNavyBlueTheme },
        { DarkAcrylicTheme.Name, DarkAcrylicTheme }
    };

    //private const string FAVICONS_SERVICE = "https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=";

    #region THEMES_PATH

    public const string DARK_BLUE_THEME_PATH = "ms-appx:///Themes/Theme.DarkBlue.xaml";
    public const string LIGHT_THEME_PATH = "ms-appx:///Themes/Theme.Light.xaml";
    public const string LIGHT_PINK_THEME_PATH = "ms-appx:///Themes/Theme.LightPink.xaml";
    public const string DARK_THEME_PATH = "ms-appx:///Themes/Theme.Dark.xaml";
    public const string LIGHT_BLUE_THEME_PATH = "ms-appx:///Themes/Theme.LightBlue.xaml";
    public const string DARK_NAVY_BLUE_THEME_PATH = "ms-appx:///Themes/Theme.Dark.NavyBlue.xaml";
    public const string DARK_ACRYLIC_THEME_PATH = "ms-appx:///Themes/Theme.Dark.Acrylic.xaml";
    public const string LIGHT_ACRYLIC_THEME_PATH = "ms-appx:///Themes/Theme.Light.Acrylic.xaml";
    public const string LIGHT_LILAC_THEME_PATH = "ms-appx:///Themes/Theme.Light.Lilac.xaml";

    #endregion

    #region FILE_ACCESS_NAMES

    public const string SETTINGS_FILE_NAME = "configs.xml";
    public const string BOOKMARKS_FILE_NAME = "bookmarks.xml";
    public const string HISTORY_FILE_NAME = "history.xml";
    public const string STARTPAGE_FILE_NAME = "startpage.xml";
    public const string NEWS_CONTENT_FILE_NAME = "newspage.xml";

    #endregion

    #region ADDRESSES

    public const string SETTINGS_ADDRESS = "app://settings";
    public const string STARTPAGE_ADDRESS = "app://newtab";
    public const string NEWS_ADDRESS = "app://news";

    #endregion
}