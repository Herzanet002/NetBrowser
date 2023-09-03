using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels;

namespace NetBrowser_UWP.Views;

/// <summary>
///     Главная страница браузера, в котором отображается весь контент
/// </summary>
public sealed partial class ShellPage : Page
{
    public ShellPageViewModel ViewModel { get; }

    public ShellPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetService<ShellPageViewModel>();
        DataContext = ViewModel;
    }
}