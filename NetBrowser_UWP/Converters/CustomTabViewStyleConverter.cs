using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using NetBrowser.Core.Enums;

namespace NetBrowser_UWP.Converters;

public class CustomTabViewStyleConverter : IValueConverter
{
    public Style TopStyle { get; set; }

    public Style BottomStyle { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
        => value is TabViewPlacementMode.Top ? TopStyle : BottomStyle;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => DependencyProperty.UnsetValue;
}