using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.Settings;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP.Views.Settings;

/// <summary>
///     Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
/// </summary>
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