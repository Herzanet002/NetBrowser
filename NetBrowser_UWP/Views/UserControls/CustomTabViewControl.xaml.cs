using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;
using NetBrowser_UWP.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NetBrowser_UWP.Views.UserControls;

public sealed partial class CustomTabViewControl : UserControl
{
    public ShellPageViewModel ViewModel { get; set; }

    public CustomTabViewControl()
    {
        ViewModel = Ioc.Default.GetService<ShellPageViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    }
}