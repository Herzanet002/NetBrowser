using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.News;

/// <summary>
///     Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
/// </summary>
public sealed partial class NewsPage : Page
{
    public NewsPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<NewsPageViewModel>();
        DataContext = ViewModel;
        ViewModel.Initialize(MainFrame, NewsNavigationView);
    }

    public NewsPageViewModel ViewModel { get; set; }
}