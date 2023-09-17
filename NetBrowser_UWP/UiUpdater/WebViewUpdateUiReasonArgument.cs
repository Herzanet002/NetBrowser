using Microsoft.UI.Xaml.Controls;

namespace NetBrowser_UWP.UiUpdater;

public class WebViewUpdateUiReasonArgument : UpdateUiReasonArgument
{
    public WebView2 WebViewInstance { get; }

    public WebViewUpdateUiReasonArgument(WebView2 webViewInstance) : base(UpdateUIReason.WebView)
        => WebViewInstance = webViewInstance;
}