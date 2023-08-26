using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using NetBrowser_UWP.CommandProcessor;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Helpers;

namespace NetBrowser_UWP.Services;

public class WebView2Service : IWebView2Service
{
    private readonly ICommandProcessor _commandProcessor;
    public ObservableCollection<WebView2> ContainerStates { get; set; } = new();

    public WebView2Service(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public string GetCurrentBrowserVersion()
        => CoreWebView2Environment.GetAvailableBrowserVersionString();

    public event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;

    public event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;

    public event TypedEventHandler<CoreWebView2, object> ContainsFullScreenElementChanged;

    public event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStarting;

    public async Task<WebView2> InstantiateWebView2(string uriToNavigate)
    {
        var instance = new WebView2();
        await instance.EnsureCoreWebView2Async();
        instance.Tag = true;
        ContainerStates.Add(instance);
        //instance.CoreWebView2.MemoryUsageTargetLevel // show if in low energy mode
        instance.NavigationCompleted += OnNavigationCompleted;
        instance.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
        instance.NavigationStarting += OnNavigationStarting;
        instance.CoreWebView2.ContainsFullScreenElementChanged += OnContainsFullScreenElementChanged;
        instance.CoreWebView2.Navigate(string.IsNullOrWhiteSpace(uriToNavigate)
            ? App.CurrentWebEngine.HomePage
            : _commandProcessor.ResolveCommand(new Command(uriToNavigate)).ResolvedCommandResult);
        return instance;
    }

    private void OnContainsFullScreenElementChanged(CoreWebView2 sender, object args)
    {
        ContainsFullScreenElementChanged?.Invoke(sender, args);
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