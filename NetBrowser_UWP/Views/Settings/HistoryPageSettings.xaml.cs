using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser.Core.Attributes;
using NetBrowser_UWP.ViewModels.Settings;

namespace NetBrowser_UWP.Views.Settings;

[PageAddress("app://settings/history")]
[ParentPageType(typeof(SettingsPage))]
public sealed partial class HistoryPageSettings : Page
{
    public HistoryPageSettings()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<HistoryPageViewModel>();
    }

    private void ListViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (e.Pointer.PointerDeviceType is PointerDeviceType.Mouse or PointerDeviceType.Pen)
            VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
    }

    private void ListViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);
    }
}