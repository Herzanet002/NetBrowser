using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser.Core.Attributes;
using NetBrowser_UWP.ViewModels.News;

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