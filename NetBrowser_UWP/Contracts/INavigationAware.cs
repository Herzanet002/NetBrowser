namespace NetBrowser_UWP.Contracts;

public interface INavigationAware
{
    void OnNavigatedTo(object parameter);

    void OnNavigatedFrom();
}