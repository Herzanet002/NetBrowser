using System;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.News;
using NetBrowser_UWP.Attributes;

namespace NetBrowser_UWP.Views.News;

[PageAddress("app://news")]
public sealed partial class NewsShellPage : Page
{
    public NewsShellPage(Type pageType = default)
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<NewsShellPageViewModel>();
        DataContext = ViewModel;
        ViewModel.NavigationViewService?.Initialize(MainFrame, NewsNavigationView, pageType ?? typeof(AllNewsPage));
    }

    public NewsShellPageViewModel ViewModel { get; set; }
}