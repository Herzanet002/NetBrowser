using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser.Core.Attributes;
using NetBrowser_UWP.ViewModels.Settings;

namespace NetBrowser_UWP.Views.Settings;

[PageAddress("app://settings/general")]
[ParentPageType(typeof(SettingsPage))]
public sealed partial class MainItemPageSettings : Page
{
    public MainItemPageSettings()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<MainSettingsPageViewModel>();
    }
}