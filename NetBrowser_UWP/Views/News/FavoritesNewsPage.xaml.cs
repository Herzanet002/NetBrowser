using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser.Core.Attributes;
using NetBrowser_UWP.ViewModels.News;

namespace NetBrowser_UWP.Views.News;

[PageAddress("app://news/favorites")]
[ParentPageType(typeof(NewsShellPage))]
public sealed partial class FavoritesNewsPage : Page
{
    public FavoritesNewsPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<FavoriteNewsPageViewModel>();
    }
}