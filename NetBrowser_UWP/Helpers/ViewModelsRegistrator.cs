using NetBrowser_UWP.ViewModels.News;
using NetBrowser_UWP.ViewModels.Settings;
using NetBrowser_UWP.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace NetBrowser_UWP.Helpers
{
    public static class ViewModelsRegistrator
    {
        public static void RegisterViewModels(this ServiceCollection services)
        {
            services.AddSingleton<ShellPageViewModel>();
            services.AddSingleton<MainSettingsPageViewModel>();
            services.AddTransient<HistoryPageViewModel>();
            services.AddTransient<AllNewsPageViewModel>();
            services.AddTransient<FavoriteNewsPageViewModel>();
            services.AddTransient<StartPageViewModel>();
            services.AddTransient<BookmarksPageViewModel>();
            services.AddTransient<PersonalizePageViewModel>();
            services.AddTransient<SearchSystemPageViewModel>();
            services.AddTransient<SettingsPageViewModel>();
            services.AddTransient<FirstRunRecommendationsViewModel>();
            services.AddSingleton<NewsShellPageViewModel>();
            services.AddSingleton<NewsSettingsViewModel>();
            services.AddTransient<RecommendationsNewsPageViewModel>();
            services.AddTransient<AboutAppViewModel>();
        }
    }
}
