using Windows.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.Helpers;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.ViewModels.Settings;

public class AboutAppViewModel : ObservableObject
{
    public AboutAppViewModel(IWebView2Service webView2Service)
    {
        WebViewVersion = webView2Service.GetCurrentBrowserVersion();
        var version = Package.Current.Id.Version;
        AppVersion = version.ToFormattedString();
    }

    public string WebViewVersion { get; set; }
    public string AppVersion { get; set; }
}