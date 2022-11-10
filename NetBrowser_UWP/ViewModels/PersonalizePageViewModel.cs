using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NetBrowser_UWP.Services;

namespace NetBrowser_UWP.ViewModels
{
    public class PersonalizePageViewModel : ObservableObject
    {
        private ObservableCollection<ThemeItem> _themesList;
        private ThemeItem _selectedTheme;
        private bool _isSuggestionBarEnabled;
        private bool _isHomeButtonEnabled;
        private bool _isAnimationEnabled;
        private int _startPageGridViewOrientation;
        
        private readonly ILocalSettingsService _localSettingsService;
        private readonly VisualElementsService _visualElementsService;

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
                _visualElementsService.VisibilityHomeButton = value;
            }
        }

        public PersonalizePageViewModel(ILocalSettingsService localSettingsService, VisualElementsService visualElementsService)
        {
            _localSettingsService = localSettingsService;
            _visualElementsService = visualElementsService;
            InitializePageComponents();
        }

        private async void InitializePageComponents()
        {
            IsSuggestionBarEnabled = await _localSettingsService.ReadSettingAsync<bool>(nameof(IsSuggestionBarEnabled));
            IsHomeButtonEnabled = await _localSettingsService.ReadSettingAsync<bool>(nameof(IsHomeButtonEnabled));
            IsAnimationEnabled = await _localSettingsService.ReadSettingAsync<bool>(nameof(IsAnimationEnabled));
            StartPageGridViewOrientation = await _localSettingsService.ReadSettingAsync<int>(nameof(StartPageGridViewOrientation));
            ThemesList = new ObservableCollection<ThemeItem>(Constants.Constants.ThemesDictionary.Values);
            SelectedTheme = App.CurrentTheme;
        }
    }
}