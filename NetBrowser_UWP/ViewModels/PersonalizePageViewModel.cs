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

        public ICommand SelectThemeCommand => new DelegateCommand(OnSelectedThemeCommandExecuted, () => true);

        private void OnSelectedThemeCommandExecuted()
        {
            _localSettingsService.SaveSettingAsync("CurrentTheme", SelectedTheme.Name);
            App.ThemeManager.SetRequestedTheme(SelectedTheme.Name);
            App.ThemeManager.SetRequestedElementThemeMode();
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


        public PersonalizePageViewModel(ILocalSettingsService localSettingsService)
        {
            _localSettingsService = localSettingsService;
            InitializePageComponents();
        }

    
        private async void InitializePageComponents()
        {
            IsSuggestionBarEnabled = await _localSettingsService.ReadSettingAsync<bool>("IsSuggestionBarEnabled");
            ThemesList = new ObservableCollection<ThemeItem>(Constants.Constants.ThemesDictionary.Values);
            SelectedTheme = App.CurrentTheme;
        }
    }
}
