using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NetBrowser_UWP.Converters;

public class HostNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not string val) return DependencyProperty.UnsetValue;
        if (string.IsNullOrWhiteSpace(val)) return DependencyProperty.UnsetValue;
        var uri = new Uri(val);
        return uri.GetLeftPart(UriPartial.Authority);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}