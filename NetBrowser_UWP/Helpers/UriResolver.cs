using System;
using System.Text.RegularExpressions;
using NetBrowser_UWP.Enums;

namespace NetBrowser_UWP.Helpers;

public static class UriResolver
{
    public class UriResolveResult
    {
        public UriResolveResult(Uri uriResult, UriResultType uriResultType)
        {
            UriResult = uriResult;
            UriResultType = uriResultType;
        }

        public Uri? UriResult { get; }

        public UriResultType UriResultType { get; }
    }

    public static UriResolveResult ResolveUri(string address)
    {
        const string addressPattern = @"^(www\.)";
        address = address.Trim();

        if (Uri.TryCreate(address, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp
                || uriResult.Scheme == Uri.UriSchemeHttps
                || uriResult.Scheme == Uri.UriSchemeFtp
                || uriResult.Scheme == Uri.UriSchemeFile))
        {
            return new UriResolveResult(uriResult, UriResultType.ValidAbsoluteUri);
        }

        if (Regex.IsMatch(address, addressPattern)
            && Uri.TryCreate("https://" + address, UriKind.Absolute, out uriResult))
        {
            return new UriResolveResult(uriResult, UriResultType.WithHttpsScheme);
        }

        var result = App.CurrentWebEngine.Prefix + address;
        return Uri.IsWellFormedUriString(result, UriKind.Absolute)
            ? new UriResolveResult(new Uri(result, UriKind.Absolute), UriResultType.Prefixed)
            : new UriResolveResult(null, UriResultType.Malformed);
    }
}