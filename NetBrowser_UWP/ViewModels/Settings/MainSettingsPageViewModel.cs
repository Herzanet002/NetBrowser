using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace NetBrowser_UWP.ViewModels.Settings;

public class MainSettingsPageViewModel : ObservableObject
{
    public MainSettingsPageViewModel()
    {
        FullAppInstalledPath = ApplicationData.Current.LocalFolder.Path;
    }

    private string _fullAppInstalledPath;

    public string FullAppInstalledPath
    {
        get => _fullAppInstalledPath;
        set => SetProperty(ref _fullAppInstalledPath, value);
    }
}