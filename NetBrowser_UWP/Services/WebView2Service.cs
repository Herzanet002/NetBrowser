using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.Services
{
    public class WebView2Service : IWebView2Service
    {
        public ObservableCollection<WebView2> States { get; set; }

        public WebView2Service()
        {
            States = new ObservableCollection<WebView2>();
        }
        public event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;
        public event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
        public event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStarting;

        public async Task<WebView2> InstantiateWebView2(string address)
        {
            var instance = new WebView2();
            await instance.EnsureCoreWebView2Async();
            States.Add(instance);
            instance.NavigationCompleted += OnNavigationCompleted; 
            instance.CoreWebView2.NewWindowRequested += OnNewWindowRequested; 
            instance.NavigationStarting += OnNavigationStarting; 
            instance.CoreWebView2.Navigate(address);
            return instance;
        }

        private void OnNavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args) =>
            NavigationStarting?.Invoke(this, args);

        private void OnNewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            NewWindowRequested?.Invoke(this, args);
            args.Handled = true;
        }
            

        private void OnNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args) => 
            NavigationCompleted?.Invoke(this, args);
    }
}
