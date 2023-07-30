using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.ViewModels.News;

public class NewsSettingsViewModel : ObservableObject
{
    private readonly IDataTransferService _dataTransferService;

    public NewsSettingsViewModel(IDataTransferService dataTransferService)
    {
        _dataTransferService = dataTransferService;
        ClearRecommendCategoriesCommand = new AsyncRelayCommand(OnClearRecommendCategoriesCommandExecuted);
    }

    public IAsyncRelayCommand ClearRecommendCategoriesCommand { get; set; }

    private async Task OnClearRecommendCategoriesCommandExecuted()
    {
        await _dataTransferService.ClearAllRecommendedCategories();
        await new ContentDialog
        {
            Title = "Успешно выполнено",
            Content = "Чтобы заново настроить список рекомендаций, перейдите на вкладку 'Рекоммендации'",
            CloseButtonText = "Закрыть"
        }.ShowAsync();
    }
}