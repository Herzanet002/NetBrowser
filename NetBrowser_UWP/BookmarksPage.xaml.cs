using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using NetBrowser_UWP.Annotations;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public sealed partial class BookmarksPage : Page, INotifyPropertyChanged
    {
        private static string _oldUrl;
        private List<BookmarkDetails> _bookmarksList;

        public List<BookmarkDetails> BookmarksList
        {

            get => _bookmarksList;
            set
            {
                _bookmarksList = value;
                OnPropertyChanged(nameof(BookmarksList));
            }
        }
        public BookmarksPage()
        {
            this.InitializeComponent();
            this.DataContext = this;

        }

        private void bookmarksPage_Loaded(object sender, RoutedEventArgs e)
        {
            GetBookmarks();
        }

        public async void GetBookmarks()
        {

            BookmarksList = await DataTransfer.GetBookmarkList();

            bookmarksListView.ItemsSource = BookmarksList;
        }

        private void bookmarksListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            bookmarkMenuFlyout.ShowAt(bookmarksListView, e.GetPosition(bookmarksListView));
            // ReSharper disable once InvertIf
            if (bookmarksListView.SelectedItem is BookmarkDetails selectedBookmark)
            {
                _oldUrl = selectedBookmark.Url;
            }
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            MainPage.CreateNewWebTab();
            if (bookmarksListView.SelectedItem is BookmarkDetails selectedBookmark) MainPage.SearchWeb(new Uri(selectedBookmark.Url));
        }
        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog editDialog = editingBookmarkDialog;
            if (bookmarksListView.SelectedItem is BookmarkDetails selectedBookmark)
            {
                bookmarkNewTitle.Text = selectedBookmark.Title;
                bookmarkNewUrl.Text = selectedBookmark.Url;
            }

            editDialog.DefaultButton = ContentDialogButton.Primary;
            editDialog.PrimaryButtonClick += EditDialog_PrimaryButtonClick;

            await editDialog.ShowAsync();
            GetBookmarks();


        }

        private static MainPage MainPage => (Window.Current.Content as Frame)?.Content as MainPage;

        private void EditDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var newTitle = bookmarkNewTitle.Text;
            var newUrl = bookmarkNewUrl.Text;

            DataTransfer.EditBookmark(_oldUrl, newUrl, newTitle);
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Content = "Вы действительно хотите удалить закладку?",
                Title = "Удаление",
                PrimaryButtonText = "Удалить",
                SecondaryButtonText = "Отмена"
            };
            deleteDialog.PrimaryButtonClick += DeleteDialog_PrimaryButtonClick;
            await deleteDialog.ShowAsync();

            GetBookmarks();
        }

        private async void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (bookmarksListView.SelectedItem is BookmarkDetails bookmarkDetails) await DataTransfer.RemoveBookmark(bookmarkDetails.Url);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
