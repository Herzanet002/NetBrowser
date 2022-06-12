using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NetBrowser_UWP.Converters
{
    public class DateTimeToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var splitstring = value.ToString().Split(":");
            if (splitstring[0].Length == 1) splitstring[0] = "0" + splitstring[0];
            return $"{splitstring[0]}:{splitstring[1]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
