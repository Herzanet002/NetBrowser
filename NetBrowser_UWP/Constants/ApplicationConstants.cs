using System.Collections.Generic;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Constants;

public static class ApplicationConstants
{
    public const string FAVICONS_SERVICE = "https://www.google.com/s2/favicons?sz=32&domain_url=";

    // Light Themes
    public static readonly ThemeItem LightTheme = new(LIGHT_THEME_PATH);

    public static readonly ThemeItem LightPinkTheme = new(LIGHT_PINK_THEME_PATH);

    public static readonly ThemeItem LightBlueTheme = new(LIGHT_BLUE_THEME_PATH);

    public static readonly ThemeItem LightAcrylicTheme = new(LIGHT_ACRYLIC_THEME_PATH);

    public static readonly ThemeItem LightLilacTheme = new(LIGHT_LILAC_THEME_PATH);

    // Dark Themes
    public static readonly ThemeItem DarkTheme = new(DARK_THEME_PATH);

    public static readonly ThemeItem DarkBlueTheme = new(DARK_BLUE_THEME_PATH);

    public static readonly ThemeItem DarkNavyBlueTheme = new(DARK_NAVY_BLUE_THEME_PATH);

    public static readonly ThemeItem DarkAcrylicTheme = new(DARK_ACRYLIC_THEME_PATH);


    public static readonly Dictionary<string, ThemeItem> ThemesDictionary = new()
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

    #region ACCESS_NAMES

    public const string HISTORY_COLLECTION_NAME = "history";
    public const string BOOKMARKS_COLLECTION_NAME = "bookmarks";
    public const string SEARCH_ENGINES_COLLECTION_NAME = "search_engines";
    public const string STARTPAGE_ITEMS_COLLECTION_NAME = "startpage_items";
    public const string SEARCHTERMS_COLLECTION_NAME = "search_term";
    public const string FAVORITE_NEWS_COLLECTION_NAME = "favorite_news";
    public const string RSS_FEEDERS_COLLECTION_NAME = "rss_feeders";
    public const string LIKED_RSS_FEEDERS_COLLECTION_NAME = "liked_rss_feeders";

    #endregion

    public const string SETTINGS_ADDRESS = "app://settings";
    public const string NEWS_ADDRESS = "app://news";
}