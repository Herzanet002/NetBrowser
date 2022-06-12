using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace NetBrowser_UWP.Commands
{
    public class OpenFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            var param = (RightTappedRoutedEventArgs)parameter;
            var flyout = FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);
            var options = new FlyoutShowOptions()
            {
                Position = param.GetPosition((FrameworkElement)sender),
                ShowMode = FlyoutShowMode.Standard
            };
            flyout?.ShowAt((FrameworkElement)sender, options);
            return null;
        }
    }
}
