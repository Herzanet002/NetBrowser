using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.Settings;
using NetBrowser_UWP.Attributes;

namespace NetBrowser_UWP.Views.Settings;

[PageAddress("app://settings/personalize")]
[ParentPageType(typeof(SettingsPage))]
public sealed partial class PersonalizePageSettings : Page
{
    public PersonalizePageViewModel ViewModel { get; }

    public PersonalizePageSettings()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<PersonalizePageViewModel>();
        DataContext = ViewModel;
        ViewModel.ShowNotificationRequested += (_, _) => RestartAppNotification.Show();
    }
}