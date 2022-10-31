using NetBrowser_UWP.ViewModels;
using System.Numerics;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;

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
            DataContext = Ioc.Default.GetService<MainPageViewModel>();
            SearchBox.Translation += new Vector3(0, 0, 32);
        }

    }

}