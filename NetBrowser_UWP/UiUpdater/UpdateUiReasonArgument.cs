namespace NetBrowser_UWP.UiUpdater;

public class UpdateUiReasonArgument
{
    public UpdateUIReason Reason { get; protected set; }

    public UpdateUiReasonArgument(UpdateUIReason reason)
    {
        Reason = reason;
    }
}