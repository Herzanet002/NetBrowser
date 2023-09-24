using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using NetBrowser.Core.Models;

namespace NetBrowser_UWP.Converters;

public class SearchTermItemConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not SearchTermItem searchTermItem) return string.Empty;
        if (!searchTermItem.IsNewSuggestedSearchQuery) return searchTermItem.Query;
        var searchEngineName = App.CurrentWebEngine.Name;
        return $"Искать в {searchEngineName}: {searchTermItem.Query}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return DependencyProperty.UnsetValue;
    }
}