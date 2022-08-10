using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts.Services
{
    public interface IWebView2Service
    {
        ObservableCollection<WebView2> ContainerStates { get; set; }

        Task<WebView2> InstantiateWebView2(string address);

        event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;
        event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
        event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStarting;

    }
}