using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels;

namespace NetBrowser_UWP.Views.UserControls
{
    public sealed partial class EditBookmarkDialog
    {
        public EditBookmarkDialog()
        {
            this.InitializeComponent();
            DataContext = Ioc.Default.GetRequiredService<BookmarksPageViewModel>();
        }


    }
}
