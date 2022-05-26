using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NetBrowser_UWP.Models;
using System.Threading.Tasks;
using NetBrowser_UWP.Properties;

namespace NetBrowser_UWP
{
    /// <summary>
    /// Обеспечивает зависящее от конкретного приложения поведение, дополняющее класс Application по умолчанию.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Инициализирует одноэлементный объект приложения. Это первая выполняемая строка разрабатываемого
        /// кода, поэтому она является логическим эквивалентом main() или WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            DataAccess.CreateHistoryFile();
            DataAccess.CreateBookmarksFile();
            DataAccess.CreateConfigFile();
            DataAccess.CreateStartPageFile();
        }
        public static ThemeManager ThemeManager
        => (ThemeManager)Current.Resources["ThemeManager"];

        public static ApplicationViewTitleBar TitleBar;
        public static int ThemeMode;
        private static SearchEngineItem _currentWebEngine;

        public static SearchEngineItem CurrentWebEngine
        {
            get => _currentWebEngine;
            set
            {
                _currentWebEngine = value;
                OnPropertyChanged(nameof(CurrentWebEngine));
            }
        }
        public static event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private static void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Вызывается при обычном запуске приложения пользователем. Будут использоваться другие точки входа,
        /// например, если приложение запускается для открытия конкретного файла.
        /// </summary>
        /// <param name="e">Сведения о запросе и обработке запуска.</param>
        /// 
        public static async Task SetApplicationTheme()
        {
            var mode = await DataTransfer.GetCurrentTheme();
            ThemeManager.LoadThemeByMode(mode);
            ThemeMode = mode;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            TitleBar = ApplicationView.GetForCurrentView().TitleBar;
            await SetApplicationTheme();
            CurrentWebEngine = await DataTransfer.GetCurrentEngine(); //Set Current Search Engine

            // Не повторяйте инициализацию приложения, если в окне уже имеется содержимое,
            // только обеспечьте активность окна
            if (rootFrame == null)
            {
                // Создание фрейма, который станет контекстом навигации, и переход к первой странице
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Загрузить состояние из ранее приостановленного приложения
                }

                // Размещение фрейма в текущем окне
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Если стек навигации не восстанавливается для перехода к первой странице,
                    // настройка новой страницы путем передачи необходимой информации в качестве параметра
                    // навигации
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Обеспечение активности текущего окна
                Window.Current.Activate();
            }
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            if (coreTitleBar != null)
            {
                coreTitleBar.ExtendViewIntoTitleBar = true;
            }

        }


        /// <summary>
        /// Вызывается в случае сбоя навигации на определенную страницу
        /// </summary>
        /// <param name="sender">Фрейм, для которого произошел сбой навигации</param>
        /// <param name="e">Сведения о сбое навигации</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Вызывается при приостановке выполнения приложения.  Состояние приложения сохраняется
        /// без учета информации о том, будет ли оно завершено или возобновлено с неизменным
        /// содержимым памяти.
        /// </summary>
        /// <param name="sender">Источник запроса приостановки.</param>
        /// <param name="e">Сведения о запросе приостановки.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Сохранить состояние приложения и остановить все фоновые операции
            deferral.Complete();
        }


    }
}
