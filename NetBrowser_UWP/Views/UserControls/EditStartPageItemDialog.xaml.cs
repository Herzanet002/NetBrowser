using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.Settings;
using Windows.UI.Xaml.Controls;
using NetBrowser_UWP.ViewModels;

// Документацию по шаблону элемента "Диалоговое окно содержимого" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.UserControls
{
    public sealed partial class EditStartPageItemDialog : ContentDialog
    {
        public EditStartPageItemDialog()
        {
            this.InitializeComponent();
            DataContext = Ioc.Default.GetRequiredService<StartPageViewModel>();
        }

    }
}
