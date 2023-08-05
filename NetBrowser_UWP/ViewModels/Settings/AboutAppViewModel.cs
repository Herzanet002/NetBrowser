using Windows.ApplicationModel;
using Microsoft.Toolkit.Uwp.Helpers;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.ViewModels.Base;

namespace NetBrowser_UWP.ViewModels.Settings;

public class AboutAppViewModel : BindableBase
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