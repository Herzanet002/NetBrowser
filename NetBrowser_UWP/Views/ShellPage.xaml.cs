using System.Numerics;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

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
        var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        coreTitleBar.ExtendViewIntoTitleBar = true;
        coreTitleBar.LayoutMetricsChanged += (sender, args) =>
        {
            LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);
        };
        coreTitleBar.IsVisibleChanged += (sender, args) =>
        {
            AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        };
        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
        titleBar.ButtonBackgroundColor = Colors.Transparent;
        Window.Current.SetTitleBar(AppTitleBar);
    }
}