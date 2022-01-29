using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using muxc = Microsoft.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace NetBrowser_UWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static muxc.TabViewItem currentSelectedTab = null;
        public static WebView currentSelectedWeb = null;
        private static readonly Uri homeUrl = new Uri("https://google.com");
        private static muxc.TabViewItem setTab = null;
        public List<BookmarkDetails> bookmarksList;

        public static MainPage mainPage;
        public MainPage()
        {
            this.InitializeComponent();
            DataAccess dataAccess = new DataAccess();
            dataAccess.CreateSettingsFile();
            mainPage = this;
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
                SearchWeb();
            }

        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchWeb();
        }

        private void browser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            searchBox.Text = currentSelectedWeb.Source.ToString();
            //progressRing.IsActive = true;
            browserProgress.IsEnabled = true;
            browserProgress.Visibility = Visibility.Visible;


        }
        
        private void browser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            browserProgress.IsEnabled = false;
            browserProgress.Visibility = Visibility.Collapsed;
            try
            {
                appTitle.Text = "NetBrowser" + " | " + sender.DocumentTitle;
                Uri icoURI = new Uri("https://www.google.com/s2/favicons?domain=" + sender.Source);
                currentSelectedTab.Header = sender.DocumentTitle;
                currentSelectedTab.IconSource = new muxc.BitmapIconSource() { UriSource = icoURI, ShowAsMonochrome = false };
                searchBox.Text = sender.Source.AbsoluteUri;
                //progressRing.IsActive = false;


                DataTransfer dataTransfer = new DataTransfer();
                if (!string.IsNullOrEmpty(searchBox.Text))
                    dataTransfer.SaveHistory(currentSelectedWeb.DocumentTitle, currentSelectedWeb.Source.AbsoluteUri);
            }
            catch { }


        }

        private void browser_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {

            args.Handled = true;
            CreateNewWebTab();
            SearchWeb(args.Uri);

        }

        
        private void CreateNewWebTab()
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

            tabView.TabItems.Add(newTab);
            tabView.SelectedItem = newTab;
        }

        public void CreateNewWebTab(string url)
        {
            WebView wb = new WebView(WebViewExecutionMode.SeparateProcess);
            wb.Navigate(new Uri(url));
            wb.NavigationCompleted += browser_NavigationCompleted;
            wb.NewWindowRequested += browser_NewWindowRequested;
            wb.NavigationStarting += browser_NavigationStarting;
            wb.ContainsFullScreenElementChanged += webView_ContainsFullScreenElementChanged;
            var newTab = new muxc.TabViewItem
            {
                Header = wb.DocumentTitle,
                Content = wb,

            };

            tabView.TabItems.Add(newTab);
            tabView.SelectedItem = newTab;
        }

        private void tabView_AddTabButtonClick(muxc.TabView sender, object args)
        {
            CreateNewWebTab();
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

        public void SearchWeb()
        {
            if (searchBox.Text.Contains("https://"))
            {
                currentSelectedWeb.Source = new Uri(searchBox.Text);
            }
            else if (currentSelectedWeb == null)
            {
                currentSelectedWeb.Source = new Uri("https://www.google.ru/search?q=" + searchBox.Text);
            }

            else if (searchBox.Text == "app://settings")
            {
                AddSettingsTab(0);
            }

            else
            {
                currentSelectedWeb.Source = new Uri("https://www.google.ru/search?q=" + searchBox.Text);
            }
        }

        public void SearchWeb(Uri uri)
        {

            currentSelectedWeb.Navigate(uri);


        }
        private void tabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                currentSelectedTab = tabView.SelectedItem as muxc.TabViewItem;
                currentSelectedWeb = currentSelectedTab.Content as WebView;
                if (currentSelectedTab == setTab)
                {
                    appTitle.Text = "NetBrowser | Настройки";
                    searchBox.Text = "app://settings";
                }
                else if (currentSelectedTab != null)
                {
                    currentSelectedWeb = currentSelectedTab.Content as WebView;
                    if(currentSelectedWeb.DocumentTitle != null)
                    {
                        appTitle.Text = "NetBrowser" + " | " + currentSelectedWeb.DocumentTitle;
                        searchBox.Text = currentSelectedWeb.Source.AbsoluteUri;
                    }
                    
                }



            }
            catch (Exception) { };

        }

        private void AddSettingsTab(int mode)
        {
            setTab = new muxc.TabViewItem
            {
                Header = "Настройки",
                IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.Setting }

            };
            Frame setFrame = new Frame();
            setTab.Content = setFrame;
            setFrame.Navigate(typeof(SettingsPage));
            SettingsPage.currentMode = mode;

            tabView.TabItems.Add(setTab);
            tabView.SelectedItem = setTab;
            
        }
        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!tabView.TabItems.Contains(setTab))
                AddSettingsTab(0);
        }

        private async void historyBtn_Click(object sender, RoutedEventArgs e)
        {
            historyListView.Items.Clear();
            DataTransfer dataTransfer = new DataTransfer();
            List<string> historyList = await dataTransfer.GetHistory("url");


            for (int i = historyList.Count - 1; i >= 0; i--)
            {
                historyListView.Items.Add(historyList[i]);

            }

        }

        private void CreateErrorLoadPage(string url)
        {
            WebView webView = new WebView();
            try
            {
                webView.Navigate(new Uri(url));
            }
            catch (Exception ex)
            {
                ErrorLoadPage.error = ex.Message;
            }
            var errorTab = new muxc.TabViewItem
            {
                Header = "Ошибка",
                IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.Cancel }

            };
            Frame setFrame = new Frame();
            errorTab.Content = setFrame;
            setFrame.Navigate(typeof(ErrorLoadPage));
            
            

            tabView.TabItems.Add(errorTab);
            tabView.SelectedItem = errorTab;
        }

        private async void historyListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            string url = e.ClickedItem.ToString();
            bool isUri = Uri.IsWellFormedUriString(url, UriKind.Absolute);
            if (isUri)
            {
                CreateNewWebTab();
                SearchWeb(new Uri(url));
                flyoutHistory.Hide();
            }
            else
            {
                CreateErrorLoadPage(url);
                flyoutHistory.Hide();
                var dialogError = new ContentDialog();
                dialogError.Title = "Неверная ссылка";
                dialogError.Content = "Ссылка " + url + " недействительна или неверна";
                dialogError.CloseButtonText = "Закрыть";
                
                await dialogError.ShowAsync();
                

            }
        }

        private void addBookmarksBtn_Click(object sender, RoutedEventArgs e)
        {
            bookmarkTitle.Text = currentSelectedWeb.DocumentTitle;
            bookmarkUrl.Text = currentSelectedWeb.Source.AbsoluteUri.ToString();

        }

        private void CancelBookmarksBtn_Click(object sender, RoutedEventArgs e)
        {
            bookmarkFlyout.Hide();
        }

        private async void SaveBookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (bookmarkTitle.Text != String.Empty && bookmarkUrl.Text != String.Empty &&
                Uri.IsWellFormedUriString(bookmarkUrl.Text, UriKind.Absolute))
            {
                DataTransfer dataTransfer = new DataTransfer();
                dataTransfer.SaveBookmark(bookmarkTitle.Text, bookmarkUrl.Text);

                var fontIcon = new FontIcon();
                fontIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                fontIcon.FontSize = 16;
                fontIcon.Glyph = "\xE735";
                addbookmarksBtn.Content = fontIcon;
                bookmarkFlyout.Hide();
            }
            else
            {
                var dialogError = new ContentDialog();
                dialogError.Title = "Неверные данные";
                dialogError.Content = "Введите верные параметры";
                dialogError.CloseButtonText = "Закрыть";

                await dialogError.ShowAsync();
                
            }
            
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            addbookmarksBtn.Margin = new Thickness(20, 0, 45, 0);
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            addbookmarksBtn.Margin = new Thickness(20, 0, 20, 0);
        }

        public async void GetBookmarks()
        {
            DataTransfer dataTransfer = new DataTransfer();
            List<BookmarkDetails> bookmarks = await dataTransfer.GetBookmarkList();

            bookmarks.Reverse();
            bookmarksFlyoutListView.ItemsSource = bookmarks;

        }
        private void bookmarksBtn_Click(object sender, RoutedEventArgs e)
        {
            GetBookmarks();
        }

        private void bookmarkSettingBtn_Click(object sender, RoutedEventArgs e)
        {

            AddSettingsTab(3);
            flyoutBookmarks.Hide();
        }

        private void bookmarksFlyoutListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            BookmarkDetails a = (BookmarkDetails)e.ClickedItem;
            CreateNewWebTab();
            SearchWeb(new Uri(a.Url));
           
            flyoutBookmarks.Hide();
            
        }

        private void browser_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            Debug.WriteLine(sender.ToString());
        }
    }
}
