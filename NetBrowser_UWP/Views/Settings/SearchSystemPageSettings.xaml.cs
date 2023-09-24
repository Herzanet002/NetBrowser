using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser.Core.Attributes;
using NetBrowser_UWP.ViewModels.Settings;

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