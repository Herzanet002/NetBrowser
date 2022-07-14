using NetBrowser_UWP.Commands;
using NetBrowser_UWP.Models;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.Views;
using NetBrowser_UWP.Views.Controls;

namespace NetBrowser_UWP.ViewModels
{
    internal class BookmarksPageViewModel : Base.ViewModel
    {
        private static List<BookmarkDetails> _bookmarksList;
        private static string _bookmarkNewTitle;
        private static string _bookmarkNewUrl;
        private static BookmarkDetails _selectedBookmark;
        private static string _oldUrl;

        private static MainPage MainPage => (Window.Current.Content as Frame)?.Content as MainPage;
        
        #region Commands
        public ICommand EditBookmarkCommand { get; set; }
        public ICommand SaveEditedBookmarkCommand { get; set; }
        public ICommand OpenBookmarkCommand { get; set; }
        public ICommand DeleteBookmarkCommand { get; set; }
        public ICommand RemoveBookmarkCommand { get; set; }
        #endregion

        private readonly IDataTransferService _dataTransferService;
        public BookmarksPageViewModel(IDataTransferService dataTransferService)
        {
            _dataTransferService = dataTransferService;
            GetBookmarksAsync();
            EditBookmarkCommand = new Command(ShowDialog, _ => true);
            SaveEditedBookmarkCommand = new Command(SaveChangedBookmarkCommand_Executed, _ => true);
            OpenBookmarkCommand = new Command(OpenBookmarkInWeb, _ => true);
            DeleteBookmarkCommand = new Command(DeleteSelectedBookmark, _ => true);
            RemoveBookmarkCommand = new Command(RemoveBookmark, _ => true);
        }

        public string BookmarkNewTitle
        {
            get => _bookmarkNewTitle;
            set => Set(ref _bookmarkNewTitle, value);
        }
        public string BookmarkNewUrl
        {
            get => _bookmarkNewUrl;
            set => Set(ref _bookmarkNewUrl, value);
        }

        public BookmarkDetails SelectedBookmark
        {
            get => _selectedBookmark;
            set => Set(ref _selectedBookmark, value);
        }

        public async void GetBookmarksAsync()
        {
            BookmarksList = await _dataTransferService.GetBookmarkList();
        }
        public List<BookmarkDetails> BookmarksList
        {
            get => _bookmarksList;
            set => Set(ref _bookmarksList, value);
        }
        private void OpenBookmarkInWeb(object parameter)
        {
            if (SelectedBookmark != null)
            {
                // TODO: MainPage.CreateNewWebTab(SelectedBookmark.Url);
            }
        }

        private async void RemoveBookmark(object parameter)
        {
            await _dataTransferService.RemoveBookmark(SelectedBookmark.Url);
        }
        private async void DeleteSelectedBookmark(object parameter)
        {
            ContentDialog deleteDialog = new DeleteBookmarkDialog();
            await deleteDialog.ShowAsync();
            GetBookmarksAsync();
        }
        private async void ShowDialog(object parameter)
        {
            _oldUrl = SelectedBookmark?.Url;
            BookmarkNewTitle = SelectedBookmark?.Name;
            BookmarkNewUrl = SelectedBookmark?.Url;
            ContentDialog dialog = new EditBookmarkDialog();
            await dialog.ShowAsync();
            GetBookmarksAsync();
        }

        private void SaveChangedBookmarkCommand_Executed(object parameter) => _dataTransferService.EditBookmark(_oldUrl, BookmarkNewUrl, BookmarkNewTitle);

        



    }
}