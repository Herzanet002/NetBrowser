using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using NetBrowser_UWP.Views.News;

namespace NetBrowser_UWP.Services;

public class FirstRunRecommendationsService
{
    private static bool shown;

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