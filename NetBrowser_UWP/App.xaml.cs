using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Contracts;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Properties;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.ViewModels;
using NetBrowser_UWP.Views;
using NetBrowser_UWP.Views.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

            _container = ConfigureDependencyInjection();
            var dataAccessService = GetService<DataAccessService>();

            dataAccessService.InitializeHistoryFile();
            dataAccessService.InitializeBookmarksFile();
            dataAccessService.InitializeConfigFile();
            dataAccessService.InitializeStartPageFile();
        }

        public static ApplicationViewTitleBar TitleBar;

        private static SearchEngineItem _currentWebEngine;

        private static IServiceProvider _container;

        public static ThemeItem CurrentTheme;

        private IServiceProvider ConfigureDependencyInjection()
        {
            var serviceCollection = new ServiceCollection();

            //Services & Managers
            serviceCollection.AddSingleton<IDataTransferService, DataTransferService>();
            serviceCollection.AddSingleton<IDataAccessService, DataAccessService>();
            //serviceCollection.AddSingleton<IThemeManager, ThemeManager>();

            //Dialogs
            serviceCollection.AddTransient<AddNewStartPageItemDialog>();
            serviceCollection.AddTransient<DeleteBookmarkDialog>();
            serviceCollection.AddTransient<EditBookmarkDialog>();
            serviceCollection.AddTransient<HistoryClearConfirmationDialog>();

            //ViewModels
            serviceCollection.AddSingleton<MainPageViewModel>();
            serviceCollection.AddSingleton<HistoryPageViewModel>();
            serviceCollection.AddTransient<StartPageViewModel>();
            serviceCollection.AddSingleton<BookmarksPageViewModel>();
            serviceCollection.AddSingleton<PersonalizePageViewModel>();


            return serviceCollection.BuildServiceProvider();
        }
        public static SearchEngineItem CurrentWebEngine
        {
            get => _currentWebEngine;
            set
            {
                _currentWebEngine = value;
                OnPropertyChanged(nameof(CurrentWebEngine));
            }
        }

        public static T GetService<T>() where T : class
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(_container, typeof(T)) as T;
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private static void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        public static async Task SetApplicationTheme()
        {
            var name = await GetService<IDataTransferService>().GetCurrentTheme();
            CurrentTheme = ThemeManager.SetRequestedTheme(name);
        }

        public static ThemeManager ThemeManager => (ThemeManager)Current.Resources["ThemeManager"];
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            TitleBar = ApplicationView.GetForCurrentView().TitleBar;
            await SetApplicationTheme();
            CurrentWebEngine = await GetService<IDataTransferService>().GetCurrentSearchEngine(); //Set Current Search Engine

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

            ThemeManager.SetRequestedElementThemeMode();

        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Сохранить состояние приложения и остановить все фоновые операции
            deferral.Complete();
        }



    }
}
