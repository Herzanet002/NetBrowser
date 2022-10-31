using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetBrowser_UWP.ViewModels
{
    public class PersonalizePageViewModel : ObservableObject
    {
        private ObservableCollection<ThemeItem> _themesList;
        private ThemeItem _selectedTheme;
        private bool _isSuggestionBarEnabled;
        private bool _isHomeButtonEnabled;
        private int _startPageGridViewOrientation;

        public ICommand SelectThemeCommand => new DelegateCommand(OnSelectedThemeCommandExecuted, () => true);

        private void OnSelectedThemeCommandExecuted()
        {
            _localSettingsService.SaveSettingAsync("CurrentTheme", SelectedTheme.Name);
            App.ThemeManager.SetRequestedTheme(SelectedTheme.Name);
        }

        public ObservableCollection<ThemeItem> ThemesList
        {
            get => _themesList;
            set => SetProperty(ref _themesList, value);
        }
        private readonly ILocalSettingsService _localSettingsService;
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
                _localSettingsService.SaveSettingAsync("IsSuggestionBarEnabled", value);
            }
        }

        public int StartPageGridViewOrientation
        {
            get => _startPageGridViewOrientation;
            set
            {
                SetProperty(ref _startPageGridViewOrientation, value);
                _localSettingsService.SaveSettingAsync("StartPageGridViewOrientation", value);
            }
        }

        public bool IsHomeButtonEnabled
        {
            get => _isHomeButtonEnabled;
            set
            {
                SetProperty(ref _isHomeButtonEnabled, value);
                _localSettingsService.SaveSettingAsync("IsHomeButtonEnabled", value);
            }
        }

        public MainPageViewModel MainViewModel { get; }
        public PersonalizePageViewModel(ILocalSettingsService localSettingsService, MainPageViewModel mainPageViewModel)
        {
            _localSettingsService = localSettingsService;
            MainViewModel = mainPageViewModel;
            InitializePageComponents();
        }

    
        private async void InitializePageComponents()
        {
            IsSuggestionBarEnabled = await _localSettingsService.ReadSettingAsync<bool>("IsSuggestionBarEnabled");
            IsHomeButtonEnabled = await _localSettingsService.ReadSettingAsync<bool>("IsHomeButtonEnabled");
            StartPageGridViewOrientation = await _localSettingsService.ReadSettingAsync<int>("StartPageGridViewOrientation");
            ThemesList = new ObservableCollection<ThemeItem>(Constants.Constants.ThemesDictionary.Values);
            SelectedTheme = App.CurrentTheme;
        }
    }
}
