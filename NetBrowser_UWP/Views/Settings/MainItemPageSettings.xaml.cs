using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.Settings;
using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.Settings;

/// <summary>
///     Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
/// </summary>
public sealed partial class MainItemPageSettings : Page
{
    public MainItemPageSettings()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<MainSettingsPageViewModel>();
    }
}