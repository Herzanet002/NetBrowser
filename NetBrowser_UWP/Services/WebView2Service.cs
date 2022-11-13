using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.Services;

public class WebView2Service : IWebView2Service
{
    public WebView2Service()
    {
        ContainerStates = new ObservableCollection<WebView2>();
    }

    public ObservableCollection<WebView2> ContainerStates { get; set; }

    public string GetCurrentBrowserVersion()
    {
        return CoreWebView2Environment.GetAvailableBrowserVersionString();
    }


    public event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;
    public event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
    public event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStarting;


    public async Task<WebView2> InstantiateWebView2(string uriToNavigate)
    {
        var instance = new WebView2();
        await instance.EnsureCoreWebView2Async();
        instance.Tag = true;
        ContainerStates.Add(instance);
        instance.NavigationCompleted += OnNavigationCompleted;
        instance.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
        instance.NavigationStarting += OnNavigationStarting;
        instance.CoreWebView2.Navigate(string.IsNullOrWhiteSpace(uriToNavigate)
            ? App.CurrentWebEngine.HomePage
            : ResolveUri(uriToNavigate).ToString());
        return instance;
    }

    public Uri ResolveUri(string address)
    {
        address = address.Trim().ToLower();
        const string PATTERN = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
        var rgx = new Regex(PATTERN, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var httpsScheme = string.Concat(Uri.UriSchemeHttps, "://");
        var httpScheme = string.Concat(Uri.UriSchemeHttp, "://");

        if (rgx.IsMatch(address))
        {
            if (!(address.StartsWith(httpScheme) || address.StartsWith(httpsScheme)))
                address = string.Concat(httpsScheme, address);
        }
        else
        {
            return new Uri(App.CurrentWebEngine.Prefix + address);
        }

        var isUriCreated = Uri.TryCreate(address, UriKind.Absolute, out var uriAddress) &&
                           (uriAddress.Scheme == Uri.UriSchemeHttp ||
                            uriAddress.Scheme == Uri.UriSchemeHttps ||
                            uriAddress.Scheme == Uri.UriSchemeFtp);

        return isUriCreated ? uriAddress : new Uri(App.CurrentWebEngine.Prefix + address);
    }

    private void OnNavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
    {
        sender.Tag = true;
        NavigationStarting?.Invoke(sender, args);
    }


    private void OnNewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
    {
        args.Handled = true;
        NewWindowRequested?.Invoke(sender, args);
    }


    private void OnNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        sender.Tag = false;
        NavigationCompleted?.Invoke(sender, args);
    }
}