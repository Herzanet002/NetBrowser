using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace NetBrowser_UWP.Models
{
    public class ThemeItem
    {
        public string Name { get; set; }

        public string Uri { get; set; }
        public ElementTheme Mode { get; set; }
        public Brush BackgroundBrush { get; set; }
        public Brush SecondBrush { get; set; }
        public Brush ThirdBrush { get; set; }
        public Brush AppTitleBrush { get; set; }
        public Brush ForegroundBrush { get; set; }
        public Brush NavigationButtonBrush { get; set; }
        public Brush SearchBoxBorderBrush { get; set; }
        public Brush SearchBoxForeground { get; set; }
        public Brush BookmarkSavedBrush { get; set; }

        public ThemeItem(string path, ElementTheme mode)
        {
            Name = Path.GetFileNameWithoutExtension(path);
            Uri = path;
            Mode = mode;

            var resourceDictionary = new ResourceDictionary();
            Application.LoadComponent(resourceDictionary, new Uri(path));

            BackgroundBrush = resourceDictionary[nameof(BackgroundBrush)] as Brush;
            SecondBrush = resourceDictionary[nameof(SecondBrush)] as Brush;
            ThirdBrush = resourceDictionary[nameof(ThirdBrush)] as Brush;

            AppTitleBrush = resourceDictionary[nameof(AppTitleBrush)] as Brush;
            ForegroundBrush = resourceDictionary[nameof(ForegroundBrush)] as Brush;
            NavigationButtonBrush = resourceDictionary[nameof(NavigationButtonBrush)] as Brush;

            SearchBoxForeground = resourceDictionary[nameof(SearchBoxForeground)] as Brush;
            SearchBoxBorderBrush = resourceDictionary[nameof(SearchBoxBorderBrush)] as Brush;

            BookmarkSavedBrush = resourceDictionary[nameof(BookmarkSavedBrush)] as Brush;

        }


    }
}
