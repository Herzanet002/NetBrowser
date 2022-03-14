using NetBrowser_UWP.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetBrowser_UWP.ViewModels
{
    internal class SearchSystemPageViewModel : Base.ViewModel
    {
        private static ObservableCollection<SearchEngineItem> _listOfEngines;
        private SearchEngineItem _currentEngine;

        public ObservableCollection<SearchEngineItem> ListOfEngines
        {
            get => _listOfEngines;
            set => Set(ref _listOfEngines, value);
        }

        public SearchEngineItem CurrentEngine
        {
            get => _currentEngine;
            set
            {
                Set(ref _currentEngine, value);
                ChangeSearchEngine();
            }
        }

        private void ChangeSearchEngine()
        {
            DataTransfer.ChangeSearchEngine(CurrentEngine.Name);
            App.CurrentWebEngine = CurrentEngine;
        }
        public SearchSystemPageViewModel()
        {
            GetEngines();
        }

        public async void GetEngines()
        {
            ListOfEngines = new ObservableCollection<SearchEngineItem>(await DataTransfer.GetSearchEngineList());

            var selectedEngine = from item in ListOfEngines
                                 where item.Mode == "1"
                                 select item;
            CurrentEngine = selectedEngine.FirstOrDefault();
        }
    }
}
