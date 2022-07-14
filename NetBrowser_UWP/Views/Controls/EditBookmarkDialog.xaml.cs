using NetBrowser_UWP.ViewModels;

namespace NetBrowser_UWP.Views.Controls
{
    public sealed partial class EditBookmarkDialog
    {
        public EditBookmarkDialog()
        {
            this.InitializeComponent();
            DataContext = App.GetService<BookmarksPageViewModel>();
        }


    }
}
