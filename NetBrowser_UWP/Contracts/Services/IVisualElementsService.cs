namespace NetBrowser_UWP.Contracts.Services
{
    public interface IVisualElementsService
    {
        void SetProgressRingActivity(bool isActive);
        void SetVisualUiElementStates(object sender);
        void SetVisualUiLabels(string appTitleText, string searchBoxText);
        void SetBookmarkIconState(bool isAccessable);
        void SetBookmarkButtonAppearance();
    }
}
