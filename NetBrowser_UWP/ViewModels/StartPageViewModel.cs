using NetBrowser_UWP.Commands;
using NetBrowser_UWP.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;
using NetBrowser_UWP.Views.Controls;

namespace NetBrowser_UWP.ViewModels
{
    public class StartPageViewModel : Base.ViewModel

    {
        private static Uri _logoSource;
        private static string _placeholderText;
        private static SiteItem _gridViewSelectedItem;
        private static string _searchBoxText;
        private static List<SiteItem> _startPageItems;
        private static string _newSiteUrl;
        private static string _newSiteName;
        private static MainPage MainPage => (Window.Current.Content as Frame)?.Content as MainPage;
        //public ICommand LoadedEventCommand => new Command(StartPageLoadedEvent, _ => true);
        public ICommand GridViewItemDeleteCommand => new Command(GridViewItemDeleteCommand_Executed, _ => true);
        public ICommand SearchButtonTappedCommand => new Command(SearchButtonTappedCommand_Executed, _ => true);
        public ICommand AddNewSiteCommand => new Command(AddNewSiteCommand_Executed, _ => true);
        public ICommand SaveNewSiteCommand => new Command(SaveNewSiteCommand_Executed, _ => true);

        public SiteItem GridViewSelectedItem
        {
            get => _gridViewSelectedItem;
            set
            {
                Set(ref _gridViewSelectedItem, value);
                if(value != null)
                    MainPage.SearchWebFromStartPage(value.Url);
            }
        }

        public string PlaceholderText
        {
            get => _placeholderText;
            set => Set(ref _placeholderText, value);
        }
        public string NewSiteUrl
        {
            get => _newSiteUrl;
            set => Set(ref _newSiteUrl, value);
        }
        public string NewSiteName
        {
            get => _newSiteName;
            set => Set(ref _newSiteName, value);
        }
        public Uri LogoSource
        {
            get => _logoSource;
            set => Set(ref _logoSource, value);
        }
        public string SearchBoxText
        {
            get => _searchBoxText;
            set => Set(ref _searchBoxText, value);
        }
        public List<SiteItem> StartPageItems
        {
            get => _startPageItems;
            set => Set(ref _startPageItems, value);
        }

        private void GridViewItemDeleteCommand_Executed(object obj)
        {
            if (obj is not SiteItem elem) return;
            DataTransfer.RemoveSiteOnStartPage(elem);
            GetStartPageElementsAsync();
        }

        private void SaveNewSiteCommand_Executed(object obj)
        {
            if (NewSiteName == string.Empty && NewSiteUrl == string.Empty) return;
            if (!(NewSiteUrl.Contains("http://") || NewSiteUrl.Contains("https://")))
                NewSiteUrl = "https://" + NewSiteUrl;
            DataTransfer.AddNewSiteOnStartPage(new SiteItem
            {
                Name = NewSiteName,
                Url = NewSiteUrl,
            });

        }
        public StartPageViewModel()
        {
            GetStartPageElementsAsync();
            SearchBoxText = string.Empty;
            NewSiteName = string.Empty;
            NewSiteUrl = string.Empty;
            var currentWebEngineName = App.CurrentWebEngine.Name;
            if (currentWebEngineName == null) return;
            LogoSource = new Uri($"ms-appx:///Resources/Logos/{currentWebEngineName}Logo.png");
            PlaceholderText = "Искать с помощью " + currentWebEngineName;
        }
        private async void AddNewSiteCommand_Executed(object obj)
        {
            ContentDialog addSiteDialog = new AddNewStartPageItemDialog();
            await addSiteDialog.ShowAsync();
            GetStartPageElementsAsync();
        }

        private void SearchButtonTappedCommand_Executed(object obj)
        {

            if (SearchBoxText == null) return;
            if (SearchBoxText.Contains("https://") || SearchBoxText.Contains("http://"))
            {
                MainPage.SearchWebFromStartPage(SearchBoxText);
                return;
            }
            MainPage.SearchWebFromStartPage(App.CurrentWebEngine.Prefix + SearchBoxText);
        }

        private async void GetStartPageElementsAsync()
        {
            if (StartPageItems != null)
            {
                StartPageItems.Clear();
                StartPageItems = await DataTransfer.GetStartPageElements();
            }
            StartPageItems = await DataTransfer.GetStartPageElements();

        }
    }

    

    
    

}
