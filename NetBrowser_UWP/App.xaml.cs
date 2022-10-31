using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.ViewModels;
using NetBrowser_UWP.Views;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Notifications;
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
        ///
        public IServiceProvider Services { get; }

        public static ApplicationViewTitleBar TitleBar => ApplicationView.GetForCurrentView().TitleBar;

        public static ThemeItem CurrentTheme;

        public static SearchEngineItem CurrentWebEngine;

        public static ThemeManager ThemeManager =>
            Current.Resources["ThemeManager"] as ThemeManager;


        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;

            this.Services = ConfigureDependencyInjection();
            Ioc.Default.ConfigureServices(Services);
            var dataAccessService = Ioc.Default.GetRequiredService<IDataAccessService>();

            dataAccessService.InitializeHistoryFile();
            dataAccessService.InitializeBookmarksFile();
            dataAccessService.InitializeConfigFile();
            dataAccessService.InitializeStartPageFile();

        }
        
        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        { 

        }

        private static IServiceProvider ConfigureDependencyInjection()
        {
            var serviceCollection = new ServiceCollection();

            //Services & Managers
            serviceCollection.AddSingleton<IDataTransferService, DataTransferService>();
            serviceCollection.AddSingleton<IDataAccessService, DataAccessService>();
            serviceCollection.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            serviceCollection.AddSingleton<IWebView2Service, WebView2Service>();
            //serviceCollection.AddSingleton<IThemeManager, ThemeManager>();


            //ViewModels
            serviceCollection.AddSingleton<MainPageViewModel>();
            serviceCollection.AddTransient<HistoryPageViewModel>();
            serviceCollection.AddTransient<StartPageViewModel>();
            serviceCollection.AddTransient<BookmarksPageViewModel>();
            serviceCollection.AddTransient<PersonalizePageViewModel>();
            serviceCollection.AddTransient<SearchSystemPageViewModel>();

            serviceCollection.AddTransient<AboutAppViewModel>();


            return serviceCollection.BuildServiceProvider();
        }

        

        public static async Task SetApplicationTheme()
        {
            var name = await Ioc.Default.GetRequiredService<ILocalSettingsService>()
                .ReadSettingAsync<string>("CurrentTheme");
            CurrentTheme = ThemeManager.SetRequestedTheme(name);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            //Set Current Search Engine
            CurrentWebEngine = await Ioc.Default.GetRequiredService<IDataTransferService>().GetCurrentSearchEngine();

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
                await SetApplicationTheme();
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
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

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
