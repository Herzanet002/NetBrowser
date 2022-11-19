using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels;
using NetBrowser_UWP.ViewModels.Settings;

// Документацию по шаблону элемента "Диалоговое окно содержимого" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.UserControls;

public sealed partial class DeleteBookmarkDialog : ContentDialog
{
    public DeleteBookmarkDialog()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<BookmarksPageViewModel>();
    }
}