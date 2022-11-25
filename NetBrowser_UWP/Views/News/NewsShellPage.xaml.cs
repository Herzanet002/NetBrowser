using System;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.News;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.News;

/// <summary>
///     Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
/// </summary>
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