using System;
using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class PersonalizePageSettings : Page
    {
        public PersonalizePageSettings()
        {
            this.InitializeComponent();
            ThemeChooserGridView.SelectedIndex = Convert.ToInt32(App.ThemeMode) - 1;
        }

        private void ThemeChooserGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            int selectedThemeMode = ThemeChooserGridView.SelectedIndex + 1;
            DataTransfer.SaveCurrentTheme(selectedThemeMode.ToString());
            App.ThemeManager.LoadThemeByMode(selectedThemeMode);
            ThemeManager.SetRequestedTheme();
        }
    }
}
