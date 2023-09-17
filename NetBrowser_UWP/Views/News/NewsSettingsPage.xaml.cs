using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.News;
using NetBrowser_UWP.Attributes;

namespace NetBrowser_UWP.Views.News;

[PageAddress("app://news/settings")]
[ParentPageType(typeof(NewsShellPage))]
public sealed partial class NewsSettingsPage : Page
{
    public NewsSettingsPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<NewsSettingsViewModel>();
    }
}