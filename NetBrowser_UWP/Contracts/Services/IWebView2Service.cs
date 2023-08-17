using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace NetBrowser_UWP.Contracts.Services;

public interface IWebView2Service
{
    Task<WebView2> InstantiateWebView2(string uriToNavigate);

    string GetCurrentBrowserVersion();

    event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;

    event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;

    event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStarting;

    event TypedEventHandler<CoreWebView2, object> ContainsFullScreenElementChanged;
}