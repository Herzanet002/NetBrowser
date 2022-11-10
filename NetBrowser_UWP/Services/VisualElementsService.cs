using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using NetBrowser_UWP.Contracts.Services;
using Windows.UI.Xaml;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Services
{
    public class VisualElementsService : ObservableObject
    {
        private readonly ILocalSettingsService _localSettingsService;

        #region Properties

        private string _appTitleText;
        private string _searchBoxText;
        private bool _isProgressRingActive;
        private bool _isFlyoutClosed;
        private bool _isBookmarksExists;
        private bool _visibilityHomeButton;
        private Visibility _visibilityDeleteBookmarkButton;

        public string SearchBoxText
        {
            get => _searchBoxText;
            set => SetProperty(ref _searchBoxText, value);
        }

        public string AppTitleText
        {
            get => _appTitleText;
            set => SetProperty(ref _appTitleText, value);
        }

        public bool IsProgressRingActive
        {
            get => _isProgressRingActive;
            set => SetProperty(ref _isProgressRingActive, value);
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

        public bool IsBookmarksExists
        {
            get => _isBookmarksExists;
            set => SetProperty(ref _isBookmarksExists, value);
        }

        public Visibility DeleteBookmarkButtonVisibility
        {
            get => _visibilityDeleteBookmarkButton;
            set => SetProperty(ref _visibilityDeleteBookmarkButton, value);
        }

        public bool VisibilityHomeButton
        {
            get => _visibilityHomeButton;
            set
            {
                SetProperty(ref _visibilityHomeButton, value);
                _localSettingsService.SaveSettingAsync("IsHomeButtonEnabled", value);
            }
        }

        #endregion Properties

        public VisualElementsService(ILocalSettingsService localSettingsService)
        {
            _localSettingsService = localSettingsService;
            InitializePageComponents();
        }

        private async void InitializePageComponents()
        {
            VisibilityHomeButton = await _localSettingsService.ReadSettingAsync<bool>("IsHomeButtonEnabled");
        }

        public void SetFlyoutClosedState(bool isClosed) =>
            IsFlyoutClosed = isClosed;

        public void SetProgressRingActivity(bool isActive) => IsProgressRingActive = isActive;

        //public void SetVisualUiElementStates(object sender)
        //{
        //    if (sender is not WebView2 webInstance)
        //    {
        //        SetProgressRingActivity(false);
        //    }
        //    else
        //    {
        //        SetProgressRingActivity((bool)webInstance.Tag);
        //    }
        //    SetBookmarkButtonAppearance(sender);
        //}

        public void SetVisualUiLabels(string appTitleText, string searchBoxText)
        {
            AppTitleText = appTitleText;
            SearchBoxText = searchBoxText;
        }

        public void SetBookmarkIconState(bool isAccessable)
        {
            IsBookmarksExists = isAccessable;
            DeleteBookmarkButtonVisibility = isAccessable ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetBookmarkButtonAppearance(WebView2 content, IList<BookmarkDetails> bookmarksList)
        {
            if (content == null)
            {
                SetBookmarkIconState(false);
                return;
            }
      
            if (bookmarksList == null) return;

            var existableBookmark = bookmarksList.FirstOrDefault(bookmark => bookmark.Url == content.Source.AbsoluteUri);

            SetBookmarkIconState(existableBookmark != null);
        }
    }
}