using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using NetBrowser_UWP.ViewModels.Settings;
using NetBrowser_UWP.Attributes;

namespace NetBrowser_UWP.Views.Settings;

[PageAddress("app://settings")]
public sealed partial class SettingsPage : NavigationViewContentPage
{
    public SettingsPage(Type pageType = default)
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<SettingsPageViewModel>();
        DataContext = ViewModel;
        ViewModel.Initialize(ContentFrame, SettingsNavigationView, pageType ?? typeof(MainItemPageSettings));
    }

    public SettingsPageViewModel ViewModel { get; set; }

    public override Type GetInnerPageType()
        => ViewModel.InnerPageType;
}