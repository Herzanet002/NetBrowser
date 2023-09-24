using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser.Core.Attributes;
using NetBrowser_UWP.ViewModels.Settings;

namespace NetBrowser_UWP.Views.Settings;

[PageAddress("app://settings/bookmarks")]
[ParentPageType(typeof(SettingsPage))]
public sealed partial class BookmarksPageSettings : Page
{
    public BookmarksPageSettings()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<BookmarksPageViewModel>();
    }

    private void BookmarksListView_Tapped(object sender, TappedRoutedEventArgs e)
    {
        var flyout = FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);
        var options = new FlyoutShowOptions
        {
            Position = e.GetPosition((FrameworkElement)sender),
            ShowMode = FlyoutShowMode.Transient
        };
        flyout?.ShowAt((FrameworkElement)sender, options);
    }
}