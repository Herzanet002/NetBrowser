using System.Text.RegularExpressions;

namespace NetBrowser_UWP.Helpers;

public static class StringHelpers
{
    public static string StripHtml(this string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
}