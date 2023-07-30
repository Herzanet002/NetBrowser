using Windows.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NetBrowser_UWP.ViewModels.Settings;

public class MainSettingsPageViewModel : ObservableObject
{
    private string _fullAppInstalledPath;

    public MainSettingsPageViewModel()
    {
        FullAppInstalledPath = ApplicationData.Current.LocalFolder.Path;
    }

    public string FullAppInstalledPath
    {
        get => _fullAppInstalledPath;
        set => SetProperty(ref _fullAppInstalledPath, value);
    }
}