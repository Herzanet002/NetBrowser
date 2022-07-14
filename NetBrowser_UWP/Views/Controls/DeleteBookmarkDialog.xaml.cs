using Windows.UI.Xaml.Controls;
using NetBrowser_UWP.ViewModels;

// Документацию по шаблону элемента "Диалоговое окно содержимого" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.Controls
{
    public sealed partial class DeleteBookmarkDialog : ContentDialog
    {
        public DeleteBookmarkDialog()
        {
            this.InitializeComponent();
            DataContext = App.GetService<BookmarksPageViewModel>();
        }
    }
}
