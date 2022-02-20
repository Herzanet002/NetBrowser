using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using NetBrowser_UWP.Views.Settings;
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

            if (args.IsSettingsSelected) return;
            switch (tag)
            {
                case "mainItem":
                    ContentFrame.Navigate(typeof(MainItemPageSettings), null, new EntranceNavigationTransitionInfo());
                    CurrentMode = 0;
                    break;
                case "personalizeItem":
                    ContentFrame.Navigate(typeof(PersonalizePageSettings), null, new EntranceNavigationTransitionInfo());
                    CurrentMode = 1;
                    break;
                case "searchItem":
                    ContentFrame.Navigate(typeof(SearchSystemPageSettings), null, new EntranceNavigationTransitionInfo());
                    CurrentMode = 2;
                    break;
                case "aboutBrowserItem":
                    ContentFrame.Navigate(typeof(AboutAppPage), null, new EntranceNavigationTransitionInfo());
                    CurrentMode = 4;
                    break;
                case "bookmarksItem":
                    ContentFrame.Navigate(typeof(BookmarksPage), null, new EntranceNavigationTransitionInfo());
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

        private void settingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            switch (CurrentMode)
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
            }
        }
    }
}
