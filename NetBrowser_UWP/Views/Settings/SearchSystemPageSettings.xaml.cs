using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.Settings;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.Settings;

/// <summary>
///     Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public sealed partial class SearchSystemPageSettings : Page
{
    public SearchSystemPageSettings()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<SearchSystemPageViewModel>();
    }
}