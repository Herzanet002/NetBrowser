using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NetBrowser_UWP.Converters;

public class TitleLetterConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var rx = new Regex(@"^((https?|ftp)://)?(www\.)?(?<domain>[^/]+)(/|$)");
        if (value is not string text) return string.Empty;
        var match = rx.Match(text);
        return match.Success ? match.Groups["domain"].Value[0].ToString().ToUpper() : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}