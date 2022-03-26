using NetBrowser_UWP.Commands;
using NetBrowser_UWP.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using NetBrowser_UWP.Views;

namespace NetBrowser_UWP.ViewModels
{
    internal class StartPageViewModel : Base.ViewModel

    {
        private static List<BookmarkDetails> _gridViewListSource;
        private static Uri _logoSource;
        private static string _placeholderText;
        private static BookmarkDetails _gridViewDelectedItem;
        private static MainPage MainPage => (Window.Current.Content as Frame)?.Content as MainPage;
        public ICommand LoadedEventCommand => new Command(StartPageLoadedEvent, _ => true);
        public ICommand GridViewItemTappedCommand => new Command(GridViewItemTappedCommand_Executed, _ => true);

        public BookmarkDetails GridViewSelectedItem
        {
            get => _gridViewDelectedItem;
            set => Set(ref _gridViewDelectedItem, value);
        }
        public string PlaceholderText
        {
            get => _placeholderText;
            set => Set(ref _placeholderText, value);
        }
        public Uri LogoSource
        {
            get => _logoSource;
            set => Set(ref _logoSource, value);
        }

        public void GridViewItemTappedCommand_Executed(object obj)
        {
            if (GridViewSelectedItem != null)
            {
                MainPage.SearchWebFromStartPage(GridViewSelectedItem.Url);
                
            }
            
        }

        private void StartPageLoadedEvent(object obj)
        {
            GetGridViewListSources();
            var currentWebEngineName = App.CurrentWebEngine.Name;
            if (currentWebEngineName == null) return;

            LogoSource = new Uri($"ms-appx:///Resources/Logos/{currentWebEngineName}Logo.png");
            PlaceholderText = "Искать с помощью " + currentWebEngineName;
            
        }
        private async void GetGridViewListSources()
        {
            GridViewListSource = await DataTransfer.GetBookmarkList();

        }

        public List<BookmarkDetails> GridViewListSource
        {
            get => _gridViewListSource;
            set => Set(ref _gridViewListSource, value);
        }
    }

    public class TitleTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            var title = value as string;
            if (title is { Length: > 12 })
                title = title.Substring(0, 12) + "...";
            return title;


        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class TitleLetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var rx = new Regex(@"^((https?|ftp)://)?(www\.)?(?<domain>[^/]+)(/|$)");
            if (value is not string text) return string.Empty;
            var match = rx.Match(text);
            return match.Success ? match.Groups["domain"].Value[0].ToString().ToUpper() : string.Empty;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }

}
