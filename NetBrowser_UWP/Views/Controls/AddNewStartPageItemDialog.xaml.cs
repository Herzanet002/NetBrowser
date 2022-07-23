using NetBrowser_UWP.ViewModels;
using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Диалоговое окно содержимого" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.Controls
{
    public sealed partial class AddNewStartPageItemDialog : ContentDialog
    {
        public AddNewStartPageItemDialog()
        {
            this.InitializeComponent();
            DataContext = App.GetService<StartPageViewModel>();
        }

    }
}
