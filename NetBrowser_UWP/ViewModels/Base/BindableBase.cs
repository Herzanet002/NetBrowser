using CommunityToolkit.Mvvm.ComponentModel;

namespace NetBrowser_UWP.ViewModels.Base;

/// <summary>
///     A base class for objects of which the properties must be observable.
///     Implementation of INotifyPropertyChanged to simplify models.
/// </summary>
public class BindableBase : ObservableObject
{
}