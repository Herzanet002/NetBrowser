using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser.Storage;

namespace NetBrowser_UWP.ViewModels.News;

public class NewsSettingsViewModel : BindableBase
{
    private readonly IDataService _dataService;

    public NewsSettingsViewModel(IDataService dataService)
    {
        _dataService = dataService;
        ClearRecommendCategoriesCommand = new AsyncRelayCommand(OnClearRecommendCategoriesCommandExecuted);
    }

    public IAsyncRelayCommand ClearRecommendCategoriesCommand { get; set; }

    private async Task OnClearRecommendCategoriesCommandExecuted()
    {
        await _dataService.ClearLikedNewsProvidersAsync();
        await new ContentDialog
        {
            Title = "Успешно выполнено",
            Content = "Чтобы заново настроить список рекомендаций, перейдите на вкладку 'Рекоммендации'",
            CloseButtonText = "Закрыть"
        }.ShowAsync();
    }
}