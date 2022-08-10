using System;
using Windows.UI.Xaml.Data;

namespace NetBrowser_UWP.Converters
{
    internal class SearchBoxTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var ValueContent = string.Empty;

            var ForegroundColor = "#000000";

            var charValue = "https://";

            //check if value is null

            if (value != null)
            {
                ValueContent = (string)value;
                // Check for the char ! and change foreground colour 
                if (ValueContent.Contains(charValue))
                {
                    ForegroundColor = "#e40034";
                }
            }

            return ForegroundColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
