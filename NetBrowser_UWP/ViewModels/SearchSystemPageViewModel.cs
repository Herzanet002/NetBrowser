using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetBrowser_UWP.ViewModels
{
    internal class SearchSystemPageViewModel : ObservableObject
    {
        private static ObservableCollection<SearchEngineItem> _listOfEngines;
        private SearchEngineItem _currentEngine;
        private readonly IDataTransferService _dataTransferService;
        public ObservableCollection<SearchEngineItem> ListOfEngines
        {
            get => _listOfEngines;
            set => SetProperty(ref _listOfEngines, value);
        }

        public SearchEngineItem CurrentEngine
        {
            get => _currentEngine;
            set
            {
                SetProperty(ref _currentEngine, value);
                ChangeSearchEngine();
            }
        }

        private void ChangeSearchEngine()
        {
            _dataTransferService.ChangeSearchEngine(CurrentEngine.Name);
            App.CurrentWebEngine = CurrentEngine;
        }
        public SearchSystemPageViewModel(IDataTransferService dataTransferService)
        {
            _dataTransferService = dataTransferService;
            GetEngines();
        }

        public async void GetEngines()
        {
            ListOfEngines = new ObservableCollection<SearchEngineItem>(await _dataTransferService.GetSearchEngineList());

            var selectedEngine = from item in ListOfEngines
                                 where item.IsSelected == "1"
                                 select item;
            CurrentEngine = selectedEngine.FirstOrDefault();
        }
    }
}
