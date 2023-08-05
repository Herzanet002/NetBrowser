using Windows.Storage;
using NetBrowser_UWP.ViewModels.Base;

namespace NetBrowser_UWP.ViewModels.Settings;

public class MainSettingsPageViewModel : BindableBase
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