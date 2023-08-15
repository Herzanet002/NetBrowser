using CommunityToolkit.Mvvm.DependencyInjection;
using System.Numerics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using NetBrowser_UWP.ViewModels.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace NetBrowser_UWP.Views.UserControls;

public sealed partial class FindSuggestBox : UserControl
{
    public FindBoxViewModel ViewModel { get; set; }

    public FindSuggestBox()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetService<FindBoxViewModel>();
        DataContext = ViewModel;
        SearchBox.Translation += new Vector3(0, 0, 32);
    }

    //private void FindBoxOnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    //{

    //}

    //private void FindBoxOnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    //{
    //    throw new System.NotImplementedException();
    //}

    //private void FindBoxOnKeyDown(object sender, KeyRoutedEventArgs e)
    //{
    //    throw new System.NotImplementedException();
    //}
}