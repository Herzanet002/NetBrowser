using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using NetBrowser_UWP.Annotations;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public sealed partial class SearchSystemPageSettings : Page, INotifyPropertyChanged
    {

        private static ObservableCollection<SearchEngineItem> _listOfEngines;
        private SearchEngineItem _currentEngine;

        public ObservableCollection<SearchEngineItem> ListOfEngines
        {

            get => _listOfEngines;
            set
            {
                _listOfEngines = value;
                OnPropertyChanged(nameof(ListOfEngines));
            }
        }

        public SearchEngineItem CurrentEngine
        {

            get => _currentEngine;
            set
            {
                _currentEngine = value;
                OnPropertyChanged(nameof(CurrentEngine));
            }
        }
        public SearchSystemPageSettings()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        private async void searchPageSettings_Loaded(object sender, RoutedEventArgs e)
        {
            ListOfEngines = new ObservableCollection<SearchEngineItem>(await DataTransfer.GetSearchEngineList());

            var selectedEngine = from item in ListOfEngines
                                 where item.Mode == "1"
                                 select item;
            CurrentEngine = selectedEngine.First();



        }

        private void comboboxSearchEngine_DropDownClosed(object sender, object e)
        {
            DataTransfer.ChangeSearchEngine(CurrentEngine.Name);
            App.CurrentWebEngine = CurrentEngine;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
