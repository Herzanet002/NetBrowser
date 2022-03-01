using NetBrowser_UWP.Commands;
using NetBrowser_UWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using NetBrowser_UWP.Views.Controls;


namespace NetBrowser_UWP.ViewModels
{
    internal class HistoryPageViewModel : Base.ViewModel
    {
        private static IEnumerable<HistoryItemDetails> _historyList;
        private static HistoryItemDetails _selectedItem;
        private static string _searchText;
        private static MainPage MainPage => (Window.Current.Content as Frame)?.Content as MainPage;

        public ICommand DeleteCommand => new Command(DeleteHistoryCommand_Executed, _ => true);
        public ICommand OpenPageCommand => new Command(OpenCommand_Executed, _ => true);
        public ICommand ClearHistoryCommand => new Command(ClearHistoryCommand_Executed, _ => true);
        public ICommand OpenClearDialogCommand => new Command(OpenClearDialogCommand_Executed, _ => true);

        private async void DeleteHistoryCommand_Executed(object param)
        {
            if (param is null) return;
            var toBeDeleted = HistoryList.FirstOrDefault(c => c.Time == param.ToString());
            var wasDeleted = await DataTransfer.RemoveHistoryItem(toBeDeleted?.Time);
            if (!wasDeleted) return;
            var history = HistoryList.ToList();
            history.Remove(toBeDeleted);
            HistoryList = history;
        }
        
        private async void ClearHistoryCommand_Executed(object param)
        {
            await DataTransfer.ClearHistoryFile();
        }

        private async void OpenClearDialogCommand_Executed(object param)
        {
            ContentDialog dialog = new HistoryClearConfirmationDialog();
            await dialog.ShowAsync();
            GetHistoryAsync();
        }
        private void OpenCommand_Executed(object param)
        {
            if (SelectedItem == null) return;
            MainPage.CreateNewWebTab();
            MainPage.SearchWeb(SelectedItem.Url);
            SelectedItem = null;
        }

        public HistoryPageViewModel()
        {
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
                where item.Title.ToLower().Contains(SearchText.ToLower()) || item.Url.ToLower().Contains(SearchText.ToLower()) select item;
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
            var list = await DataTransfer.GetHistory("url");
            list.Reverse();
            HistoryList = list;
        }

    }

    public class DateTimeToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string[] splitstring = value.ToString().Split(":");
            if (splitstring[0].Length == 1) splitstring[0] = "0" + splitstring[0];
            return $"{splitstring[0]}:{splitstring[1]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }

}
