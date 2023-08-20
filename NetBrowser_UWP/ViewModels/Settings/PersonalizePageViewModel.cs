using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Contracts.Services.Settings;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.ViewModels.Base;
using Prism.Commands;

namespace NetBrowser_UWP.ViewModels.Settings;

public class PersonalizePageViewModel : BindableBase
{
    private readonly IAppearanceSettingsService _appearanceSettingsService;
    private bool _isAnimationEnabled;
    private bool _isHomeButtonEnabled;
    private bool _isSuggestionBarEnabled;
    private ThemeItem _selectedTheme;
    private int _startPageGridViewOrientation;
    private ObservableCollection<ThemeItem> _themesList;

    public PersonalizePageViewModel(IAppearanceSettingsService appearanceSettingsService,
        ShellPageViewModel mainPageViewModel)
    {
        _appearanceSettingsService = appearanceSettingsService;
        MainViewModel = mainPageViewModel;
        PersonalizePageLoadedCommand = new AsyncRelayCommand(OnPersonalizePageLoadedCommandExecuted);
    }

    public IAsyncRelayCommand PersonalizePageLoadedCommand { get; set; }

    public ICommand SelectThemeCommand => new DelegateCommand(OnSelectedThemeCommandExecuted, () => true);

    public ObservableCollection<ThemeItem> ThemesList
    {
        get => _themesList;
        set => SetProperty(ref _themesList, value);
    }

    public ThemeItem SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (value == null) return;
            SetProperty(ref _selectedTheme, value);
        }
    }

    public bool IsSuggestionBarEnabled
    {
        get => _isSuggestionBarEnabled;
        set
        {
            SetProperty(ref _isSuggestionBarEnabled, value);
            _appearanceSettingsService.IsSuggestionBarEnabled = value;
        }
    }

    public bool IsAnimationEnabled
    {
        get => _isAnimationEnabled;
        set
        {
            SetProperty(ref _isAnimationEnabled, value);
            _appearanceSettingsService.IsAnimationEnabled = value;
        }
    }

    public int StartPageGridViewOrientation
    {
        get => _startPageGridViewOrientation;
        set
        {
            SetProperty(ref _startPageGridViewOrientation, value);
            _appearanceSettingsService.StartPageGridViewOrientation = value;
        }
    }

    public bool IsHomeButtonEnabled
    {
        get => _isHomeButtonEnabled;
        set
        {
            SetProperty(ref _isHomeButtonEnabled, value);
            _appearanceSettingsService.IsHomeButtonEnabled = value;
        }
    }

    public ShellPageViewModel MainViewModel { get; }

    private async Task OnPersonalizePageLoadedCommandExecuted(CancellationToken ct)
    {
        IsSuggestionBarEnabled = _appearanceSettingsService.IsSuggestionBarEnabled;
        IsHomeButtonEnabled = _appearanceSettingsService.IsHomeButtonEnabled;
        IsAnimationEnabled = _appearanceSettingsService.IsAnimationEnabled;
        StartPageGridViewOrientation = _appearanceSettingsService.StartPageGridViewOrientation;
        ThemesList = new ObservableCollection<ThemeItem>(Constants.ApplicationConstants.ThemesDictionary.Values);
        SelectedTheme = _appearanceSettingsService.SelectedTheme;
    }

    private void OnSelectedThemeCommandExecuted()
    {
        _appearanceSettingsService.SelectedTheme = SelectedTheme;
        App.ThemeManager.SetRequestedTheme(SelectedTheme.Name);
    }
}