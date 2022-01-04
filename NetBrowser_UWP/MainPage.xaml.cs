using System;
using System.Drawing;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using muxc = Microsoft.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace NetBrowser_UWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static muxc.TabViewItem currentSelectedTab = null;
        private static WebView currentSelectedWeb = null;
        private static readonly Uri homeUrl = new Uri("https://google.com");
        private static muxc.TabViewItem setTab = null;
        public MainPage()
        {
            this.InitializeComponent();
            browser = new WebView(WebViewExecutionMode.SeparateProcess);
            browser.Navigate(homeUrl);
            browser.NavigationCompleted += browser_NavigationCompleted;
            browser.NewWindowRequested += browser_NewWindowRequested;
            browser.NavigationStarting += browser_NavigationStarting;
            browser.ContainsFullScreenElementChanged += webView_ContainsFullScreenElementChanged;
            defaultTab.Content = browser;


        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelectedWeb.CanGoBack)
                currentSelectedWeb.GoBack();
        }

        private void forwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelectedWeb.CanGoForward)
                currentSelectedWeb.GoForward();
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            currentSelectedWeb.Refresh();
        }

        private void searchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                searchBtn_Click(sender, e);
            }

        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchWeb();
        }

        private void browser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            searchBox.Text = browser.Source.ToString();
            currentSelectedTab.Header = sender.DocumentTitle;
            Uri icoURI = new Uri("https://www.google.com/s2/favicons?domain=" + sender.Source);
            currentSelectedTab.IconSource = new muxc.BitmapIconSource() { UriSource = icoURI, ShowAsMonochrome = false };
        }

        private void browser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            appTitle.Text = "NetBrowser" + " | " + sender.DocumentTitle;
            Uri icoURI = new Uri("https://www.google.com/s2/favicons?domain=" + sender.Source);
            currentSelectedTab.Header = sender.DocumentTitle;
            currentSelectedTab.IconSource = new muxc.BitmapIconSource() { UriSource = icoURI, ShowAsMonochrome = false };
            searchBox.Text = sender.Source.AbsoluteUri;

        }

        private void browser_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {

            args.Handled = true;
            WebView wb = new WebView(WebViewExecutionMode.SeparateProcess);
            wb.Navigate(args.Uri);
            wb.NavigationCompleted += browser_NavigationCompleted;
            wb.NewWindowRequested += browser_NewWindowRequested;
            wb.NavigationStarting += browser_NavigationStarting;
            wb.ContainsFullScreenElementChanged += webView_ContainsFullScreenElementChanged;
            var newTab = new muxc.TabViewItem
            {
                Header = sender.DocumentTitle.ToString(),
                Content = wb
            };

            tabView.TabItems.Add(newTab);
            tabView.SelectedItem = newTab;


        }

        private void tabView_AddTabButtonClick(muxc.TabView sender, object args)
        {
            WebView wb = new WebView(WebViewExecutionMode.SeparateProcess);
            wb.Navigate(homeUrl);
            wb.NavigationCompleted += browser_NavigationCompleted;
            wb.NewWindowRequested += browser_NewWindowRequested;
            wb.NavigationStarting += browser_NavigationStarting;
            wb.ContainsFullScreenElementChanged += webView_ContainsFullScreenElementChanged;
            var newTab = new muxc.TabViewItem
            {
                Header = wb.DocumentTitle,
                Content = wb,

            };

            sender.TabItems.Add(newTab);
            sender.SelectedItem = newTab;
        }

        private void tabView_TabCloseRequested(muxc.TabView sender, muxc.TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }

        private void webView_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            if (sender.ContainsFullScreenElement)
            {
                applicationView.TryEnterFullScreenMode();
            }
            else if (applicationView.IsFullScreenMode)
            {
                applicationView.ExitFullScreenMode();
            }
        }

        private void SearchWeb()
        {
            if (currentSelectedWeb == null)
            {
                browser.Source = new Uri("https://www.google.ru/search?q=" + searchBox.Text);
            }
            if (searchBox.Text.Contains("https://"))
            {
                browser.Source = new Uri(searchBox.Text);
            }
            if(searchBox.Text == "app://settings")
            {
                AddSettingsTab();
            }

            else
            {
                currentSelectedWeb.Source = new Uri("https://www.google.ru/search?q=" + searchBox.Text);
            }
        }
        private void tabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                currentSelectedTab = tabView.SelectedItem as muxc.TabViewItem;
                if (currentSelectedTab == setTab)
                {
                    appTitle.Text = "NetBrowser | Настройки";
                    searchBox.Text = "app://settings";
                }
                else if (currentSelectedTab != null)
                {
                    currentSelectedWeb = currentSelectedTab.Content as WebView;
                    appTitle.Text = "NetBrowser" + " | " + currentSelectedWeb.DocumentTitle;
                    searchBox.Text = currentSelectedWeb.Source.AbsoluteUri;
                }



            }
            catch (Exception) { };


        }

        private void AddSettingsTab()
        {
            setTab = new muxc.TabViewItem
            {
                Header = "Настройки",
                IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.Setting }

            };
            Frame setFrame = new Frame();
            setTab.Content = setFrame;
            setFrame.Navigate(typeof(SettingsPage));


            tabView.TabItems.Add(setTab);
            tabView.SelectedItem = setTab;
        }
        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!tabView.TabItems.Contains(setTab))
                AddSettingsTab();
        }
    }
}
