using NetBrowser_UWP.Contracts.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NetBrowser_UWP.Services;

internal class UiExperienceService : ObservableObject, IUiExperienceService
{
    private string _appTitleText;
    private string _searchBoxText;

    public string AppTitleText
    {
        get => _appTitleText;
        set => SetProperty(ref _appTitleText, value);
    }

    public string SearchBoxText
    {
        get => _searchBoxText;
        set => SetProperty(ref _searchBoxText, value);
    }

    public void SetUiLabels(string appTitleText, string searchBoxText)
    {
        AppTitleText = appTitleText;
        SearchBoxText = searchBoxText;
    }
}