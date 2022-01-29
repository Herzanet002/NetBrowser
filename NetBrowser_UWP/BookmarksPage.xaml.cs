using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;
// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class BookmarksPage : Page
    {
        public string oldUrl;
        public string oldTitle;
        
        public BookmarksPage()
        {
            this.InitializeComponent();

        }

        private void bookmarksPage_Loaded(object sender, RoutedEventArgs e)
        {
            GetBookmarks();
        }

        public async void GetBookmarks()
        {
            DataTransfer dataTransfer = new DataTransfer();
            List<BookmarkDetails> bookmarks = await dataTransfer.GetBookmarkList();
            
            bookmarksListView.ItemsSource = bookmarks;
        }

        private void bookmarksListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            bookmarkMenuFlyout.ShowAt(bookmarksListView, e.GetPosition(bookmarksListView));
            BookmarkDetails selectedBookmark = bookmarksListView.SelectedItem as BookmarkDetails;
            oldUrl = selectedBookmark.Url;
            oldTitle = selectedBookmark.Title;
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog editDialog = editingBookmarkDialog;
            BookmarkDetails selectedBookmark = bookmarksListView.SelectedItem as BookmarkDetails;
            bookmarkNewTitle.Text = selectedBookmark.Title;
            bookmarkNewUrl.Text = selectedBookmark.Url;
            editDialog.DefaultButton = ContentDialogButton.Primary;
            editDialog.PrimaryButtonClick += EditDialog_PrimaryButtonClick;
            await editDialog.ShowAsync();
        }

        private void EditDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var newTitle = bookmarkNewTitle.Text;
            var newUrl = bookmarkNewUrl.Text;
            DataTransfer dataTransfer = new DataTransfer();
            dataTransfer.EditBookmark(oldUrl, newUrl, newTitle);
            GetBookmarks();
           

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

        }

        private void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            DataTransfer dataTransfer = new DataTransfer();
            BookmarkDetails bookmarkDetails = bookmarksListView.SelectedItem as BookmarkDetails;
            dataTransfer.RemoveBookmark(bookmarkDetails.Url);
            
            GetBookmarks();
        }


        
    }
}
