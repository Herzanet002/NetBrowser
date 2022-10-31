using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.ViewModels
{
    public class AboutAppViewModel : ObservableObject
    {
		private string _webViewVersion;

		public string WebViewVersion
		{
			get => _webViewVersion;
            set => _webViewVersion = value;
        }

        public AboutAppViewModel(IWebView2Service webView2Service)
        {
            WebViewVersion = webView2Service.GetCurrentBrowserVersion();
				
        }

	}
}
