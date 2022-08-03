using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Views.Controls;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NetBrowser_UWP.ViewModels
{
    internal class BookmarksPageViewModel : ObservableObject
    {
        private static List<BookmarkDetails> _bookmarksList;
        private static string _bookmarkNewTitle;
        private static string _bookmarkNewUrl;
        private static BookmarkDetails _selectedBookmark;
        private static string _oldUrl;


        #region Commands
        public ICommand EditBookmarkCommand => new DelegateCommand(OnEditBookmarkCommandExecuted, () => true);
        public ICommand SaveEditedBookmarkCommand => new DelegateCommand(OnSaveEditedBookmarkCommandExecuted, () => true);
        public ICommand OpenBookmarkCommand => new DelegateCommand(OnOpenBookmarkInWebCommandExecuted, () => true);
        public ICommand DeleteBookmarkCommand => new DelegateCommand(OnDeleteSelectedBookmarkCommandExecuted, () => true);
        public ICommand RemoveBookmarkCommand => new DelegateCommand(OnRemoveBookmarkCommandExecuted, () => true);
        #endregion

        private readonly IDataTransferService _dataTransferService;
        private readonly MainPageViewModel _mainPageViewModel;

        public BookmarksPageViewModel(IDataTransferService dataTransferService, MainPageViewModel mainPageViewModel)
        {
            _dataTransferService = dataTransferService;
            _mainPageViewModel = mainPageViewModel;
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
            BookmarksList = await _dataTransferService.GetBookmarkList();
        }
        public List<BookmarkDetails> BookmarksList
        {
            get => _bookmarksList;
            set => SetProperty(ref _bookmarksList, value);
        }

        private void OnOpenBookmarkInWebCommandExecuted()
        {
            _mainPageViewModel.CreateNewWebTab(SelectedBookmark.Url);
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
            _oldUrl = SelectedBookmark.Url;
            BookmarkNewTitle = SelectedBookmark.Name;
            BookmarkNewUrl = SelectedBookmark.Url;
            await new EditBookmarkDialog().ShowAsync();
            GetBookmarksAsync();
        }

        private void OnSaveEditedBookmarkCommandExecuted() => _dataTransferService.EditBookmark(_oldUrl, BookmarkNewUrl, BookmarkNewTitle);





    }
}