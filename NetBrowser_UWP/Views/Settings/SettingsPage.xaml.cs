using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using muxc = Microsoft.UI.Xaml.Controls;
// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.Settings
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public sealed partial class SettingsPage : Page
    {
        private static int _currentMode;
        public SettingsPage(int mode)
        {
            this.InitializeComponent();
            _currentMode = mode;
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender,
            Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (muxc.NavigationViewItem)sender.SelectedItem;
            var tag = selectedItem.Tag.ToString();

            if (args.IsSettingsSelected) return;
            switch (tag)
            {
                case "mainItem":
                    ContentFrame.Navigate(typeof(MainItemPageSettings), null, new EntranceNavigationTransitionInfo());
                    _currentMode = 0;
                    break;
                case "personalizeItem":
                    ContentFrame.Navigate(typeof(PersonalizePageSettings), null, new EntranceNavigationTransitionInfo());
                    _currentMode = 1;
                    break;
                case "searchItem":
                    ContentFrame.Navigate(typeof(SearchSystemPageSettings), null, new EntranceNavigationTransitionInfo());
                    _currentMode = 2;
                    break;
                case "aboutBrowserItem":
                    ContentFrame.Navigate(typeof(AboutAppPage), null, new EntranceNavigationTransitionInfo());
                    _currentMode = 4;
                    break;
                case "bookmarksItem":
                    ContentFrame.Navigate(typeof(BookmarksPage), null, new EntranceNavigationTransitionInfo());
                    _currentMode = 3;
                    break;
                case "historyItem":
                    ContentFrame.Navigate(typeof(HistoryPageSettings), null, new EntranceNavigationTransitionInfo());
                    _currentMode = 5;
                    break;
                case "historyFileOpen":
                    DataTransfer.LoadXmlFile("history.xml");
                    break;
                case "bookmarksFileOpen":
                    DataTransfer.LoadXmlFile("bookmarks.xml");
                    break;
            }

        }

        private void settingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            switch (_currentMode)
            {
                case 0:
                    ContentFrame.Navigate(typeof(MainItemPageSettings), null);
                    break;
                case 1:
                    ContentFrame.Navigate(typeof(PersonalizePageSettings), null);
                    break;
                case 2:
                    ContentFrame.Navigate(typeof(SearchSystemPageSettings), null);
                    break;
                case 3:
                    ContentFrame.Navigate(typeof(BookmarksPage), null);
                    break;
                case 4:
                    ContentFrame.Navigate(typeof(AboutAppPage), null);
                    break;
                case 5:
                    ContentFrame.Navigate(typeof(HistoryPageSettings), null);
                    break;
            }
        }
    }
}
