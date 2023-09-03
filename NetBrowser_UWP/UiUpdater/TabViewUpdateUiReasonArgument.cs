namespace NetBrowser_UWP.UiUpdater;

public class TabViewUpdateUiReasonArgument : UpdateUiReasonArgument
{
    public object Content { get; }

    public TabViewUpdateUiReasonArgument(object content) : base(UpdateUIReason.TabViewSelectionChanged)
        => Content = content;
}