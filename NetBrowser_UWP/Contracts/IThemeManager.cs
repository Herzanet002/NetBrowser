using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Contracts
{
    public interface IThemeManager
    {
        public ThemeItem GetRequestedTheme(string name);
        public ThemeItem SetRequestedTheme(string themeName);

        public void SetRequestedElementThemeMode();
    }
}
