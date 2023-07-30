using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Markup;

namespace NetBrowser_UWP.Helpers;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class ResourceString : MarkupExtension
{
    private static readonly ResourceLoader ResourceLoader = ResourceLoader.GetForCurrentView();

    public string Name { get; set; }

    protected override object ProvideValue()
    {
        return ResourceLoader.GetString(Name);
    }
}