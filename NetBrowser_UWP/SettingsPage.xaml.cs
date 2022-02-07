using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;
// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            ThemeManager.SetRequestedTheme();
        }

        public static int currentMode = 0;

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender,
            Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (muxc.NavigationViewItem)sender.SelectedItem;
            string tag = selectedItem.Tag.ToString();

            if (!args.IsSettingsSelected)
            {
                if(tag == "mainItem")
                {
                    contentFrame.Navigate(typeof(MainItemPageSettings), null, args.RecommendedNavigationTransitionInfo);
                    currentMode = 0;
                }
                else if(tag == "personalizeItem")
                {
                    contentFrame.Navigate(typeof(PersonalizePageSettings), null, args.RecommendedNavigationTransitionInfo);
                    currentMode = 1;
                }
                else if (tag == "searchItem")
                {
                    contentFrame.Navigate(typeof(SearchSystemPageSettings), null, args.RecommendedNavigationTransitionInfo);
                    currentMode = 2;
                }
                else if (tag == "aboutBrowserItem")
                {
                    contentFrame.Navigate(typeof(AboutAppPage), null, args.RecommendedNavigationTransitionInfo);
                    currentMode = 4;
                }
                else if(tag == "bookmarksItem")
                {
                    contentFrame.Navigate(typeof(BookmarksPage), null, args.RecommendedNavigationTransitionInfo);
                    currentMode = 3;
                }
                else if(tag == "historyFileOpen")
                {
                    DataTransfer.LoadXmlFile("history.xml");
                }
                else if (tag == "bookmarksFileOpen")
                {
                    DataTransfer.LoadXmlFile("bookmarks.xml");
                }
            }
            
        }

        private void settingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            switch (currentMode)
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
