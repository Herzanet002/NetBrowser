using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using NetBrowser_UWP.Contracts.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NetBrowser_UWP.Services
{
    public class WebView2Service : IWebView2Service
    {
        public ObservableCollection<WebView2> ContainerStates { get; set; }

        public WebView2Service()
        {
            ContainerStates = new ObservableCollection<WebView2>();
        }

        public event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;
        public event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
        public event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStarting;

        public async Task<WebView2> InstantiateWebView2(string address)
        {
            var instance = new WebView2();
            await instance.EnsureCoreWebView2Async();
            instance.Tag = true;
            ContainerStates.Add(instance);
            instance.NavigationCompleted += OnNavigationCompleted;
            instance.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
            instance.NavigationStarting += OnNavigationStarting;
            instance.CoreWebView2.Navigate(address);
            return instance;
        }

        private void OnNavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            sender.Tag = true;
            NavigationStarting?.Invoke(sender, args);
        }


        private void OnNewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            NewWindowRequested?.Invoke(sender, args);
            args.Handled = true;
        }


        private void OnNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            sender.Tag = false;
            NavigationCompleted?.Invoke(sender, args);
        }

    }


}
