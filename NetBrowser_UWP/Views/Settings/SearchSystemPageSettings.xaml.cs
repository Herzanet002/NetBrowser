using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.Settings;
using NetBrowser_UWP.Attributes;

namespace NetBrowser_UWP.Views.Settings;

[PageAddress("app://settings/search")]
[ParentPageType(typeof(SettingsPage))]
public sealed partial class SearchSystemPageSettings : Page
{
    public SearchSystemPageSettings()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<SearchSystemPageViewModel>();
    }
}