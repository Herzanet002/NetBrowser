using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NetBrowser_UWP.Converters
{
    public class TitleTextConverter : IValueConverter
    {
        private const int MAX_LENGTH = 18;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var title = value as string;

            if (title is { Length: > MAX_LENGTH })
                title = title.Substring(0, MAX_LENGTH) + "...";
            return title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
