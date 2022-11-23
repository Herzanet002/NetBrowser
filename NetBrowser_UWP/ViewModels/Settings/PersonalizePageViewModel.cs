using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetBrowser_UWP.ViewModels.Settings;

public class PersonalizePageViewModel : ObservableObject
{
    private readonly ILocalSettingsService _localSettingsService;
    private bool _isAnimationEnabled;
    private bool _isHomeButtonEnabled;
    private bool _isSuggestionBarEnabled;
    private ThemeItem _selectedTheme;
    private int _startPageGridViewOrientation;
    private ObservableCollection<ThemeItem> _themesList;

    public IAsyncRelayCommand PersonalizePageLoadedCommand { get; set; }

    public PersonalizePageViewModel(ILocalSettingsService localSettingsService, ShellPageViewModel mainPageViewModel)
    {
        _localSettingsService = localSettingsService;
        MainViewModel = mainPageViewModel;
        PersonalizePageLoadedCommand = new AsyncRelayCommand(OnPersonalizePageLoadedCommandExecuted);
    }

    private async Task OnPersonalizePageLoadedCommandExecuted(CancellationToken ct)
    {
        IsSuggestionBarEnabled = await _localSettingsService.ReadSettingAsync<bool>(nameof(IsSuggestionBarEnabled));
        IsHomeButtonEnabled = await _localSettingsService.ReadSettingAsync<bool>(nameof(IsHomeButtonEnabled));
        IsAnimationEnabled = await _localSettingsService.ReadSettingAsync<bool>(nameof(IsAnimationEnabled));
        StartPageGridViewOrientation =
            await _localSettingsService.ReadSettingAsync<int>(nameof(StartPageGridViewOrientation));
        ThemesList = new ObservableCollection<ThemeItem>(Constants.Constants.ThemesDictionary.Values);
        SelectedTheme = App.CurrentTheme;
    }

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
            _localSettingsService.SaveSettingAsync(nameof(IsSuggestionBarEnabled), value);
        }
    }

    public bool IsAnimationEnabled
    {
        get => _isAnimationEnabled;
        set
        {
            SetProperty(ref _isAnimationEnabled, value);
            _localSettingsService.SaveSettingAsync(nameof(IsAnimationEnabled), value);
        }
    }

    public int StartPageGridViewOrientation
    {
        get => _startPageGridViewOrientation;
        set
        {
            SetProperty(ref _startPageGridViewOrientation, value);
            _localSettingsService.SaveSettingAsync(nameof(StartPageGridViewOrientation), value);
        }
    }

    public bool IsHomeButtonEnabled
    {
        get => _isHomeButtonEnabled;
        set
        {
            SetProperty(ref _isHomeButtonEnabled, value);
            _localSettingsService.SaveSettingAsync(nameof(IsHomeButtonEnabled), value);
        }
    }

    public ShellPageViewModel MainViewModel { get; }

    private void OnSelectedThemeCommandExecuted()
    {
        _localSettingsService.SaveSettingAsync("CurrentTheme", SelectedTheme.Name);
        App.ThemeManager.SetRequestedTheme(SelectedTheme.Name);
    }
}