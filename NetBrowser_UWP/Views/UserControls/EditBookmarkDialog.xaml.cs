using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.Settings;

namespace NetBrowser_UWP.Views.UserControls;

public sealed partial class EditBookmarkDialog
{
    public EditBookmarkDialog()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<BookmarksPageViewModel>();
    }
}