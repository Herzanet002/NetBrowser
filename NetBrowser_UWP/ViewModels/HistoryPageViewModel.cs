using NetBrowser_UWP.Commands;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;


namespace NetBrowser_UWP.ViewModels
{
    internal class HistoryPageViewModel : Base.ViewModel
    {
        private static IEnumerable<HistoryItemDetails> _historyList;
        private static HistoryItemDetails _selectedItem;
        private static string _searchText;

        private readonly IDataTransferService _dataTransferService;
        public ICommand DeleteCommand => new Command(DeleteHistoryCommand_Executed, _ => true);
        public ICommand OpenPageCommand => new Command(OpenCommand_Executed, _ => true);
        public ICommand ClearHistoryCommand => new Command(ClearHistoryCommand_Executed, _ => true);
        public ICommand OpenClearDialogCommand => new Command(OpenClearDialogCommand_Executed, _ => true);

        private async void DeleteHistoryCommand_Executed(object param)
        {
            if (param is null) return;
            var toBeDeleted = HistoryList.FirstOrDefault(c => c.Time == param.ToString());
            var wasDeleted = await _dataTransferService.RemoveHistoryItem(toBeDeleted?.Time);
            if (!wasDeleted) return;
            var history = HistoryList.ToList();
            history.Remove(toBeDeleted);
            HistoryList = history;
        }

        private async void ClearHistoryCommand_Executed(object param)
        {
            await _dataTransferService.ClearHistoryFile();
        }

        private async void OpenClearDialogCommand_Executed(object param)
        {
            await new HistoryClearConfirmationDialog().ShowAsync();
            GetHistoryAsync();
        }
        private async void OpenCommand_Executed(object param)
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
                Set(ref _searchText, value);
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
            set => Set(ref _historyList, value);
        }
        public HistoryItemDetails SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public async void GetHistoryAsync()
        {
            var list = await _dataTransferService.GetHistory();
            list.Reverse();
            HistoryList = list;
        }

    }



}
