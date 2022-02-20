using NetBrowser_UWP.Commands;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        public BookmarksPageViewModel()
        {
            GetBookmarksAsync();
            EditBookmarkCommand = new Command(ShowDialog, CanExecuteMethod);
            SaveEditedBookmarkCommand = new Command(SaveChangedBookmark, CanExecuteMethod);
            OpenBookmarkCommand = new Command(OpenBookmarkInWeb, CanExecuteMethod);
            DeleteBookmarkCommand = new Command(DeleteSelectedBookmark, CanExecuteMethod);
            RemoveBookmarkCommand = new Command(RemoveBookmark, CanExecuteMethod);
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
            BookmarksList = await DataTransfer.GetBookmarkList();
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
                MainPage.CreateNewWebTab();
                MainPage.SearchWeb(new Uri(SelectedBookmark.Url));
            }
           
        }

        private async void RemoveBookmark(object parameter)
        {
            await DataTransfer.RemoveBookmark(SelectedBookmark.Url);
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
            BookmarkNewTitle = SelectedBookmark?.Title;
            BookmarkNewUrl = SelectedBookmark?.Url;
            ContentDialog dialog = new EditBookmarkDialog();
            await dialog.ShowAsync();
            GetBookmarksAsync();
        }

        private void SaveChangedBookmark(object parameter) => DataTransfer.EditBookmark(_oldUrl, BookmarkNewUrl, BookmarkNewTitle);

        private static bool CanExecuteMethod(object parameter)
        {
            return true;
        }



    }
}