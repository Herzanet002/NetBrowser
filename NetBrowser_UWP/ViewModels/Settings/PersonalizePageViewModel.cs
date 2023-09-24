using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.ViewModels.Base;
using Windows.ApplicationModel.Core;
using System;
using NetBrowser.Core.Enums;
using NetBrowser_UWP.Contracts.Services.Settings;

namespace NetBrowser_UWP.ViewModels.Settings;

public class PersonalizePageViewModel : BindableBase
{
    private readonly IAppearanceSettingsService _appearanceSettingsService;

    private bool _isAnimationEnabled;
    private bool _isHomeButtonEnabled;
    private bool _isSuggestionBarEnabled;

    private int _startPageGridViewOrientation;
    private int? _tabViewPlacementMode;

    private ThemeItem _selectedTheme;
    private ObservableCollection<ThemeItem> _themesList;

    public event EventHandler ShowNotificationRequested;

    public PersonalizePageViewModel(IAppearanceSettingsService appearanceSettingsService)
    {
        _appearanceSettingsService = appearanceSettingsService;
        InitializePageComponents();
    }

    public ICommand SelectThemeCommand => new RelayCommand(OnSelectedThemeCommandExecuted);

    public ICommand RestartApplicationCommand => new AsyncRelayCommand(async () =>
        await CoreApplication.RequestRestartAsync("-changed_tab_view_property -placement"));

    #region Properties

    public ObservableCollection<ThemeItem> ThemesList
    {
        get => _themesList;
        private set => SetProperty(ref _themesList, value);
    }

    public ThemeItem SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (value == null)
                return;
            SetProperty(ref _selectedTheme, value);
        }
    }

    public bool IsSuggestionBarEnabled
    {
        get => _isSuggestionBarEnabled;
        set
        {
            _appearanceSettingsService.IsSuggestionBarEnabled.SetSetting(value);
            SetProperty(ref _isSuggestionBarEnabled, value);
        }
    }

    public bool IsAnimationEnabled
    {
        get => _isAnimationEnabled;
        set
        {
            _appearanceSettingsService.IsAnimationEnabled.SetSetting(value);
            SetProperty(ref _isAnimationEnabled, value);
        }
    }

    public int StartPageGridViewOrientation
    {
        get => _startPageGridViewOrientation;
        set
        {
            _appearanceSettingsService.StartPageGridViewOrientation.SetSetting(value);
            SetProperty(ref _startPageGridViewOrientation, value);
        }
    }

    public int? TabViewPlacementMode
    {
        get => _tabViewPlacementMode;
        set
        {
            if (value == null)
            {
                return;
            }

            if (_tabViewPlacementMode.HasValue)
            {
                ShowNotificationRequested?.Invoke(this, EventArgs.Empty);
            }

            _appearanceSettingsService.TabViewPlacementMode.SetSetting((TabViewPlacementMode)value.Value);
            SetProperty(ref _tabViewPlacementMode, value);
        }
    }

    public bool IsHomeButtonEnabled
    {
        get => _isHomeButtonEnabled;
        set
        {
            _appearanceSettingsService.IsHomeButtonEnabled.SetSetting(value);
            SetProperty(ref _isHomeButtonEnabled, value);
        }
    }

    #endregion

    private void InitializePageComponents()
    {
        IsSuggestionBarEnabled = _appearanceSettingsService.IsSuggestionBarEnabled.GetSetting();
        IsHomeButtonEnabled = _appearanceSettingsService.IsHomeButtonEnabled.GetSetting();
        IsAnimationEnabled = _appearanceSettingsService.IsAnimationEnabled.GetSetting();
        StartPageGridViewOrientation = _appearanceSettingsService.StartPageGridViewOrientation.GetSetting();
        TabViewPlacementMode = (int)_appearanceSettingsService.TabViewPlacementMode.GetSetting();
        ThemesList = new ObservableCollection<ThemeItem>(Constants.AppConstants.ThemesDictionary.Values);
        SelectedTheme = App.CurrentTheme;
    }

    private void OnSelectedThemeCommandExecuted()
    {
        _appearanceSettingsService.SelectedTheme.SetSetting(SelectedTheme);
        App.ThemeManager.SetRequestedTheme(SelectedTheme.Name);
    }
}