using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace NetBrowser_UWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class AboutAppPage : Page
    {
        public AboutAppPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialogError = new ContentDialog
            {
                Title = "Проверка обновлений",
                Content = "Обновления не найдены",
                CloseButtonText = "Закрыть"
            };

            await dialogError.ShowAsync();
        }
    }
}
