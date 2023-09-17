using System.Text.RegularExpressions;

namespace NetBrowser.Utils;

public static class StringHelpers
{
    public static string StripHtml(this string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
}