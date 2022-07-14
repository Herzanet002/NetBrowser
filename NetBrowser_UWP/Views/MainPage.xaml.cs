using Windows.UI.Xaml.Controls;
using NetBrowser_UWP.ViewModels;

namespace NetBrowser_UWP.Views
{
    /// <summary>
    ///     Главная страница браузера, в котором отображается весь контент
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {

            InitializeComponent();
            DataContext = App.GetService<MainPageViewModel>();
        }

    }

}