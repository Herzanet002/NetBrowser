using NetBrowser_UWP.Commands;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NetBrowser_UWP.ViewModels
{
    public class PersonalizePageViewModel : ViewModel
    {
        private ObservableCollection<ThemeItem> _themesList;
        private ThemeItem _selectedTheme;
        private bool _isSuggestionBarEnabled;

        public ICommand SelectThemeCommand => new Command(OnSelectedThemeCommandExecuted, _ => true);

        private void OnSelectedThemeCommandExecuted(object obj)
        {
            _dataTransferService.SaveCurrentTheme(SelectedTheme.Name);
            App.ThemeManager.SetRequestedTheme(SelectedTheme.Name);
            App.ThemeManager.SetRequestedElementThemeMode();
        }

        public ObservableCollection<ThemeItem> ThemesList
        {
            get => _themesList;
            set => Set(ref _themesList, value);
        }

        public ThemeItem SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (value == null) return;
                Set(ref _selectedTheme, value);
            }
        }

        public bool IsSuggestionBarEnabled
        {
            get => _isSuggestionBarEnabled;
            set => Set(ref _isSuggestionBarEnabled, value);
        }

        private readonly IDataTransferService _dataTransferService;
        //private readonly IThemeManager _themeManager;
        public PersonalizePageViewModel(IDataTransferService dataTransferService)
        {
            //_themeManager = themeManager;
            _dataTransferService = dataTransferService;

            ThemesList = new ObservableCollection<ThemeItem>(Constants.Constants.ThemesDictionary.Values);
            SelectedTheme = App.CurrentTheme;
        }
    }
}
