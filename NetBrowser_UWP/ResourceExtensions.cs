using Windows.ApplicationModel.Resources;

namespace NetBrowser_UWP;

internal static class ResourceExtensions
{
    private static readonly ResourceLoader _resourceLoader = new();

    public static string GetLocalized(string resourceKey) => _resourceLoader.GetString(resourceKey);
}
