using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels;
using NetBrowser_UWP.Attributes;

namespace NetBrowser_UWP.Views;

[PageAddress("app://startpage")]
public sealed partial class StartPage : Page
{
    public StartPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<StartPageViewModel>();
    }

    private void GridView_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (e.Pointer.PointerDeviceType is PointerDeviceType.Mouse or PointerDeviceType.Pen)
            VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
    }

    private void GridView_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);
    }
}