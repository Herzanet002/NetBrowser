using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.Attributes;
using NetBrowser_UWP.ViewModels.News;

namespace NetBrowser_UWP.Views.News;

[PageAddress("app://news/all")]
[ParentPageType(typeof(NewsShellPage))]
public sealed partial class AllNewsPage : Page
{
    public AllNewsPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<AllNewsPageViewModel>();
    }
}