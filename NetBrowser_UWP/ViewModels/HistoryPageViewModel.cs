using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Views.Controls;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;


namespace NetBrowser_UWP.ViewModels
{
    internal class HistoryPageViewModel : ObservableObject
    {
        private static IEnumerable<HistoryItemDetails> _historyList;
        private static HistoryItemDetails _selectedItem;
        private static string _searchText;

        private readonly IDataTransferService _dataTransferService;
        public ICommand DeleteCommand => new DelegateCommand<object>(OnDeleteHistoryItemCommandExecuted, _ => true);
        public ICommand OpenPageCommand => new DelegateCommand(OnOpenHistoryItemCommandExecuted, () => true);
        public ICommand ClearHistoryCommand => new DelegateCommand(OnClearHistoryJournalCommandExecuted, () => true);
        public ICommand OpenClearDialogCommand => new DelegateCommand(OnOpenClearHistoryJournalDialogCommandExecuted, () => true);

        private async void OnDeleteHistoryItemCommandExecuted(object param)
        {
            if (param is null) return;
            var toBeDeleted = HistoryList.FirstOrDefault(c => c.Time == param.ToString());
            var wasDeleted = await _dataTransferService.RemoveHistoryItem(toBeDeleted?.Time);
            if (!wasDeleted) return;
            var history = HistoryList.ToList();
            history.Remove(toBeDeleted);
            HistoryList = history;
        }

        private async void OnClearHistoryJournalCommandExecuted()
        {
            await _dataTransferService.ClearHistoryFile();
        }

        private async void OnOpenClearHistoryJournalDialogCommandExecuted()
        {
            await new HistoryClearConfirmationDialog().ShowAsync();
            GetHistoryAsync();
        }
        private async void OnOpenHistoryItemCommandExecuted()
        {
            if (SelectedItem == null) return;
            if (Uri.IsWellFormedUriString(SelectedItem.Url, UriKind.Absolute))
            {
                _mainPageViewModel.CreateNewWebTab(SelectedItem.Url);
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

        private readonly MainPageViewModel _mainPageViewModel;
        public HistoryPageViewModel(IDataTransferService dataTransferService, MainPageViewModel mainPageViewModel)
        {
            _dataTransferService = dataTransferService;
            _mainPageViewModel = mainPageViewModel;
            GetHistoryAsync();
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                GetSearchSuggestions();
            }
        }

        public void GetSearchSuggestions()
        {
            var suitable = from item in HistoryList
                           where item.Name.ToLower().Contains(SearchText.ToLower()) || item.Url.ToLower().Contains(SearchText.ToLower())
                           select item;
            HistoryList = suitable;

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

        public async void GetHistoryAsync()
        {
            var list = await _dataTransferService.GetHistory();
            list.Reverse();
            HistoryList = list;
        }

    }



}
