using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace NetBrowser_UWP.ViewModels
{
    public class StartPageViewModel : ObservableObject
    {
        private Uri _logoSource;
        private string _placeholderText;
        private SiteItem _gridViewSelectedItem;
        private SiteItem _searchBarSelectedItem;
        private string _searchBoxText;
        private ObservableCollection<SiteItem> _startPageItems;
        private HashSet<SiteItem> _recentlySearchedItems;
        private string _newSiteUrl;
        private string _newSiteName;
        private int _gridViewOrientation;

        private bool _isSuggestionBarEnabled;
        private bool _isFlyoutClosed;
        private bool _isAnimationEnabled;

        public ICommand GridViewItemDeleteCommand => new DelegateCommand<object>(OnGridViewItemDeleteCommandExecuted);
        public ICommand SearchButtonTappedCommand => new DelegateCommand(OnSearchButtonTappedCommandExecuted);
        public ICommand SaveNewSiteCommand => new DelegateCommand(OnSaveNewSiteCommandExecuted);
        public ICommand KeyDownCommand => new DelegateCommand<object>(OnKeyDownCommandExecuted);
        public ICommand CancelCommand => new DelegateCommand(() => IsFlyoutClosed = true);
        public ICommand EditStartPageItem => new DelegateCommand<object>(OnEditStartPageItem);

        private readonly IDataTransferService _dataTransferService;
        private readonly ShellPageViewModel _mainPageViewModel;
        private readonly ILocalSettingsService _localSettingsService;

        //TODO: OnEditStartPageItem
        private void OnEditStartPageItem(object obj)
        {
        }

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

        public bool IsAnimationEnabled
        {
            get => _isAnimationEnabled;
            set => SetProperty(ref _isAnimationEnabled, value);
        }

        public bool IsFlyoutClosed
        {
            get => _isFlyoutClosed;
            set
            {
                SetProperty(ref _isFlyoutClosed, value);
                if (value)
                    IsFlyoutClosed = false;
            }
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

        public SiteItem SearchBarSelectedItem
        {
            get => _searchBarSelectedItem;
            set
            {
                SetProperty(ref _searchBarSelectedItem, value);
                if (value == null) return;
                _mainPageViewModel.SearchWebFromStartPage(value.Name);
            }
        }

        public int GridViewOrientation
        {
            get => _gridViewOrientation;
            set => SetProperty(ref _gridViewOrientation, value);
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

        public ObservableCollection<SiteItem> StartPageItems
        {
            get => _startPageItems;
            set => SetProperty(ref _startPageItems, value);
        }

        public HashSet<SiteItem> RecentlySearchedItems
        {
            get => _recentlySearchedItems;
            set => SetProperty(ref _recentlySearchedItems, value);
        }

        private async void OnGridViewItemDeleteCommandExecuted(object obj)
        {
            if (obj is not SiteItem elem) return;
            await _dataTransferService.RemoveSiteOnStartPage(elem);
            GetStartPageElementsAsync();
        }

        private async void OnSaveNewSiteCommandExecuted()
        {
            if (string.IsNullOrWhiteSpace(NewSiteName) ||
                string.IsNullOrWhiteSpace(NewSiteUrl))
            {
                var dialogError = new ContentDialog
                {
                    Title = "Внимание",
                    Content = "Убедитесь, что все поля заполнены",
                    CloseButtonText = "Закрыть"
                };

                await dialogError.ShowAsync();
                return;
            }

            if (!(NewSiteUrl.StartsWith("http://") ||
                  NewSiteUrl.StartsWith("https://")))
                NewSiteUrl = "https://" + NewSiteUrl;
            await _dataTransferService.AddNewSiteOnStartPage(new SiteItem
            {
                Name = NewSiteName,
                Url = NewSiteUrl,
            });
            IsFlyoutClosed = true;
            GetStartPageElementsAsync();
        }

        public StartPageViewModel(IDataTransferService dataTransferService,
            ShellPageViewModel mainViewModel,
            ILocalSettingsService localSettingsService)
        {
            _dataTransferService = dataTransferService;
            _mainPageViewModel = mainViewModel;
            _localSettingsService = localSettingsService;
            InitializePageComponents();
        }

        private async void InitializePageComponents()
        {
            IsSuggestionBarEnabled = await _localSettingsService.ReadSettingAsync<bool>(nameof(IsSuggestionBarEnabled));
            IsAnimationEnabled = await _localSettingsService.ReadSettingAsync<bool>(nameof(IsAnimationEnabled));
            GridViewOrientation = await _localSettingsService.ReadSettingAsync<int>("StartPageGridViewOrientation");

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
            var termListTransfer = searchTermListTransfer.ToList();
            termListTransfer.Reverse();
            RecentlySearchedItems = new HashSet<SiteItem>(termListTransfer);
        }

        private void OnSearchButtonTappedCommandExecuted()
        {
            _mainPageViewModel.SearchWebFromStartPage(SearchBoxText);
        }

        private async void GetStartPageElementsAsync()
        {
            StartPageItems = new ObservableCollection<SiteItem>(await _dataTransferService.GetStartPageElements());
        }
    }
}