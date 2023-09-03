using System.Diagnostics.CodeAnalysis;

namespace NetBrowser_UWP.UiUpdater;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum UpdateUIReason : uint
{
    WebView,
    InnerPageChanged,
    TabViewSelectionChanged
}