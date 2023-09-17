using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.TemplateSelectors;

public class FindBoxItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate SuggestionNormalTemplate { get; set; }
    public DataTemplate SuggestionChosenTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (container is not ListViewItem listViewItem)
        {
            return SuggestionNormalTemplate;
        }

        if (listViewItem.Tag != null && long.TryParse(listViewItem.Tag.ToString(), out var token))
        {
            listViewItem.UnregisterPropertyChangedCallback(SelectorItem.IsSelectedProperty, token);
        }

        listViewItem.Tag = listViewItem.RegisterPropertyChangedCallback(SelectorItem.IsSelectedProperty,
            (_, _) =>
            {
                listViewItem.ContentTemplateSelector = null;
                listViewItem.ContentTemplateSelector = this;
            });


        return listViewItem.IsSelected && item is SearchTermItem { LastTimeAccess: not null }
            ? SuggestionChosenTemplate
            : SuggestionNormalTemplate;
    }
}