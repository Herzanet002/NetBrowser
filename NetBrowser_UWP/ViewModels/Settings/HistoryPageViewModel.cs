using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.Views.UserControls;

namespace NetBrowser_UWP.ViewModels.Settings;

public class HistoryPageViewModel : ObservableObject
{
    private static IEnumerable<HistoryItemDetails> _historyList;
    private static HistoryItemDetails _selectedItem;
    private static string _searchText;

    private readonly IDataTransferService _dataTransferService;
    private readonly TabViewService _tabViewService;

    public HistoryPageViewModel(IDataTransferService dataTransferService, TabViewService tabViewService)
    {
        _dataTransferService = dataTransferService;
        _tabViewService = tabViewService;
    }

    public IAsyncRelayCommand DeleteCommand =>
        new AsyncRelayCommand<HistoryItemDetails>(OnDeleteHistoryItemCommandExecuted);

    public IAsyncRelayCommand HistoryPageLoadedCommand => new AsyncRelayCommand(OnHistoryPageLoadedCommandExecuted);

    public IAsyncRelayCommand OpenPageCommand => new AsyncRelayCommand(OnOpenHistoryItemCommandExecuted);
    public IAsyncRelayCommand ClearHistoryCommand => new AsyncRelayCommand(OnClearHistoryJournalCommandExecuted);

    public IAsyncRelayCommand OpenClearDialogCommand =>
        new AsyncRelayCommand(OnOpenClearHistoryJournalDialogCommandExecuted);

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            GetSearchSuggestions();
        }
    }

    public IEnumerable<HistoryItemDetails> HistoryList
    {
        get => _historyList;
        set => SetProperty(ref _historyList, value);
    }

    public HistoryItemDetails SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    private async Task OnDeleteHistoryItemCommandExecuted(HistoryItemDetails historyItem)
    {
        await _dataTransferService.RemoveHistoryItemAsync(historyItem);
        var history = HistoryList.ToList();
        history.Remove(historyItem);
        HistoryList = history;
    }

    private async Task OnHistoryPageLoadedCommandExecuted(CancellationToken ct)
    {
        await GetHistoryAsync();
    }

    private async Task OnClearHistoryJournalCommandExecuted()
    {
        await _dataTransferService.ClearAllHistoryAsync();
    }

    private async Task OnOpenClearHistoryJournalDialogCommandExecuted()
    {
        await new HistoryClearConfirmationDialog().ShowAsync();
        await GetHistoryAsync().ConfigureAwait(false);
    }

    private async Task OnOpenHistoryItemCommandExecuted()
    {
        if (SelectedItem == null) return;
        if (Uri.IsWellFormedUriString(SelectedItem.Url, UriKind.Absolute))
        {
            await _tabViewService.CreateNewWebTab(SelectedItem.Url);
            SelectedItem = null;
        }
        else
        {
            var dialogError = new ContentDialog
            {
                Title = "Неверная ссылка",
                Content = "Ссылка " + SelectedItem.Url + " недействительна или неверна",
                CloseButtonText = "Закрыть"
            };

            await dialogError.ShowAsync();
        }
    }

    public void GetSearchSuggestions()
    {
        var suitable = from item in HistoryList
            where item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                  item.Url.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            select item;
        HistoryList = suitable;
    }

    public async Task GetHistoryAsync()
    {
        var list = await _dataTransferService.GetHistoryAsync();
        if (list == null) return;
        var historyItemDetailsEnumerable = list.ToList();
        historyItemDetailsEnumerable.Reverse();
        HistoryList = historyItemDetailsEnumerable;
    }
}