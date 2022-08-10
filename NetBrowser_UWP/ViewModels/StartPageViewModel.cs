using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Views.Controls;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace NetBrowser_UWP.ViewModels
{
    public class StartPageViewModel : ObservableObject
    {
        private static Uri _logoSource;
        private static string _placeholderText;
        private static SiteItem _gridViewSelectedItem;
        private static string _searchBoxText;
        private static HashSet<SiteItem> _startPageItems;
        private static HashSet<SiteItem> _recentlySearchedItems;
        private static string _newSiteUrl;
        private static string _newSiteName;
        private bool _isSuggestionBarEnabled;

        public ICommand GridViewItemDeleteCommand => new DelegateCommand<object>(OnGridViewItemDeleteCommandExecuted, _ => true);
        public ICommand SearchButtonTappedCommand => new DelegateCommand(OnSearchButtonTappedCommandExecuted, () => true);
        public ICommand AddNewSiteCommand => new DelegateCommand(OnAddNewSiteCommandExecuted, () => true);
        public ICommand SaveNewSiteCommand => new DelegateCommand(OnSaveNewSiteCommandExecuted, () => true);
        public ICommand KeyDownCommand => new DelegateCommand<object>(OnKeyDownCommandExecuted, _ => true);

        private void OnKeyDownCommandExecuted(object obj)
        {
            if (obj is not KeyRoutedEventArgs { Key: VirtualKey.Enter }) return;
            OnSearchButtonTappedCommandExecuted();
        }

        public bool IsSuggestionBarEnabled
        {
            get => _isSuggestionBarEnabled;
            set => SetProperty(ref _isSuggestionBarEnabled, value);
        }

        public SiteItem GridViewSelectedItem
        {
            get => _gridViewSelectedItem;
            set
            {
                SetProperty(ref _gridViewSelectedItem, value);
                if (value == null) return;
                _mainPageViewModel.SearchWebFromStartPage(value.Url);
            }
        }

        public string PlaceholderText
        {
            get => _placeholderText;
            set => SetProperty(ref _placeholderText, value);
        }
        public string NewSiteUrl
        {
            get => _newSiteUrl;
            set => SetProperty(ref _newSiteUrl, value);
        }
        public string NewSiteName
        {
            get => _newSiteName;
            set => SetProperty(ref _newSiteName, value);
        }
        public Uri LogoSource
        {
            get => _logoSource;
            set => SetProperty(ref _logoSource, value);
        }
        public string SearchBoxText
        {
            get => _searchBoxText;
            set => SetProperty(ref _searchBoxText, value);
        }
        public HashSet<SiteItem> StartPageItems
        {
            get => _startPageItems;
            set => SetProperty(ref _startPageItems, value);
        }
        public HashSet<SiteItem> RecentlySearchedItems
        {
            get => _recentlySearchedItems;
            set => SetProperty(ref _recentlySearchedItems, value);
        }
        private void OnGridViewItemDeleteCommandExecuted(object obj)
        {
            if (obj is not SiteItem elem) return;
            _dataTransferService.RemoveSiteOnStartPage(elem);
            GetStartPageElementsAsync();
        }

        private void OnSaveNewSiteCommandExecuted()
        {
            if (string.IsNullOrEmpty(NewSiteName) || string.IsNullOrEmpty(NewSiteUrl)) return;
            if (!(NewSiteUrl.StartsWith("http://") || NewSiteUrl.StartsWith("https://")))
                NewSiteUrl = "https://" + NewSiteUrl;
            _dataTransferService.AddNewSiteOnStartPage(new SiteItem
            {
                Name = NewSiteName,
                Url = NewSiteUrl,
            });

        }
        private readonly IDataTransferService _dataTransferService;
        private readonly MainPageViewModel _mainPageViewModel;
        private readonly ILocalSettingsService _localSettingsService;

        public StartPageViewModel(IDataTransferService dataTransferService, MainPageViewModel mainViewModel, ILocalSettingsService localSettingsService)
        {
            _dataTransferService = dataTransferService;
            _mainPageViewModel = mainViewModel;
            _localSettingsService = localSettingsService;
            InitializePageComponents();
        }

        private async void InitializePageComponents()
        {
            IsSuggestionBarEnabled = await _localSettingsService.ReadSettingAsync<bool>("IsSuggestionBarEnabled");

            SearchBoxText = string.Empty;
            NewSiteName = string.Empty;
            NewSiteUrl = string.Empty;

            var currentWebEngineName = App.CurrentWebEngine.Name;
            if (currentWebEngineName == null) return;
            LogoSource = new Uri($"ms-appx:///Resources/Logos/{currentWebEngineName}Logo.png");
            PlaceholderText = "Искать с помощью " + currentWebEngineName;

            GetStartPageElementsAsync();
            if (IsSuggestionBarEnabled) GetRecentlySearchedItemsAsync();
        }

        private async void GetRecentlySearchedItemsAsync()
        {
            var searchTermListTransfer = await _dataTransferService.GetSearchTerm();
            if (searchTermListTransfer == null) return;
            searchTermListTransfer.Reverse();
            RecentlySearchedItems = new HashSet<SiteItem>(searchTermListTransfer);
        }

        private async void OnAddNewSiteCommandExecuted()
        {
            await new AddNewStartPageItemDialog().ShowAsync();
            GetStartPageElementsAsync();
        }

        private void OnSearchButtonTappedCommandExecuted()
        {
            if (string.IsNullOrEmpty(SearchBoxText) || string.IsNullOrWhiteSpace(SearchBoxText)) return;
            if (SearchBoxText.StartsWith("https://") || SearchBoxText.StartsWith("http://"))
            {
                _mainPageViewModel.SearchWebFromStartPage(SearchBoxText);
                return;
            }
            _mainPageViewModel.SearchWebFromStartPage(App.CurrentWebEngine.Prefix + SearchBoxText);
        }

        private async void GetStartPageElementsAsync()
        {
            StartPageItems?.Clear();
            StartPageItems = new HashSet<SiteItem>(await _dataTransferService.GetStartPageElements());
        }
    }

}
