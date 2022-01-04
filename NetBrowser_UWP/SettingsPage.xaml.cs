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
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (muxc.NavigationViewItem)sender.SelectedItem;
            string tag = selectedItem.Tag.ToString();

            if (!args.IsSettingsSelected)
            {
                if(tag == "mainItem")
                {
                    contentFrame.Navigate(typeof(MainItemPageSettings), null, args.RecommendedNavigationTransitionInfo);
                }
                else if(tag == "personalizeItem")
                {
                    contentFrame.Navigate(typeof(PersonalizePageSettings), null, args.RecommendedNavigationTransitionInfo);
                }
                else if (tag == "searchItem")
                {
                    contentFrame.Navigate(typeof(SearchSystemPageSettings), null, args.RecommendedNavigationTransitionInfo);
                }
                else if (tag == "aboutBrowserItem")
                {
                    contentFrame.Navigate(typeof(AboutAppPage), null, args.RecommendedNavigationTransitionInfo);
                }
            }
            
        }
    }
}
