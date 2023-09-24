using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser.Core.Attributes;
using NetBrowser_UWP.ViewModels.Settings;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.Settings;

[PageAddress("app://settings/about")]
[ParentPageType(typeof(SettingsPage))]
public sealed partial class AboutAppPage : Page
{
    public AboutAppPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<AboutAppViewModel>();
    }
}