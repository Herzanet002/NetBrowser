using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            ThemeChooserGridView.SelectedIndex = Convert.ToInt32(App.ThemeMode)-1;
        }

        private void ThemeChooserGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataTransfer dataTransfer = new DataTransfer();
            int selectedThemeMode = ThemeChooserGridView.SelectedIndex + 1;
            dataTransfer.SaveCurrentTheme(selectedThemeMode.ToString());
            App.ThemeManager.LoadThemeByMode(selectedThemeMode.ToString());
            ThemeManager.SetRequestedTheme();
        }
    }
}
