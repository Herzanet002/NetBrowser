namespace NetBrowser_UWP.Contracts.Services;

public interface IUiExperienceService
{
    public string AppTitleText { get; set; }
    public string SearchBoxText { get; set; }
    void SetUiLabels(string appTitleText, string searchBoxText);
}