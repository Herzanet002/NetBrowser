using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Views.UserControls;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NetBrowser_UWP.Services;

namespace NetBrowser_UWP.ViewModels
{
    internal class BookmarksPageViewModel : ObservableObject
    {
        private static ObservableCollection<BookmarkDetails> _bookmarksList;
        private static string _bookmarkNewTitle;
        private static string _bookmarkNewUrl;
        private static BookmarkDetails _selectedBookmark;

        #region Commands

        public ICommand EditBookmarkCommand => new DelegateCommand(OnEditBookmarkCommandExecuted, () => true);
        public ICommand SaveEditedBookmarkCommand => new DelegateCommand(OnSaveEditedBookmarkCommandExecuted, () => true);
        public ICommand OpenBookmarkCommand => new DelegateCommand(OnOpenBookmarkInWebCommandExecuted, () => true);
        public ICommand DeleteBookmarkCommand => new DelegateCommand(OnDeleteSelectedBookmarkCommandExecuted, () => true);
        public ICommand RemoveBookmarkCommand => new DelegateCommand(OnRemoveBookmarkCommandExecuted, () => true);

        #endregion Commands

        private readonly IDataTransferService _dataTransferService;
        private readonly TabViewService _tabViewService;

        public BookmarksPageViewModel(IDataTransferService dataTransferService, TabViewService tabViewService)
        {
            _dataTransferService = dataTransferService;
            _tabViewService = tabViewService;
            GetBookmarksAsync();
        }

        public string BookmarkNewTitle
        {
            get => _bookmarkNewTitle;
            set => SetProperty(ref _bookmarkNewTitle, value);
        }

        public string BookmarkNewUrl
        {
            get => _bookmarkNewUrl;
            set => SetProperty(ref _bookmarkNewUrl, value);
        }

        public BookmarkDetails SelectedBookmark
        {
            get => _selectedBookmark;
            set => SetProperty(ref _selectedBookmark, value);
        }

        public async void GetBookmarksAsync()
        {
            BookmarksList = new ObservableCollection<BookmarkDetails>(await _dataTransferService.GetBookmarksList());
        }

        public ObservableCollection<BookmarkDetails> BookmarksList
        {
            get => _bookmarksList;
            set => SetProperty(ref _bookmarksList, value);
        }

        private void OnOpenBookmarkInWebCommandExecuted()
        {
            _tabViewService.CreateNewWebTab(SelectedBookmark.Url);
            SelectedBookmark = null;
        }

        private async void OnRemoveBookmarkCommandExecuted()
        {
            await _dataTransferService.RemoveBookmark(SelectedBookmark);
        }

        private async void OnDeleteSelectedBookmarkCommandExecuted()
        {
            await new DeleteBookmarkDialog().ShowAsync();
            GetBookmarksAsync();
        }

        private async void OnEditBookmarkCommandExecuted()
        {
            BookmarkNewTitle = SelectedBookmark.Name;
            BookmarkNewUrl = SelectedBookmark.Url;

            await new EditBookmarkDialog().ShowAsync();
            GetBookmarksAsync();
        }

        private void OnSaveEditedBookmarkCommandExecuted()
        {
            _dataTransferService.EditBookmark(SelectedBookmark, new BookmarkDetails
            {
                Name = BookmarkNewTitle,
                Url = BookmarkNewUrl,
                FaviconUrl = Constants.Constants.FAVICONS_SERVICE + BookmarkNewUrl
            });
        }
    }
}