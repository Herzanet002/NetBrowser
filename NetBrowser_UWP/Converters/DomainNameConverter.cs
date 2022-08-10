using System;
using Windows.UI.Xaml.Data;

namespace NetBrowser_UWP.Converters
{
    public class DomainNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string val) return null;
            if (string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val)) return null;
            var uri = new Uri(val);
            return uri.GetLeftPart(UriPartial.Authority);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
