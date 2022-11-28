using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels;

namespace NetBrowser_UWP.Views;

/// <summary>
///     Главная страница браузера, в котором отображается весь контент
/// </summary>
public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<ShellPageViewModel>();
        SearchBox.Translation += new Vector3(0, 0, 32);
    }
}