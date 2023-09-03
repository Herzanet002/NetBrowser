using System;

namespace NetBrowser_UWP.UiUpdater;

public class InnerPageUpdateUiReasonArgument : UpdateUiReasonArgument
{
    public Type PageType { get; }

    public InnerPageUpdateUiReasonArgument(Type pageType) : base(UpdateUIReason.InnerPageChanged)
    {
        PageType = pageType;
    }
}