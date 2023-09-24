using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser_UWP.Views.UserControls;
using NetBrowser.Core.Models;
using NetBrowser.Storage;

namespace NetBrowser_UWP.ViewModels.Settings;

public class HistoryPageViewModel : BindableBase
{
    private static IEnumerable<HistoryItem> _historyList;
    private static HistoryItem _selectedItem;
    private static string _searchText;

    private readonly IDataService _dataService;
    private readonly ITabViewService _tabViewService;

    public HistoryPageViewModel(IDataService dataService, ITabViewService tabViewService)
    {
        _dataService = dataService;
        _tabViewService = tabViewService;
    }

    public IAsyncRelayCommand DeleteCommand =>
        new AsyncRelayCommand<HistoryItem>(OnDeleteHistoryItemCommandExecuted);

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

    public IEnumerable<HistoryItem> HistoryList
    {
        get => _historyList;
        set => SetProperty(ref _historyList, value);
    }

    public HistoryItem SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    private async Task OnDeleteHistoryItemCommandExecuted(HistoryItem historyItem)
    {
        await _dataService.RemoveHistoryItemAsync(historyItem);
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
        await _dataService.ClearAllHistoryAsync();
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
        var list = await _dataService.GetHistoryAsync();
        if (list == null) return;
        var historyItemDetailsEnumerable = list.ToList();
        historyItemDetailsEnumerable.Reverse();
        HistoryList = historyItemDetailsEnumerable;
    }
}