using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.News;
using NetBrowser_UWP.Attributes;

namespace NetBrowser_UWP.Views.News;

[PageAddress("app://news/recommendations")]
[ParentPageType(typeof(NewsShellPage))]
public sealed partial class RecommendationsNewsPage : Page
{
    public RecommendationsNewsPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<RecommendationsNewsPageViewModel>();
    }
}