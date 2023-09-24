using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser.Core.Attributes;
using NetBrowser_UWP.ViewModels.News;

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