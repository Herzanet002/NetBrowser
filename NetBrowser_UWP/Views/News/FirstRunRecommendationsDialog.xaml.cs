using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;
using NetBrowser_UWP.ViewModels.News;

// Документацию по шаблону элемента "Диалоговое окно содержимого" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.News
{
    public sealed partial class FirstRunRecommendationsDialog : ContentDialog
    {
        public FirstRunRecommendationsDialog()
        {
            this.InitializeComponent();
            DataContext = Ioc.Default.GetRequiredService<FirstRunRecommendationsViewModel>();
        }
    }
}
