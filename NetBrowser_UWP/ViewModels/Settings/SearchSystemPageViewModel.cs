using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser.Core.Models;
using NetBrowser.Storage;

namespace NetBrowser_UWP.ViewModels.Settings;

public class SearchSystemPageViewModel : BindableBase
{
    private static ObservableCollection<SearchEngineItem> _listOfEngines;
    private readonly IDataService _dataService;
    private SearchEngineItem _currentEngine;

    public SearchSystemPageViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetEngines();
    }

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
        _dataService.SetDefaultSearchEngineAsync(CurrentEngine);
        App.CurrentWebEngine = CurrentEngine;
    }

    public async Task GetEngines()
    {
        ListOfEngines =
            new ObservableCollection<SearchEngineItem>(await _dataService.GetSearchEngineListAsync());

        var selectedEngine = from item in ListOfEngines
            where item.IsSelected
            select item;
        CurrentEngine = selectedEngine.FirstOrDefault();
    }
}