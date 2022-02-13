using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using muxc = Microsoft.UI.Xaml.Controls;
// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

        }

        public static int CurrentMode = 0;

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender,
            Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (muxc.NavigationViewItem)sender.SelectedItem;
            string tag = selectedItem.Tag.ToString();

            if (!args.IsSettingsSelected)
            {
                switch (tag)
                {
                    case "mainItem":
                        contentFrame.Navigate(typeof(MainItemPageSettings), null, args.RecommendedNavigationTransitionInfo);
                        CurrentMode = 0;
                        break;
                    case "personalizeItem":
                        contentFrame.Navigate(typeof(PersonalizePageSettings), null, args.RecommendedNavigationTransitionInfo);
                        CurrentMode = 1;
                        break;
                    case "searchItem":
                        contentFrame.Navigate(typeof(SearchSystemPageSettings), null, args.RecommendedNavigationTransitionInfo);
                        CurrentMode = 2;
                        break;
                    case "aboutBrowserItem":
                        contentFrame.Navigate(typeof(AboutAppPage), null, args.RecommendedNavigationTransitionInfo);
                        CurrentMode = 4;
                        break;
                    case "bookmarksItem":
                        contentFrame.Navigate(typeof(BookmarksPage), null, args.RecommendedNavigationTransitionInfo);
                        CurrentMode = 3;
                        break;
                    case "historyFileOpen":
                        DataTransfer.LoadXmlFile("history.xml");
                        break;
                    case "bookmarksFileOpen":
                        DataTransfer.LoadXmlFile("bookmarks.xml");
                        break;
                }
            }

        }

        private void settingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            switch (CurrentMode)
            {
                case 0:
                    contentFrame.Navigate(typeof(MainItemPageSettings), null);
                    break;
                case 1:
                    contentFrame.Navigate(typeof(PersonalizePageSettings), null);
                    break;
                case 2:
                    contentFrame.Navigate(typeof(SearchSystemPageSettings), null);
                    break;
                case 3:
                    contentFrame.Navigate(typeof(BookmarksPage), null);
                    break;
                case 4:
                    contentFrame.Navigate(typeof(AboutAppPage), null);
                    break;
            }
        }
    }
}
