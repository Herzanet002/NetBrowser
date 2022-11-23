using NetBrowser_UWP.Views.News;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace NetBrowser_UWP.Services
{
    public class FirstRunRecommendationsService
    {
        private static bool shown = false;
        public async Task ShowIfAppropriateAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    if (shown) return;
                    shown = true;
                    var dialog = new FirstRunRecommendationsDialog();
                    await dialog.ShowAsync();
                });
        }
    }
}
