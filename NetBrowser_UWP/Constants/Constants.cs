using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBrowser_UWP.Constants
{
    public static class Constants
    {
        public const string DarkBlueThemePath = "ms-appx:///Themes/Theme.DarkBlue.xaml";
        public const string LightThemePath = "ms-appx:///Themes/Theme.Light.xaml";
        public const string LightPinkThemePath = "ms-appx:///Themes/Theme.LightPink.xaml";
        public const string DarkThemePath = "ms-appx:///Themes/Theme.Dark.xaml";
        public const string LightBlueThemePath = "ms-appx:///Themes/Theme.LightBlue.xaml";
        public const string DarkNavyBlue = "ms-appx:///Themes/Theme.Dark.NavyBlue.xaml";
        public const string DarkAcrylic = "ms-appx:///Themes/Theme.Dark.Acrylic.xaml";


        public static Dictionary<int, (string, int)> Themes = new Dictionary<int, (string, int)>()
        {
            {1, (LightThemePath,1) },
            {2, (DarkThemePath,2) },
            {3, (DarkBlueThemePath,2)},
            {4, (LightPinkThemePath,1) },
            {5, (LightBlueThemePath,1) },
            {6, (DarkNavyBlue, 2) },
            {7, (DarkAcrylic, 2) },
        };
    }
}
