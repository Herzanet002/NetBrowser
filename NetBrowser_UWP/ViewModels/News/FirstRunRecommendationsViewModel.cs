using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.ViewModels.Base;
using Prism.Commands;

namespace NetBrowser_UWP.ViewModels.News;

public class FirstRunRecommendationsViewModel : BindableBase
{
    private readonly ObservableCollection<NewsProvider> _chosenProviders;
    private readonly IDataService _dataService;
    private readonly INewsApiClientService _newsApiClientService;
    private bool _canContinue;
    private ObservableCollection<NewsProvider> _providers;
    private bool _isProgressRingActive = true;
    
    public FirstRunRecommendationsViewModel(IDataService dataService, INewsApiClientService newsApiClientService)
    {
        _dataService = dataService;
        _newsApiClientService = newsApiClientService;
        PageLoadedCommand = new AsyncRelayCommand(OnPageLoaded);
        OkButtonCommand = new AsyncRelayCommand(OnOkButtonCommandExecuted);
        SelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(OnSelectionChangedCommandExecuted);
        Providers = new ObservableCollection<NewsProvider>();
        _chosenProviders = new ObservableCollection<NewsProvider>();
    }
    
    public IAsyncRelayCommand PageLoadedCommand { get; set; }
    public IAsyncRelayCommand OkButtonCommand { get; set; }
    public DelegateCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; set; }
    
    
    public ObservableCollection<NewsProvider> Providers
    {
        get => _providers;
        set => SetProperty(ref _providers, value);
    }
    
    public bool CanContinue
    {
        get => _canContinue;
        set => SetProperty(ref _canContinue, value);
    }
    
    
    public bool IsProgressRingActive
    {
        get => _isProgressRingActive;
        set => SetProperty(ref _isProgressRingActive, value);
    }
    
    private async Task OnOkButtonCommandExecuted()
    {
        await _dataService.AddLikedNewsProvidersAsync(_chosenProviders);
    }
    
    private void OnSelectionChangedCommandExecuted(SelectionChangedEventArgs obj)
    {
        foreach (var item in obj.AddedItems) _chosenProviders.Add(item as NewsProvider);
    
        foreach (var item in obj.RemovedItems) _chosenProviders.Remove(item as NewsProvider);
    
        CanContinue = _chosenProviders.Count > 0;
    }
    
    private async Task OnPageLoaded()
    {
        Providers = new ObservableCollection<NewsProvider>(await _newsApiClientService.GetNewsProvidersAsync());
        IsProgressRingActive = false;
    }
}