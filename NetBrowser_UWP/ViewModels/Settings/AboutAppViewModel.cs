using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.ViewModels.Settings;

public class AboutAppViewModel : ObservableObject
{
    public AboutAppViewModel(IWebView2Service webView2Service)
    {
        WebViewVersion = webView2Service.GetCurrentBrowserVersion();
    }

    public string WebViewVersion { get; set; }
}