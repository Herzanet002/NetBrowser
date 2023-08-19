using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.Views;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;
using Windows.Storage;
using Microsoft.Toolkit.Uwp.Helpers;
using NetBrowser.Utils;
using NetBrowser_UWP.Constants;

namespace NetBrowser_UWP;

/// <summary>
///     Обеспечивает зависящее от конкретного приложения поведение, дополняющее класс Application по умолчанию.
/// </summary>
public sealed partial class App : Application
{
    public static ThemeItem CurrentTheme;
    public static SearchEngineItem CurrentWebEngine;

    public App()
    {
        InitializeComponent();
        Suspending += OnSuspending;
        UnhandledException += App_UnhandledException;
        Services = ConfigureDependencyInjection();
        Ioc.Default.ConfigureServices(Services);
    }

    public IServiceProvider Services { get; }

    public static ApplicationViewTitleBar TitleBar => ApplicationView.GetForCurrentView().TitleBar;

    public static ThemeManager ThemeManager =>
        Current.Resources["ThemeManager"] as ThemeManager;

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
#if DEBUG
        Debug.WriteLine(e.Exception);
#endif
    }

    private static IServiceProvider ConfigureDependencyInjection()
    {
        var services = new ServiceCollection();

        // Services
        services.AddScoped<IFirstRunAppInitializerService, FirstRunAppInitializerService>();
        services.AddSingleton<IDataService, DataService>();
        services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
        services.AddSingleton<IWebView2Service, WebView2Service>();
        services.AddTransient<INavigationService, NavigationService>();
        services.AddTransient<INavigationViewService, NavigationViewService>();
        services.AddSingleton<AppConfigService>();
        services.AddSingleton<TabViewService>();
        services.AddScoped<IRssWorkerService, RssWorkerService>();

        // ViewModels
        services.RegisterViewModels();

        services.AddLogging(x => x.AddConsole());
        return services.BuildServiceProvider();
    }

    private static async Task SetApplicationTheme()
    {
        var name = await Ioc.Default.GetRequiredService<ILocalSettingsService>()
            .ReadSettingAsync<string>("CurrentTheme");
        CurrentTheme = ThemeManager.SetRequestedTheme(name);
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs e)
    {
        if (SystemInformation.Instance.IsFirstRun ||
            !await Ioc.Default.GetRequiredService<ILocalSettingsService>()
                .ReadSettingAsync<bool>(ApplicationConstants.FirstRunInitResultSettingsKey))
        {
            await Ioc.Default.GetRequiredService<IFirstRunAppInitializerService>()
                .InitializeAppStorageAsync();
        }

        CurrentWebEngine = await Ioc.Default.GetRequiredService<IDataService>().GetCurrentSearchEngineAsync();

        // Не повторяйте инициализацию приложения, если в окне уже имеется содержимое,
        // только обеспечьте активность окна
        if (Window.Current.Content is not Frame rootFrame)
        {
            // Создание фрейма, который станет контекстом навигации, и переход к первой странице
            rootFrame = new Frame();

            rootFrame.NavigationFailed += OnNavigationFailed;

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Загрузить состояние из ранее приостановленного приложения
                //TODO: Загрузить предыдущие вкладки...
            }

            // Размещение фрейма в текущем окне
            Window.Current.Content = rootFrame;
            await SetApplicationTheme();
        }

        if (e.PrelaunchActivated == false)
        {
            if (rootFrame.Content == null)
                // Если стек навигации не восстанавливается для перехода к первой странице,
                // настройка новой страницы путем передачи необходимой информации в качестве параметра
                // навигации
                rootFrame.Navigate(typeof(ShellPage), e.Arguments);
            // Обеспечение активности текущего окна
            Window.Current.Activate();
        }

        CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
    }

    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
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