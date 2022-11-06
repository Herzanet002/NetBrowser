using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts.Services
{
    public interface IWebView2Service
    {
        Task<WebView2> InstantiateWebView2(string uriToNavigate);
        string GetCurrentBrowserVersion();
        Uri ResolveUri(string address);
        event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;
        event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
        event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStarting;

    }
}